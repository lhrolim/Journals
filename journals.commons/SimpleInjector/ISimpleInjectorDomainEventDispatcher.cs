namespace journals.commons.SimpleInjector {
    public interface ISimpleInjectorDomainEventDispatcher : ISingletonComponent{
        void Dispatch<T>(T eventToDispatch, bool parallel = false) where T : class;
        void DispatchAsync<T>(T eventToDispatch, bool parallel = false) where T : class;
    }
}