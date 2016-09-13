using System;
using System.Threading.Tasks;
using System.Web.Http;
using journals.commons.Model.dao;
using journals.commons.Model.Entities;
using journals.commons.Util;
using Journals.Web.Api.Models;
using Journals.Web.Api.Providers;

namespace Journals.Web.Api.Controllers {

    /// <summary>
    /// Controller for retrieving the content (pdf) of one given publication. 
    /// </summary>
    [Authorize]
    public class PublicationController : ApiController {

        private readonly SecurityFacade _securityFacade;
        private readonly IGenericDao _dao;
        private readonly WebSubscriptionManager _webSubscriptionManager;

        public PublicationController(SecurityFacade securityFacade, IGenericDao dao, WebSubscriptionManager webSubscriptionManager) {
            _securityFacade = securityFacade;
            _dao = dao;
            _webSubscriptionManager = webSubscriptionManager;
        }

        [HttpGet]
        public async Task<PublicationData> DownloadPublication(int publicationId) {
            var pubData = await _dao.FindSingleByQuery<PublicationData>(PublicationData.ByPublication, publicationId);
            //to avoid extra serialization
            pubData.Publication = null;
            pubData.Base64String = Convert.ToBase64String(CompressionUtil.Decompress(pubData.BinaryContent));
            pubData.BinaryContent = null;
            return pubData;
        }




    }
}