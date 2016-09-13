using System.Web.Http;

namespace Journals.Web.Api.SimpleInjectorUtil
{
    public interface IAPIControllerFactory
    {
        ApiController CreateNew(string name);
    }
}