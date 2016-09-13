using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace journals.commons.Util {
    public class AssemblyUtil {

        private static ReadOnlyCollection<Assembly> AllAssemblies;

        public static ReadOnlyCollection<Assembly> GetAssemblies() {
            if (AllAssemblies != null) {
                return AllAssemblies;
            }

            AllAssemblies = new ReadOnlyCollection<Assembly>(
              GetListOfAssemblies().ToList());

            return AllAssemblies;
        }

        private static IEnumerable<Assembly> GetListOfAssemblies() {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public static IEnumerable<Assembly> GetJournalAssemblies() {
            return GetAssemblies().Where(IsJournalAssembly);
        }

        public static bool IsJournalAssembly(Assembly assembly) {
            var fullName = assembly.FullName;
            return fullName.StartsWith("journals", StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
