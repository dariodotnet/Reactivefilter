namespace ReactiveFilter.Services
{
    using DynamicData;
    using Models;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    public class ElementsService : IElementsService
    {
        private IDisposable _elementCreator;
        private Random _random = new Random();

        public SourceList<ElementViewModel> Elements { get; }

        public ElementsService()
        {
            Elements = new SourceList<ElementViewModel>();
        }

        public Task StartService()
        {
            //We can connect to this service and update the list in real time
            //_elementCreator = _elementCreatorDefinition;

            for (var i = 0; i < 1000; i++)
            {
                Elements.Add(GenerateElement());
            }

            return Task.CompletedTask;
        }

        public void StopService()
        {
            _elementCreator?.Dispose();
        }

        public void EvaluateElement(string id, int userValue)
        {
            var element = Elements.Items.FirstOrDefault(x => x.Id.Equals(id));
            Elements.Items.Where(x => x.Mobile != null && x.Mobile.Model.Equals(element.Model))
                .ToList()
                .ForEach(e => e.UsersValue.Add(userValue));
        }

        private IDisposable _elementCreatorDefinition =>
            Observable.Interval(TimeSpan.FromMilliseconds(500))
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Do(signal =>
                {
                    Elements.Add(GenerateElement());
                })
                .Subscribe();

        private ElementViewModel GenerateElement()
        {
            var element = new ElementViewModel
            {
                Id = Guid.NewGuid().ToString("D"),
                Mobile = MobileProperties.Mobiles[_random.Next(0, MobileProperties.Mobiles.Count - 1)],
                TimeStamp = DateTime.UtcNow.AddDays(_random.Next(-185, -1)).AddHours(_random.Next(0, 23)).AddMinutes(_random.Next(0, 58)).AddSeconds(_random.Next(0, 58)),
                Color = MobileProperties.Colors[_random.Next(0, MobileProperties.Colors.Count - 1)],
            };

            var elements = Elements.Items.Where(x => !x.Id.Equals(element.Id) && x.Description == element.Description);

            element.DeliveryTime = !elements.Any() ? TimeSpan.FromDays(_random.Next(1, 3)) : elements.FirstOrDefault().DeliveryTime;

            element.UsersValue = elements.Select(x => x.UserValue).ToList();

            return element;
        }
    }

    public static class MobileProperties
    {
        public static List<Mobile> Mobiles = new List<Mobile>
        {
            new Mobile{ Model = "iPhone 7", OperativeSystem = "iOS", Brand = "Apple", Cost = 250.00 },
            new Mobile{ Model = "iPhone 7 Plus", OperativeSystem = "iOS", Brand = "Apple", Cost = 325.00 },
            new Mobile{ Model = "iPhone 8", OperativeSystem = "iOS", Brand = "Apple", Cost = 275.50 },
            new Mobile{ Model = "iPhone 8 Plus", OperativeSystem = "iOS", Brand = "Apple", Cost = 375.75 },
            new Mobile{ Model = "iPhone X", OperativeSystem = "iOS", Brand = "Apple", Cost = 425.00 },
            new Mobile{ Model = "iPhone XS", OperativeSystem = "iOS", Brand = "Apple", Cost = 400.00 },
            new Mobile{ Model = "iPhone 11", OperativeSystem = "iOS", Brand = "Apple", Cost = 525.00 },
            new Mobile{ Model = "iPhone 11 Pro", OperativeSystem = "iOS", Brand = "Apple", Cost = 625.00 },
            new Mobile{ Model = "iPhone 12", OperativeSystem = "iOS", Brand = "Apple", Cost = 725.00 },
            new Mobile{ Model = "iPhone 12 Pro", OperativeSystem = "iOS", Brand = "Apple", Cost = 925.00 },
            new Mobile{ Model = "iPhone 12 Pro Max", OperativeSystem = "iOS", Brand = "Apple", Cost = 1025.50 },
            new Mobile{ Model = "Samsung SII", OperativeSystem = "Android", Brand = "Samsung", Cost = 125.00 },
            new Mobile{ Model = "Samsung SIII", OperativeSystem = "Android", Brand = "Samsung", Cost = 132.00 },
            new Mobile{ Model = "Samsung SIV", OperativeSystem = "Android", Brand = "Samsung", Cost = 145.00 },
            new Mobile{ Model = "Samsung SV", OperativeSystem = "Android", Brand = "Samsung", Cost = 175.00 },
            new Mobile{ Model = "Samsung SVI", OperativeSystem = "Android", Brand = "Samsung", Cost = 195.00 },
            new Mobile{ Model = "Samsung S7", OperativeSystem = "Android", Brand = "Samsung", Cost = 215.00 },
            new Mobile{ Model = "Samsung S8", OperativeSystem = "Android", Brand = "Samsung", Cost = 245.00 },
            new Mobile{ Model = "Samsung S9", OperativeSystem = "Android", Brand = "Samsung", Cost = 345.00 },
            new Mobile{ Model = "Samsung SX", OperativeSystem = "Android", Brand = "Samsung", Cost = 645.00 }
        };

        public static List<string> Colors = new List<string>
        {
            "Black",
            "White",
            "Blue",
            "Orange",
            "Red"
        };
    }
}