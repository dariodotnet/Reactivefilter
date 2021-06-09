namespace ReactiveFilter
{
    using Models;
    using ReactiveUI;
    using ReactiveUI.Fody.Helpers;
    using Services;
    using Splat;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    public class ElementViewModel : ReactiveObject, IDisposable
    {
        private CompositeDisposable Disposables;
        private readonly IElementsService _elementsService;

        [Reactive] public string Id { get; set; }
        [Reactive] public Mobile Mobile { get; set; }
        [Reactive] public string Model { get; set; }
        [Reactive] public string Description { get; set; }
        [Reactive] public string Brand { get; set; }
        [Reactive] public DateTime TimeStamp { get; set; }
        [Reactive] public string OperativeSystem { get; set; }
        [Reactive] public int UserValue { get; set; }
        [Reactive] public List<int> UsersValue { get; set; }
        [Reactive] public double UsersValueAverage { get; set; }
        [Reactive] public string Color { get; set; }
        [Reactive] public double Cost { get; set; }
        [Reactive] public TimeSpan DeliveryTime { get; set; }

        public ReactiveCommand<Unit, Unit> SendEvaluation { get; }

        public ElementViewModel(IElementsService elementsService = null)
        {
            Disposables = new CompositeDisposable();

            _elementsService = elementsService ?? Locator.Current.GetService<IElementsService>();

            var canEvaluate = this.WhenAny(x => x.UserValue, v => v.Value > 0 && v.Value < 6);
            SendEvaluation = ReactiveCommand.Create(() => _elementsService.EvaluateElement(Id, UserValue), canEvaluate);

            this.WhenAnyValue(x => x.Mobile)
                .WhereNotNull()
                .Do(mobile =>
                {
                    Model = mobile.Model;
                    Brand = mobile.Brand;
                    OperativeSystem = mobile.OperativeSystem;
                    Cost = mobile.Cost;
                }).Subscribe();

            this.WhenAnyValue(x => x.UsersValue)
                .WhereNotNull()
                .Where(x => x.Any())
                .Do(values => UsersValueAverage = values.Average())
                .Subscribe();
        }

        public void Dispose()
        {
            Disposables?.Dispose();
        }
    }
}