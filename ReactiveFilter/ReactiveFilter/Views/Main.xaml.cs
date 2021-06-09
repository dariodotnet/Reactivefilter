namespace ReactiveFilter
{
    using ReactiveUI;
    using System;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using Xamarin.Forms;

    public partial class Main
    {
        public Main()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
        }

        protected override void SetBindings()
        {
            this.Events().Appearing
                .Where(x => !ViewModel.IsLoaded)
                .Throttle(TimeSpan.FromMilliseconds(3000), RxApp.MainThreadScheduler)
                .Select(x => Unit.Default)
                .InvokeCommand(ViewModel.LoadData).DisposeWith(Disposables);

            this.OneWayBind(ViewModel, vm => vm.Elements, v => v.Elements.ItemsSource);

            this.WhenAnyValue(v => v.ViewModel.Loading, v => v.ViewModel.IsLoaded)
                .Select(x => x.Item1 || !x.Item2)
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, v => v.ActivityIndicator.IsVisible).DisposeWith(Disposables);

            ExpanderActivator.Events().Clicked
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(x =>
                {
                    Expander.IsExpanded = !Expander.IsExpanded;
                    ExpanderActivator.Rotation = Expander.IsExpanded ? -90 : 90;
                })
                .Subscribe();

            //TODO navigate on selected element
            //TestClick.Events().Clicked
            //    .Do(async x =>
            //    {
            //        var viewModel = new DetailViewModel();
            //        var detail = new Detail
            //        {
            //            ViewModel = viewModel
            //        };
            //        await Navigation.PushAsync(detail);
            //    }).Subscribe();
        }
    }
}