namespace ReactiveFilter.Views
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Detail
    {
        public Detail()
        {
            InitializeComponent();

            this.Events().Disappearing
                .Do(x => Dispose())
                .Subscribe().DisposeWith(Disposables);
        }
    }
}