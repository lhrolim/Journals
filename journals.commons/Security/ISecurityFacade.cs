using System.Threading.Tasks;
using journals.commons.Model.Entities;
using journals.commons.SimpleInjector;

namespace journals.commons.Security
{
    public interface ISecurityFacade : ISingletonComponent {
        Task<UserIdentity> Authenticate(string username, string password);
        UserIdentity CurrentUser();
        Task<UserIdentity> RegisterUser(User userModel);

        
    }
    
}