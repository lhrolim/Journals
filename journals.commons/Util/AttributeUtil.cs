using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace journals.commons.Util {
    public class AttributeUtil {

        private static readonly IDictionary<Type, ISet<Type>> AttributesCache = new Dictionary<Type, ISet<Type>>();

        public static ISet<Type> FindTypesAnnotattedWith(IEnumerable<Assembly> assembliesToSearch, params Type[] typesToSearch) {
            var resulTypes = new HashSet<Type>();
            IList<Type> typesNotYetCached = new List<Type>();
            foreach (var typeToSearch in typesToSearch) {
                if (!AttributesCache.ContainsKey(typeToSearch)) {
                    typesNotYetCached.Add(typeToSearch);
                    AttributesCache[typeToSearch] = new HashSet<Type>();
                } else {
                    resulTypes.AddAll(AttributesCache[typeToSearch]);
                }
            }
            if (!typesNotYetCached.Any()) {
                return resulTypes;
            }

            var swAssemblies = assembliesToSearch;
            foreach (var swAssembly in swAssemblies) {
                foreach (var type in swAssembly.GetTypes()) {
                    foreach (var typeToSearch in typesNotYetCached) {
                        if (type.GetCustomAttributes(typeToSearch, false).Length > 0) {
                            AttributesCache[typeToSearch].Add(type);
                            resulTypes.Add(type);
                        }
                    }
                }
            }
            return resulTypes;
        }

    }
}
