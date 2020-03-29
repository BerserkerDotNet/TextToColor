using BenchmarkDotNet.Running;

namespace TextToColor.Benchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<TextToColorBenchmarks>();
        }
    }
}
