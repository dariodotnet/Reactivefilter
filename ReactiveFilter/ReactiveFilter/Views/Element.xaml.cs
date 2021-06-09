namespace ReactiveFilter.Views
{
    using ReactiveUI;
    using System;
    using System.Globalization;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Element
    {
        private CompositeDisposable Disposables;

        public Element()
        {
            InitializeComponent();

            Disposables = new CompositeDisposable();

            this.WhenAnyValue(v => v.ViewModel)
                .WhereNotNull()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(viewModel => SetBindings())
                .Subscribe().DisposeWith(Disposables);
        }

        private void SetBindings()
        {
            this.OneWayBind(ViewModel, vm => vm.Color, v => v.Color.BackgroundColor, GetColorFromModel).DisposeWith(Disposables);

            this.OneWayBind(ViewModel, vm => vm.Model, v => v.Model.Text).DisposeWith(Disposables);

            this.OneWayBind(ViewModel, vm => vm.UsersValueAverage, v => v.Rating.Text, r => Math.Round(r, 1).ToString(CultureInfo.InvariantCulture)).DisposeWith(Disposables);

            this.OneWayBind(ViewModel, vm => vm.DeliveryTime, v => v.DeliveryTime.Text,
                time => time.Days > 1 ? $"Delivery time: {time.Days} days" : $"Delivery time: {time.Days} day").DisposeWith(Disposables);

            this.OneWayBind(ViewModel, vm => vm.Cost, v => v.Cost.Text, cost => $"{cost}€").DisposeWith(Disposables);
        }

        private Color GetColorFromModel(string color)
        {
            switch (color)
            {
                case "Black":
                    return Xamarin.Forms.Color.Black;
                case "Blue":
                    return Xamarin.Forms.Color.Blue;
                case "Orange":
                    return Xamarin.Forms.Color.Orange;
                default:
                    return Xamarin.Forms.Color.Red;
            }
        }
    }
}