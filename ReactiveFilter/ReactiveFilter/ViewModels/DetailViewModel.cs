namespace ReactiveFilter
{
    using ReactiveUI.Fody.Helpers;

    public class DetailViewModel : BaseViewModel
    {
        [Reactive] public ElementViewModel ElementViewModel { get; set; }

        public DetailViewModel()
        {

        }
    }
}