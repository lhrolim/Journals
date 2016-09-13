namespace journals.commons.SimpleInjector {
    public interface IEventListener<in T> : IComponent {
        void HandleEvent(T eventToDispatch);
    }
}