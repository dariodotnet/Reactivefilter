namespace ReactiveFilter
{
    using Services;
    using Splat;

    public class AppBootstrap
    {
        public AppBootstrap()
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => new ElementsService(), typeof(IElementsService));
        }
    }
}