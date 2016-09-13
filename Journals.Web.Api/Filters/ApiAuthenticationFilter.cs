using System.Threading.Tasks;
using System.Web.Http.Controllers;
using journals.commons.SimpleInjector;
using Journals.Web.Api.Providers;

namespace Journals.Web.Api.Filters {
    /// <summary>
    /// Custom Authentication Filter Extending basic Authentication
    /// </summary>
    public class ApiAuthenticationFilter : GenericAuthenticationFilter {
        /// <summary>
        /// Default Authentication Constructor
        /// </summary>
        public ApiAuthenticationFilter() {
        }

        /// <summary>
        /// AuthenticationFilter constructor with isActive parameter
        /// </summary>
        /// <param name="isActive"></param>
        public ApiAuthenticationFilter(bool isActive)
            : base(isActive) {
        }

        /// <summary>
        /// Protected overriden method for authorizing user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override async Task<bool> OnAuthorizeUser(string username, string password,
            HttpActionContext actionContext) {
            var provider = SimpleInjectorGenericFactory.Instance.GetObject<SecurityFacade>();

            if (provider != null) {
                return await provider.Authenticate(username, password) !=null;
            }
            return false;
        }
    }
}