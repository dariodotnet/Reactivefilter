namespace ReactiveFilter
{
    using DynamicData;
    using DynamicData.Binding;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using Services;
    using Splat;
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    public class MainViewModel : BaseViewModel
    {
        private readonly IElementsService _elementsService;

        private ReadOnlyObservableCollection<ElementViewModel> _elements;
        public ReadOnlyObservableCollection<ElementViewModel> Elements => _elements;

        private ReadOnlyObservableCollection<ElementsGroup> _elementsGroup;
        public ReadOnlyObservableCollection<ElementsGroup> ElementsGroup => _elementsGroup;

        [Reactive] public bool IsLoaded { get; set; }
        public bool Loading { [ObservableAsProperty] get; }

        [Reactive] public bool IsExpanded { get; set; }
        [Reactive] public string ModelFilter { get; set; }
        [Reactive] public Sorter Sorter { get; set; }
        [Reactive] public bool Ascending { get; set; }
        [Reactive] public double Min { get; set; }
        [Reactive] public double MinSelected { get; set; }
        [Reactive] public double Max { get; set; }
        [Reactive] public double MaxSelected { get; set; }
        [Reactive] public Group Group { get; set; }

        public ReactiveCommand<Unit, Unit> LoadData { get; }
        public ReactiveCommand<Unit, Unit> ChangeExpand { get; }
        public ReactiveCommand<Sorter, Unit> Sort { get; }
        public ReactiveCommand<Group, Unit> GroupElements { get; }

        public MainViewModel(IElementsService elementsService = null)
        {
            _elementsService = elementsService ?? Locator.Current.GetService<IElementsService>();

            LoadData = ReactiveCommand.CreateFromTask(async () =>
            {
                await LoadExecute();
            });
            LoadData.IsExecuting.ToPropertyEx(this, x => x.Loading);
            LoadData
                .Do(signal => IsLoaded = true)
                .Do(x =>
                {
                    Min = _elementsService.Min();
                    MinSelected = Min;
                    Max = _elementsService.Max();
                    MaxSelected = Max;
                }).Subscribe();
            LoadData.ThrownExceptions.Subscribe(ex =>
            {
                //TODO handle exceptions
            });

            ChangeExpand = ReactiveCommand.Create(() => { IsExpanded = !IsExpanded; });

            Sort = ReactiveCommand.Create<Sorter, Unit>(sorter =>
            {
                Sorter = sorter;
                return Unit.Default;
            });
            Sort.InvokeCommand(ChangeExpand);

            GroupElements = ReactiveCommand.Create<Group, Unit>(group =>
            {
                Group = group;
                return Unit.Default;
            });
            GroupElements.InvokeCommand(ChangeExpand);

            var filter = this.WhenAnyValue(x => x.ModelFilter)
                .Select(BuildFilter);

            var sort = this.WhenAnyValue(x => x.Sorter, x => x.Ascending)
                .Select(sorter =>
                {
                    switch (sorter.Item1)
                    {
                        case Sorter.ModelName:
                            return sorter.Item2
                                ? SortExpressionComparer<ElementViewModel>.Ascending(x => x.Model)
                                : SortExpressionComparer<ElementViewModel>.Descending(x => x.Model);
                        case Sorter.DeliveryTime:
                            return sorter.Item2
                                ? SortExpressionComparer<ElementViewModel>.Ascending(x => x.DeliveryTime)
                                : SortExpressionComparer<ElementViewModel>.Descending(x => x.DeliveryTime);
                        case Sorter.Rating:
                            return sorter.Item2
                                ? SortExpressionComparer<ElementViewModel>.Ascending(x => x.UsersValueAverage)
                                : SortExpressionComparer<ElementViewModel>.Descending(x => x.UsersValueAverage);
                        case Sorter.Color:
                            return sorter.Item2
                                ? SortExpressionComparer<ElementViewModel>.Ascending(x => x.Color)
                                : SortExpressionComparer<ElementViewModel>.Descending(x => x.Color);
                        default:
                            return sorter.Item2
                                ? SortExpressionComparer<ElementViewModel>.Ascending(x => x.Id)
                                : SortExpressionComparer<ElementViewModel>.Descending(x => x.Id);
                    }
                });

            _elementsService.Elements.Connect()
                .Filter(filter)
                .Sort(sort)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _elements)
                .DisposeMany()
                .Subscribe();

            _elementsService.Elements.Connect()
                .Filter(filter)
                .Sort(sort)
                .GroupOn(arg =>
                {
                    switch (Group)
                    {
                        case Group.Color:
                            return arg.Color;
                        case Group.Brand:
                            return arg.Brand;
                        default:
                            return arg.OperativeSystem;
                    }
                })
                .Transform(grouping => new ElementsGroup(grouping))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _elementsGroup)
                .DisposeMany()
                .Subscribe();
        }

        private Task LoadExecute() => _elementsService.StartService();

        private Func<ElementViewModel, bool> BuildFilter(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
                return element => true;

            return element => element.Model.ToLower().Contains(model);
        }

        private Func<ElementViewModel, bool> BuildComplexFilter()
        {
            return element => true;
        }
    }

    public class ElementsGroup : ObservableCollectionExtended<ElementViewModel>, IDisposable
    {
        private CompositeDisposable Disposables;

        [Reactive] public string Header { get; set; }

        public ElementsGroup(IGroup<ElementViewModel, string> grouping)
        {
            Disposables = new CompositeDisposable();

            Header = grouping.GroupKey;

            grouping.List.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(this, 2000)
                .Subscribe().DisposeWith(Disposables);
        }
        public void Dispose()
        {
            Disposables?.Dispose();
        }
    }

    public enum Sorter
    {
        None, ModelName, DeliveryTime, Rating, Color
    }

    public enum Group
    {
        None, Color, Brand, OperativeSystem
    }
}