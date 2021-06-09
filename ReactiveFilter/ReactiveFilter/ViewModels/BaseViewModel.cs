namespace ReactiveFilter
{
    using ReactiveUI;
    using System;
    using System.Reactive.Disposables;

    public class BaseViewModel : ReactiveObject, IDisposable
    {
        protected CompositeDisposable Disposables;

        public BaseViewModel()
        {
            Disposables = new CompositeDisposable();
        }

        public void Dispose()
        {
            Disposables?.Dispose();
        }
    }
}