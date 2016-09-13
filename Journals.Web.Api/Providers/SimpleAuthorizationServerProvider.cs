using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Journals.Web.Api.Providers {
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider {

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context) {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context) {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            var securityFacade = SecurityFacade.GetInstance();

            var user = await securityFacade.Authenticate(context.UserName, context.Password);

            if (user == null) {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Publisher.ToString()));
            identity.AddClaim(new Claim("publisher", user.Publisher.ToString()));

            context.Validated(identity);

            var props = new AuthenticationProperties(new Dictionary<string, string>{
                    {"publisher", user.Publisher.ToString()}
            });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);

        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context) {

            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary) {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}