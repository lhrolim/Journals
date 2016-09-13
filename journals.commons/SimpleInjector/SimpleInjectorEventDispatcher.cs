using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;

namespace journals.commons.SimpleInjector {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using SimpleInjector;

    public class SimpleInjectorDomainEventDispatcher : ISimpleInjectorDomainEventDispatcher {
        private readonly Container _container;

        private readonly IDictionary<Type, object> _cachedHandlers = new Dictionary<Type, object>();

        private static readonly ILog Log = LogManager.GetLogger(typeof(SimpleInjectorDomainEventDispatcher));

        public SimpleInjectorDomainEventDispatcher(Container container) {
            _container = container;
        }

        private void RunEventHandler<T>(IEventListener<T> listener, T dispatchedEvent) {
            listener.HandleEvent(dispatchedEvent);
        }


        public void Dispatch<T>(T eventToDispatch, bool parallel = false) where T : class {
            var handlers = FindHandlers<T>(eventToDispatch);
            var eventName = eventToDispatch.GetType().Name;
            // sequential execution
            Log.DebugFormat("Running sequential iteration dispatch for event {0}", eventName);
            foreach (var item in handlers) {
                RunEventHandler(item, eventToDispatch);
            }
        }

        public void DispatchAsync<T>(T eventToDispatch, bool parallel = false) where T : class {
            Log.DebugFormat("Running Fire-and-forget dispatch for event {0}", eventToDispatch.GetType().Name);
            Task.Run(() => Dispatch(eventToDispatch, parallel)).ConfigureAwait(false);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private List<IEventListener<T>> FindHandlers<T>(object eventToDispatch) where T : class {
            if (_cachedHandlers.ContainsKey(eventToDispatch.GetType())) {
                return (List<IEventListener<T>>)_cachedHandlers[eventToDispatch.GetType()];
            }
            var handlers = _container.GetAllInstances<IEventListener<T>>();
            var swEventListeners = new List<IEventListener<T>>((IEnumerable<IEventListener<T>>)handlers);
            _cachedHandlers.Add(eventToDispatch.GetType(), swEventListeners);
            return swEventListeners;
        }
    }
}
