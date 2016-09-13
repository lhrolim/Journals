using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using journals.commons.Model.dao;
using journals.commons.SimpleInjector;
using Journals.Client.Util;
using Newtonsoft.Json.Linq;
using ServiceStack;

namespace Journals.Client.Model {

    /// <summary>
    /// Responsible for handling login authentication. 
    /// 
    /// Goes to server every time in order to validate login 
    /// 
    /// </summary>
    public class ClientLoginManager : ISingletonComponent {
        private readonly IGenericDao _dao;
        private readonly JournalViewModel _jvm;
        private RestService _restService;

        public ClientLoginManager(IGenericDao dao, JournalViewModel jvm, RestService restService) {
            _dao = dao;
            _jvm = jvm;
            _restService = restService;
        }

        public async Task<bool> Login(string userName, string password, string serverUrl) {
            var data = "grant_type=password&username={0}&password={1}".Fmt(userName, password);

            var response = await RestUtil.CallRestApi(serverUrl + "/token", "post",
                new Dictionary<string, string>() { { "Content-Type", "application/x-www-form-urlencoded" } },
                data);
            if (response.Item1 != HttpStatusCode.OK) {
                //TODO: handle error
                return false;
            }

            

            var result = response.Item2;
            var token = JObject.Parse(result).StringValue("access_token");

            

            var config = new ClientConfiguration {
                Id = 1,
                Token = token,
                Url = serverUrl,
            };

            _jvm.Config = config;
            
            await _dao.Save(config);
            
            return true;
        }

    }
}
