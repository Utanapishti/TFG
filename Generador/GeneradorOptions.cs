namespace Generador
{
    public class GeneradorOptions
    {
        public double[] Values { get; set; } = new double[] { 0 };
        public TimeSpan Interval {get;set;}=TimeSpan.FromSeconds(5);
        public string Name { get; set; } = String.Empty;
    }

}
