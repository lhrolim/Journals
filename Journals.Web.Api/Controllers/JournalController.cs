using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Http;
using journals.commons.Model.dao;
using journals.commons.Model.Entities;
using Journals.Web.Api.Models;
using Journals.Web.Api.Providers;

namespace Journals.Web.Api.Controllers {

    /// <summary>
    /// Controller for handling operations associated with a journal, like searching, creating a journal as well as finding and adding publications 
    /// </summary>
    [Authorize]
    public class JournalController : ApiController {

        private readonly SecurityFacade _securityFacade;
        private readonly IGenericDao _dao;
        private readonly WebSubscriptionManager _webSubscriptionManager;

        public JournalController(SecurityFacade securityFacade, IGenericDao dao, WebSubscriptionManager webSubscriptionManager) {
            _securityFacade = securityFacade;
            _dao = dao;
            _webSubscriptionManager = webSubscriptionManager;
        }

        [HttpGet]
        public async Task<IList<JournalDto>> ListJournals([FromUri]PaginationData paginationData) {
            var user = _securityFacade.CurrentUser();
            return await _webSubscriptionManager.LookupJournals(user, paginationData);
        }


        [HttpGet]
        public async Task<IList<Publication>> FindPublicationsOfJournal([FromUri]PaginationData paginationData, [FromUri]int journalid) {
            return await _dao.FindByQuery<Publication>(Publication.ByJournalId, paginationData, journalid);
        }

        [HttpPost]
        public async Task<Journal> CreateJournal(Journal journalData) {
            return await _dao.Save(journalData);
        }


        [HttpPost]
        public async Task<IHttpActionResult> AddPublication(PublicationDTO publicationdto) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var pub = await _webSubscriptionManager.AddPublication(publicationdto.Publication, publicationdto.JournalId, publicationdto.Base64Data);
            return Ok(pub);
        }

        public class PublicationDTO {
            [Required]
            public Publication Publication {
                get; set;
            }
            [Required]
            public string Base64Data {
                get; set;
            }
            [Required]
            public int JournalId {
                get; set;
            }
        }


        [HttpPost]
        public async Task Subscribe(int journalId) {
            await _webSubscriptionManager.Subscribe(journalId);
        }

        [HttpPost]
        public async Task UnSubscribe(int journalId) {
            var user = _securityFacade.CurrentUser();
            await _webSubscriptionManager.UnSubscribe(journalId, user.UserId);
        }

    }
}