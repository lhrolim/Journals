using System.Web.Http;
using journals.commons.SimpleInjector;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Journals.Web.Api.SimpleInjectorUtil {
    public class WebSimpleInjectorScanner : SimpleInjectorScanner {

        public static Container InitDIController() {

            var before = Stopwatch.StartNew();
            // Create the container as usual.
            var container = new Container();
            container.Options.AllowOverridingRegistrations = true;
            container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();

            RegisterComponents(container);
            SimpleInjectorWebApiUtil.RegisterWebApiControllers(container);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            // Register the dependency resolver.
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
            
            // Verify the container configuration
            container.Verify();

            before.Stop();
            Log.DebugFormat("SimpleInjector context initialized in {0}", before.ElapsedMilliseconds);
            return container;
        }

    }
}