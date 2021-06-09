namespace ReactiveFilter
{
    using Xamarin.Forms;

    public partial class App
    {
        public App()
        {
            InitializeComponent();

            var start = new AppBootstrap();

            MainPage = new NavigationPage(new Main());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}