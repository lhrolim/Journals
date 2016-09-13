using System;
using System.Threading.Tasks;
using journals.commons.Model.dao;
using journals.commons.Model.Dto;
using journals.commons.SimpleInjector;
using journals.commons.Util;
using Journals.Client.Util;
using log4net;
using Newtonsoft.Json.Linq;

namespace Journals.Client.Model {
    public class PublicationSyncManager : ISingletonComponent {
        private ILog Log = log4net.LogManager.GetLogger(typeof(PublicationSyncManager));

        private readonly IGenericDao _dao;
        private readonly JournalViewModel _jvm;
        private readonly RestService _restService;

        

        public PublicationSyncManager(IGenericDao dao, JournalViewModel jvm, RestService restService) {
            _dao = dao;
            _jvm = jvm;
            _restService = restService;
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
