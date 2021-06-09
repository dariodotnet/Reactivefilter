namespace ReactiveFilter
{
    using DynamicData;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using Services;
    using Splat;
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    public class MainViewModel : BaseViewModel
    {
        private readonly IElementsService _elementsService;

        private ReadOnlyObservableCollection<ElementViewModel> _elements;
        public ReadOnlyObservableCollection<ElementViewModel> Elements => _elements;

        [Reactive] public bool IsLoaded { get; set; }
        public bool Loading { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, Unit> LoadData { get; }

        public MainViewModel(IElementsService elementsService = null)
        {
            _elementsService = elementsService ?? Locator.Current.GetService<IElementsService>();

            LoadData = ReactiveCommand.CreateFromTask(async () =>
            {
                await LoadExecute();
            });
            LoadData.IsExecuting.ToPropertyEx(this, x => x.Loading);
            LoadData.Do(signal => IsLoaded = true).Subscribe();
            LoadData.ThrownExceptions.Subscribe(ex =>
            {
                //TODO handle exceptions
            });

            _elementsService.Elements.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _elements)
                .DisposeMany()
                .Subscribe();
        }

        private Task LoadExecute() => _elementsService.StartService();
    }
}