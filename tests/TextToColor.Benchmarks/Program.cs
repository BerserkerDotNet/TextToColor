using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Buffers;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace TextToColor.Benchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<TextToColorBenchmarks>();
        }
    }

    [MemoryDiagnoser]
    public class TextToColorBenchmarks
    {
        private string _text;

        [Params(1, 10, 100, 1000)]
        public int TextLength { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                var data = new byte[TextLength];
                rngCsp.GetBytes(data);
                _text = Encoding.UTF8.GetString(data);
            }
        }

        [Benchmark]
        public Color MD5()
        {
            return _text.ToColor(c => c.WithMD5HashProvider());
        }

        [Benchmark]
        public Color SHA256()
        {
            return _text.ToColor(c => c.WithSHA256HashProvider());
        }
    }
}
