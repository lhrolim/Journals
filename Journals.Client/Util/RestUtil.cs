using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Journals.Client.Util {
    public static class RestUtil {
        public static async Task<Tuple<HttpStatusCode,string>> CallRestApi(string url, string method, Dictionary<string, string> headers = null, string payload = null) {
            var request = DoBuildRequest(url, method, headers, payload);
            // fetch response
            var response = (HttpWebResponse) await request.GetResponseAsync();

            return new Tuple<HttpStatusCode, string>(response.StatusCode, ConvertResponseToText(response));

        }


        private static HttpWebRequest DoBuildRequest(string url, string method, Dictionary<string, string> headers, string payload) {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.ContentType = "application/json";
            // headers
            if (headers != null) {
                foreach (var header in headers) {
                    if (header.Key.Equals("Content-Type")) {
                        request.ContentType = header.Value;
                    } else {
                        request.Headers[header.Key] = header.Value;
                    }
                }
            }

            // write payload to requests stream
            if (!string.IsNullOrEmpty(payload)) {
                var body = Encoding.UTF8.GetBytes(payload);
                request.ContentLength = body.Length;
                using (var requestStream = request.GetRequestStream()) {
                    requestStream.Write(body, 0, body.Length);
                }
            }
            return request;
        }

        public static string ConvertResponseToText(WebResponse callRestApiSync) {
            using (var responseStream = callRestApiSync.GetResponseStream()) {
                if (responseStream == null) {
                    return null;
                }
                using (var responseReader = new StreamReader(responseStream)) {
                    // parse xml response
                    var text = responseReader.ReadToEnd();
                    return text;
                }
            }
        }
    }
}
