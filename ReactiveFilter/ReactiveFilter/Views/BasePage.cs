namespace ReactiveFilter
{
    using ReactiveUI;
    using ReactiveUI.XamForms;
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    public class BasePage<TViewModel> : ReactiveContentPage<TViewModel>, IDisposable where TViewModel : BaseViewModel
    {
        protected CompositeDisposable Disposables;

        public BasePage()
        {
            Disposables = new CompositeDisposable();

            this.WhenAnyValue(v => v.ViewModel)
                .WhereNotNull()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(vm => SetBindings())
                .Subscribe().DisposeWith(Disposables);
        }

        protected virtual void SetBindings()
        {

        }

        public void Dispose()
        {
            ViewModel?.Dispose();
            Disposables?.Dispose();
        }
    }
}