using journals.commons.SimpleInjector;
using SimpleInjector;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Journals.Client.Util {
    public class ClientSimpleInjectorScanner : SimpleInjectorScanner {

        public static Container InitDIController() {

            var before = Stopwatch.StartNew();
            // Create the container as usual.
            var container = new Container();
            container.Options.AllowOverridingRegistrations = true;
            

            RegisterComponents(container);
            // Register the dependency resolver.
            // Verify the container configuration
            container.Verify();

            before.Stop();
            Log.DebugFormat("SimpleInjector context initialized in {0}", before.ElapsedMilliseconds);
            return container;
        }

    }
}