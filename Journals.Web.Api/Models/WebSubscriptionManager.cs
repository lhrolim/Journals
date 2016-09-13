using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using journals.commons.Model.dao;
using journals.commons.Model.Dto;
using journals.commons.Model.Entities;
using journals.commons.Model.Manager;
using journals.commons.Security;
using journals.commons.SimpleInjector;
using journals.commons.Util;

namespace Journals.Web.Api.Models {

    public class WebSubscriptionManager : SubscriptionManager {

        private readonly ISecurityFacade _securityFacade;
        private readonly IGenericDao _dao;
        private readonly PublicationManager _publicationManager;

        public WebSubscriptionManager(ISecurityFacade securityFacade, IGenericDao dao, PublicationManager publicationManager) : base(dao) {
            _securityFacade = securityFacade;
            _dao = dao;
            _publicationManager = publicationManager;
        }


        public async Task Subscribe(int journalId) {
            var user = _securityFacade.CurrentUser();
            var subscription = new Subscription() { Journal = new Journal() { Id = journalId }, User = new User() { Id = user.UserId } };
            //TODO: notify clients
            await _dao.Save(subscription);
        }



        public async Task<bool> UnSubscribe(int journalId, int userId) {
            return await _dao.ExecuteSql("delete from Subscription where journal_id = ? and user_id = ? ", journalId, userId) > 0;
        }

        public async Task<Publication> AddPublication(Publication publication, int journalId, string base64Data) {
            publication.JournalId = journalId;


            var savedPublication = await _publicationManager.SavePublication(publication, base64Data);
            //TODO: notify clients
            return savedPublication;
        }

        public async Task<SubscriptionDataDto> DownloadSubscriptionData(UserIdentity user, DateTime? lastSyncDate) {
            var mySubscriptions = await _dao.FindByQuery<Subscription>(Subscription.ByUser, null, user.UserId);
            if (lastSyncDate == null) {
                //searching since last month, by default
                lastSyncDate = DateTime.Now.AddMonths(-1);
            }
            var journals = await
                    _dao.FindByQuery<Journal>(Journal.ByIdsAndDate, null, mySubscriptions.Select(m => m.Journal.Id),
                        lastSyncDate);
            foreach (var journal in journals) {
                //cleaning up non needed proxies
                journal.Subscriptions = null;
                journal.Publications = null;
            }

            var publications = await
                    _dao.FindByQuery<Publication>(Publication.ByJournalIdsAndDate, null, mySubscriptions.Select(m => m.Journal.Id), lastSyncDate);

            return new SubscriptionDataDto() { NewJournals = journals, NewPublications = publications };

        }
    }
}