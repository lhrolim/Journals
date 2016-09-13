using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Journals.Client.Util {
    public static class JsonUtil {

        public static string StringValue(this JObject ob, string propertyName) {
            var prop = ob.Property(propertyName);
            if (prop == null || prop.Value == null) {
                return null;
            }
            var val = prop.Value;
            if (val is JValue) {
                var jvalValue = ((JValue)val).Value;
                return jvalValue == null ? null : jvalValue.ToString();
            }
            return val.ToString();
        }

    }
}
