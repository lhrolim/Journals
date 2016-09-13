using System;
using System.Threading.Tasks;
using System.Web.Http;
using journals.commons.Model.dao;
using journals.commons.Model.Dto;
using Journals.Web.Api.Models;
using Journals.Web.Api.Providers;

namespace Journals.Web.Api.Controllers {

    /// <summary>
    ///  Controller for retrieving subscription data (newJournals and newPublications) for a given user
    /// </summary>
    [Authorize]
    public class SubscriptionController : ApiController {

        private readonly SecurityFacade _securityFacade;
        private readonly IGenericDao _dao;
        private readonly WebSubscriptionManager _webSubscriptionManager;

        public SubscriptionController(SecurityFacade securityFacade, IGenericDao dao, WebSubscriptionManager webSubscriptionManager) {
            _securityFacade = securityFacade;
            _dao = dao;
            _webSubscriptionManager = webSubscriptionManager;
        }

        [HttpGet]
        public async Task<SubscriptionDataDto> DownloadSubscriptions(DateTime? syncDate) {
            var user = _securityFacade.CurrentUser();
            return await _webSubscriptionManager.DownloadSubscriptionData(user, syncDate);
        }


      



    }
}