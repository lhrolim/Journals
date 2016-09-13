using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using journals.commons.Model.dao;
using journals.commons.Model.Dao;
using journals.commons.Model.Entities;
using journals.commons.Security;
using journals.commons.SimpleInjector;
using journals.commons.Util;

namespace Journals.Web.Api.Providers {

    public class SecurityFacade : ISecurityFacade {

        private readonly IGenericDao _dao;


        private static readonly IDictionary<string, UserIdentity> Users = new ConcurrentDictionary<string, UserIdentity>();


        public SecurityFacade(IGenericDao dao) {
            _dao = dao;
        }

        public static SecurityFacade GetInstance() {
            return SimpleInjectorGenericFactory.Instance.GetObject<SecurityFacade>(typeof(SecurityFacade));
        }

        public async Task<UserIdentity> Authenticate(string username, string password) {
            var user = await _dao.FindSingleByQuery<User>(User.ByUserName, username);
            if (user == null) {
                return null;
            }
            if (user.Password.Equals(AuthUtils.GetSha1HashData(password))) {

                var identity = user.ToIdentity();
                if (Users.ContainsKey(username)) {
                    Users.Remove(username);
                }
                Users.Add(username, identity);
                var genericPrincipal = new GenericPrincipal(identity, null);
                Thread.CurrentPrincipal = genericPrincipal;
                return identity;
            }
            return null;
        }

        private static string CurrentPrincipalLogin() {
            var identity = Thread.CurrentPrincipal.Identity;
            return identity.Name;
//            if (identity is ClaimsIdentity) {
//                return ((ClaimsIdentity)identity).Name;
//            }
        }

        public UserIdentity CurrentUser() {
            var currLogin = CurrentPrincipalLogin();
            if (Users.ContainsKey(currLogin)) {
                return Users[currLogin];
            }
            return null;
        }

        public async Task<UserIdentity> RegisterUser(User user) {
            if (!user.Password.Equals(user.ConfirmPassword)) {
                throw new RegistrationException("Passwords do not match");
            }

            var dbUser = await _dao.FindSingleByQuery<User>(User.ByUserName, user.Login);
            if (dbUser != null) {
                throw new RegistrationException("user {0} already registered".Fmt(user.Login));
            }
            try {
                user.Password = AuthUtils.GetSha1HashData(user.Password);
                var savedUser = await _dao.Save(user);
                return savedUser.ToIdentity();
            } catch (ModelValidationException e) {
                throw new RegistrationException(e);
            }

        }
    }
}