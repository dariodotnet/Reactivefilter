namespace ReactiveFilter.Services
{
    using DynamicData;
    using System.Threading.Tasks;

    public interface IElementsService
    {
        SourceList<ElementViewModel> Elements { get; }
        Task StartService();
        void StopService();
        void EvaluateElement(string id, int userValue);

        double Max();
        double Min();
    }
}