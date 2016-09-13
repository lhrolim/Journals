using System;
using System.Collections.Generic;
using System.Linq;
using journals.commons.Util;
using log4net;
using SimpleInjector;

namespace journals.commons.SimpleInjector {
    public class SimpleInjectorScanner {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(SimpleInjectorScanner));
        private const string DynReplaceMsg = "Replacing component {0} with the dynamic component {1}";





        protected static void RegisterComponents(Container container) {

            var assemblies = AssemblyUtil.GetJournalAssemblies();
            IDictionary<Type, IList<Registration>> tempDict = new Dictionary<Type, IList<Registration>>();
            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes().Where(type => typeof(IComponent).IsAssignableFrom(type));
                foreach (var typeToRegister in types) {
                    if (typeToRegister.IsInterface || typeToRegister.IsAbstract) {
                        continue;
                    }

                    var name = SimpleInjectorGenericFactory.BuildRegisterName(typeToRegister);
                    var finalType = typeToRegister;

                    var reg = Lifestyle.Singleton.CreateRegistration(finalType, container);

                    RegisterFromInterfaces(typeToRegister, tempDict, reg);
                    RegisterClassItSelf(container, typeToRegister, reg, name);
                }
            }
            foreach (var entry in tempDict) {
                var coll = entry.Value;
                var serviceType = entry.Key;
                if (typeof(ISingletonComponent).IsAssignableFrom(serviceType)) {
                    container.AddRegistration(serviceType, coll.FirstOrDefault());
                    SimpleInjectorGenericFactory.RegisterNameAndType(serviceType);
                } else {
                    container.RegisterCollection(serviceType, coll);
                }
            }
        }

        private static void RegisterClassItSelf(Container container, Type registration, Registration reg, string name) {
            if (!registration.IsPublic && !registration.IsNestedPublic) {
                return;
            }

            if (SimpleInjectorGenericFactory.ContainsService(name)) {
                Log.DebugFormat("ignoring type {0} due to presence of existing overriding", registration.Name);
                return;
            }

            container.AddRegistration(registration, reg);
            SimpleInjectorGenericFactory.RegisterNameAndType(registration, name);
        }

        private static void RegisterFromInterfaces(Type registration, IDictionary<Type, IList<Registration>> tempDict, Registration reg) {
            foreach (var type in registration.GetInterfaces().Where(type => typeof(IComponent).IsAssignableFrom(type))) {
                if (!tempDict.ContainsKey(type)) {
                    tempDict.Add(type, new List<Registration>());
                }
                tempDict[type].Add(reg);
            }
        }

    }
}
