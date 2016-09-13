using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using journals.commons.SimpleInjector;
using journals.commons.Util;
using Journals.Client.Model;

namespace Journals.Client.Util {
    public class RestService : ISingletonComponent {
        private readonly JournalViewModel _journalViewModel;

        public RestService(JournalViewModel journalViewModel) {
            _journalViewModel = journalViewModel;
        }

        public async Task<Tuple<HttpStatusCode, string>> Get(string relPath, string queryString) {
            var basePath = _journalViewModel.Config.Url + "api/generic/";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + _journalViewModel.Config.Token }
            };
            return await RestUtil.CallRestApi(basePath + relPath + "?" + queryString, "get", headers);
        }


    }
}
