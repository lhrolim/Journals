using System;
using System.Threading.Tasks;
using journals.commons.Model.dao;
using journals.commons.Model.Dto;
using journals.commons.Model.Entities;
using journals.commons.SimpleInjector;
using journals.commons.Util;
using journals.commons.Util.Redis;
using Journals.Client.Util;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace Journals.Client.Model {
    public class PublicationSyncManager : ISingletonComponent {
        private readonly ILog Log = log4net.LogManager.GetLogger(typeof(PublicationSyncManager));

        private readonly IGenericDao _dao;
        private readonly JournalViewModel _jvm;
        private readonly RestService _restService;
        private readonly RedisService _redisService;
        private readonly NewtonsoftSerializer _newtonsoftSerializer;


        public PublicationSyncManager(IGenericDao dao, JournalViewModel jvm, RestService restService, RedisService redisService) {
            _dao = dao;
            _jvm = jvm;
            _restService = restService;
            _redisService = redisService;
            _newtonsoftSerializer = new NewtonsoftSerializer(new JsonSerializerSettings() { });

        }

        public async Task HandleSubscriptions() {

            await _redisService.SubscriptionAdded(_jvm.UserName, async (channel, message) => {
                var newJournal = _newtonsoftSerializer.Deserialize<Journal>(message);
                await _dao.Save(newJournal);

                //subscribing to receive publications of this new journal
                await SubscribeToJournal(newJournal);

            });

            await _redisService.SubscriptionRemoved(_jvm.UserName, async (channel, message) => {
                var journalId = (int)message;

                Log.InfoFormat("processing unsubscription of journal " + journalId);

                using (var session = _dao.GetSession()) {

                    await _dao.ExecuteSql("delete from publication_data where publication_id in (select p.id from publication p where p.journal_id = ?) ", session, journalId);
                    await _dao.ExecuteSql("delete from Publication where journal_id = ? ", session, journalId);
                    await _dao.ExecuteSql("delete from Journal where id = ? ", session, journalId);

                }

                //Cancelling Journal subscription
                await _redisService.UnSubscribeToJournal(journalId);

            });

            var existingJournals = await _dao.FindByQuery<Journal>("from Journal");

            foreach (var journal in existingJournals) {
                await SubscribeToJournal(journal);
            }

        }

        private async Task SubscribeToJournal(Journal journal) {
            Log.InfoFormat("subscribing to journal " + journal.Name);

            await _redisService.SubscribeToJournal(journal.Id.Value, (channel2, message2) => {
                var newPublication = _newtonsoftSerializer.Deserialize<Publication>(message2);
                _dao.Save(newPublication);
            });
        }


        public async Task SyncData() {
            var lastSyncDate = _jvm.Config?.SyncDate;
            if (lastSyncDate == null) {
                //picking maximum of a month
                lastSyncDate = DateTime.Now.AddMonths(-1);
            }

            try {
                var data = await _restService.Get("/Subscription/DownloadSubscriptions", "syncDate={0}".Fmt(lastSyncDate));
                var subsData = JObject.Parse(data.Item2).ToObject<SubscriptionDataDto>();
                await _dao.BulkSave(subsData.NewJournals);
                await _dao.BulkSave(subsData.NewPublications);
                if (_jvm.Config == null) {
                    _jvm.Config = new ClientConfiguration() {
                        Url = "xxx",
                        Id = 1,
                    };
                }
                _jvm.Config.SyncDate = DateTime.Now;
                await _dao.Save(_jvm.Config);



            } catch (Exception e) {
                Log.Error(e);
            }


        }

    }
}
