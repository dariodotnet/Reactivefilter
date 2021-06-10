namespace ReactiveFilter
{
    using ReactiveUI;
    using System;
    using System.Linq;
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

            this.Bind(ViewModel, vm => vm.ModelFilter, v => v.SearchBar.Text);

            this.OneWayBind(ViewModel, vm => vm.Group, v => v.Elements.IsGrouped, g => g != Group.None);

            this.WhenAnyValue(v => v.ViewModel.Group)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(group =>
                {
                    if (group == Group.None)
                    {
                        this.OneWayBind(ViewModel, vm => vm.Elements, v => v.Elements.ItemsSource);
                    }
                    else
                    {
                        this.OneWayBind(ViewModel, vm => vm.ElementsGroup, v => v.Elements.ItemsSource);
                    }
                }).Subscribe();

            this.WhenAnyValue(v => v.ViewModel.Loading, v => v.ViewModel.IsLoaded)
                .Select(x => x.Item1 || !x.Item2)
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, v => v.ActivityIndicator.IsVisible).DisposeWith(Disposables);

            this.BindCommand(ViewModel, vm => vm.ChangeExpand, v => v.ExpanderActivator);

            this.OneWayBind(ViewModel, vm => vm.IsExpanded, v => v.Expander.IsExpanded);
            this.OneWayBind(ViewModel, vm => vm.IsExpanded, v => v.ExpanderActivator.Rotation, ex => ex ? -90 : 90);

            SortBindings();
            GroupBindings();
            CostBindings();
        }

        private void GroupBindings()
        {
            this.OneWayBind(ViewModel, vm => vm.Group, v => v.GroupNoneTitle.FontAttributes,
                group => group == Group.None ? FontAttributes.Bold : FontAttributes.None);

            this.OneWayBind(ViewModel, vm => vm.Group, v => v.GroupColorTitle.FontAttributes,
                group => group == Group.Color ? FontAttributes.Bold : FontAttributes.None);

            this.OneWayBind(ViewModel, vm => vm.Group, v => v.GroupBrandTitle.FontAttributes,
                group => group == Group.Brand ? FontAttributes.Bold : FontAttributes.None);

            this.OneWayBind(ViewModel, vm => vm.Group, v => v.GroupOperativeSystemTitle.FontAttributes,
                group => group == Group.OperativeSystem ? FontAttributes.Bold : FontAttributes.None);

            this.BindCommand(ViewModel, vm => vm.GroupElements, v => v.GroupNone, Observable.Return(Group.None));
            this.BindCommand(ViewModel, vm => vm.GroupElements, v => v.GroupColor, Observable.Return(Group.Color));
            this.BindCommand(ViewModel, vm => vm.GroupElements, v => v.GroupBrand, Observable.Return(Group.Brand));
            this.BindCommand(ViewModel, vm => vm.GroupElements, v => v.GroupOperativeSystem, Observable.Return(Group.OperativeSystem));
        }

        private void CostBindings()
        {
            this.OneWayBind(ViewModel, vm => vm.Min, v => v.RangeSlider.MinimumValue);
            this.Bind(ViewModel, vm => vm.MinSelected, v => v.RangeSlider.LowerValue);
            this.OneWayBind(ViewModel, vm => vm.Max, v => v.RangeSlider.MaximumValue);
            this.Bind(ViewModel, vm => vm.MaxSelected, v => v.RangeSlider.UpperValue);
        }

        private void SortBindings()
        {
            this.OneWayBind(ViewModel, vm => vm.Sorter, v => v.SorterNoneTitle.FontAttributes,
                sort => sort == Sorter.None ? FontAttributes.Bold : FontAttributes.None);

            this.OneWayBind(ViewModel, vm => vm.Sorter, v => v.SorterNameTitle.FontAttributes,
                sort => sort == Sorter.ModelName ? FontAttributes.Bold : FontAttributes.None);

            this.OneWayBind(ViewModel, vm => vm.Sorter, v => v.SorterDeliveryTitle.FontAttributes,
                sort => sort == Sorter.DeliveryTime ? FontAttributes.Bold : FontAttributes.None);

            this.OneWayBind(ViewModel, vm => vm.Sorter, v => v.SorterRatingTitle.FontAttributes,
                sort => sort == Sorter.Rating ? FontAttributes.Bold : FontAttributes.None);

            this.OneWayBind(ViewModel, vm => vm.Sorter, v => v.SorterColorTitle.FontAttributes,
                sort => sort == Sorter.Color ? FontAttributes.Bold : FontAttributes.None);

            this.Bind(ViewModel, vm => vm.Ascending, v => v.Ascending.IsToggled);

            this.BindCommand(ViewModel, vm => vm.Sort, v => v.SorterNone, Observable.Return(Sorter.None));
            this.BindCommand(ViewModel, vm => vm.Sort, v => v.SorterName, Observable.Return(Sorter.ModelName));
            this.BindCommand(ViewModel, vm => vm.Sort, v => v.SorterDelivery, Observable.Return(Sorter.DeliveryTime));
            this.BindCommand(ViewModel, vm => vm.Sort, v => v.SorterRating, Observable.Return(Sorter.Rating));
            this.BindCommand(ViewModel, vm => vm.Sort, v => v.SorterColor, Observable.Return(Sorter.Color));
        }
    }
}