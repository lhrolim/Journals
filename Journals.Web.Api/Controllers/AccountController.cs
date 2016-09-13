using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using journals.commons.Model.Entities;
using journals.commons.Security;
using Microsoft.Owin.Security;

namespace Journals.Web.Api.Controllers {

    /// <summary>
    /// Controller for handling user registration
    /// </summary>
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController {
        private readonly ISecurityFacade _securityFacade;

        public AccountController(ISecurityFacade securityFacade) {
            _securityFacade = securityFacade;
        }



        private IAuthenticationManager Authentication {
            get {
                return Request.GetOwinContext().Authentication;
            }
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(User userModel) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var result = await _securityFacade.RegisterUser(userModel);
                return Ok();
            } catch (RegistrationException e) {
                ModelState.AddModelError("", e.Message);
                return BadRequest(ModelState);
            } catch (Exception) {
                return InternalServerError();
            }
        }

    }
}
