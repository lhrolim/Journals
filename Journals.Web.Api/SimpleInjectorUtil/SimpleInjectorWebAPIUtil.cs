using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using journals.commons.Util;
using SimpleInjector;

namespace Journals.Web.Api.SimpleInjectorUtil {
    public class SimpleInjectorWebApiUtil {

        public static void RegisterWebApiControllers(Container container) {
            foreach (var assembly in AssemblyUtil.GetAssemblies()) {
                var registrations = assembly.GetTypes().Where(type => typeof(ApiController).IsAssignableFrom(type));
                var dict = new Dictionary<string, Type>();
                foreach (var registration in registrations) {
                    if (registration.IsInterface || registration.IsAbstract) {
                        continue;
                    }
                    dict.Add(registration.Name.Replace("Controller", ""), registration);
                }
                container.RegisterSingleton<IAPIControllerFactory>(new WEbApiSimpleInjectorFactory(container, dict));
            }
        }

        private class WEbApiSimpleInjectorFactory : IAPIControllerFactory {
            private readonly Container _container;
            private readonly Dictionary<string, Type> _dictionary;

            public WEbApiSimpleInjectorFactory(Container container, Dictionary<string, Type> dictionary) {
                _container = container;
                _dictionary = dictionary;
            }

            public ApiController CreateNew(string name) {
                return (ApiController)_container.GetInstance(_dictionary[name]);
            }
        }
    }
}