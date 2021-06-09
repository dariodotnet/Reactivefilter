namespace ReactiveFilter.Models
{
    public class Mobile
    {
        public string Model { get; set; }
        public string OperativeSystem { get; set; }
        public string Brand { get; set; }
        public double Cost { get; set; }

        public override string ToString()
        {
            return $"{Model} {OperativeSystem} {Brand} {Cost}";
        }
    }
}