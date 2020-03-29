using NUnit.Framework;
using System.Collections;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using System.Buffers;
using Moq;

namespace TextToColor.Tests
{
    public class TextToColor
    {
        const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        [Test, TestCaseSource(nameof(ReturnsSameColorForSameInputMD5TestCases))]
        public Color ReturnsSameColorForSameInputMD5(string text)
        {
            return text.ToColor(c => c.WithMD5HashProvider());
        }

        [Test, TestCaseSource(nameof(ReturnsSameColorForSameInputSHA256TestCases))]
        public Color ReturnsSameColorForSameInputSHA256(string text)
        {
            return text.ToColor(c => c.WithSHA256HashProvider());
        }

        [Test]
        public void HandleHugeText()
        {
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                var data = new byte[1024 * 1024 * 10];
                rngCsp.GetBytes(data);

                var text = Encoding.UTF8.GetString(data);
                text.Invoking(t => t.ToColor())
                    .Should().NotThrow();
            }
        }

        [Test]
        public void RandonValuesTest()
        {
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < 10_000; i++)
                {
                    var data = ArrayPool<byte>.Shared.Rent(1024 * 10);
                    rngCsp.GetBytes(data);
                    var text = Encoding.UTF8.GetString(data);

                    text.Invoking(t => t.ToColor(c => c.WithMD5HashProvider()))
                        .Should().NotThrow();

                    text.Invoking(t => t.ToColor(c => c.WithSHA256HashProvider()))
                        .Should().NotThrow();

                    ArrayPool<byte>.Shared.Return(data);
                }
            }
        }

        [Test]
        public void RespectSaturationValuesFromConfiguration()
        {
            var text = "Hello world";

            var colorMD5 = text.ToColor(c => c.WithMD5HashProvider()); 
            var colorMD5WithCustomOptions = text.ToColor(c => c
                .WithMD5HashProvider()
                .WithPossibleSaturationValues(new[] { 0.1f, 1f } )
                .WithPossibleLightnessValues(new[] { 0.1f, 1f }));

            colorMD5.Should().NotBeSameAs(colorMD5WithCustomOptions);

            var colorSHA256 = text.ToColor(c => c.WithSHA256HashProvider());
            var colorSHA256WithCustomOptions = text.ToColor(c => c
                .WithSHA256HashProvider()
                .WithPossibleSaturationValues(new[] { 0.1f, 1f })
                .WithPossibleLightnessValues(new[] { 0.1f, 1f }));

            colorSHA256.Should().NotBeSameAs(colorSHA256WithCustomOptions);
        }

        [Test]
        public void CustomHashProvider()
        {
            var text = "Hello world";

            var hashProvider = new Mock<IHashProvider>();
            hashProvider.Setup(s => s.Hash(It.IsAny<string>()))
                .Returns(157894);

            var color= text.ToColor(c => c
                .WithHashProvider(hashProvider.Object));

            color.Should().Be(Color.FromArgb(190, 135, 197));
        }

        [Test]
        public void CustomAlphaValue()
        {
            var text = "Hello world";

            var color = text.ToColor(c => c.WithAlpha(0.1f));

            color.A.Should().Be(25);
        }

        public static IEnumerable ReturnsSameColorForSameInputMD5TestCases
        {
            get
            {
                yield return new TestCaseData(null).Returns(Color.FromArgb(255, 120, 58, 58));
                yield return new TestCaseData("").Returns(Color.FromArgb(255, 120, 58, 58));
                yield return new TestCaseData(" ").Returns(Color.FromArgb(255, 89, 45, 134));
                yield return new TestCaseData("A").Returns(Color.FromArgb(255, 121, 191, 210));
                yield return new TestCaseData("B").Returns(Color.FromArgb(255, 137, 224, 108));
                yield return new TestCaseData("C").Returns(Color.FromArgb(255, 210, 121, 145));
                yield return new TestCaseData("D").Returns(Color.FromArgb(255, 197, 135, 156));
                yield return new TestCaseData("E").Returns(Color.FromArgb(255, 83, 172, 104));
                yield return new TestCaseData("AAAAAAA").Returns(Color.FromArgb(255, 147, 31, 70));
                yield return new TestCaseData("AbABACA").Returns(Color.FromArgb(255, 120, 134, 45));
                yield return new TestCaseData("wqj  porjioqwjrdsnjnjd fasd f").Returns(Color.FromArgb(255, 192, 135, 197));
                yield return new TestCaseData(LoremIpsum).Returns(Color.FromArgb(255, 49, 31, 147));
            }
        }

        public static IEnumerable ReturnsSameColorForSameInputSHA256TestCases
        {
            get
            {
                yield return new TestCaseData(null).Returns(Color.FromArgb(255, 120, 58, 58));
                yield return new TestCaseData("").Returns(Color.FromArgb(255, 120, 58, 58));
                yield return new TestCaseData(" ").Returns(Color.FromArgb(255, 210, 72, 45));
                yield return new TestCaseData("A").Returns(Color.FromArgb(255, 89, 191, 64));
                yield return new TestCaseData("B").Returns(Color.FromArgb(255, 197, 156, 135));
                yield return new TestCaseData("C").Returns(Color.FromArgb(255, 64, 191, 181));
                yield return new TestCaseData("D").Returns(Color.FromArgb(255, 134, 45, 51));
                yield return new TestCaseData("E").Returns(Color.FromArgb(255, 45, 210, 94));
                yield return new TestCaseData("AAAAAAA").Returns(Color.FromArgb(255, 159, 64, 191));
                yield return new TestCaseData("AbABACA").Returns(Color.FromArgb(255, 77, 191, 64));
                yield return new TestCaseData("wqj  porjioqwjrdsnjnjd fasd f").Returns(Color.FromArgb(255, 210, 45, 180));
                yield return new TestCaseData(LoremIpsum).Returns(Color.FromArgb(255, 99, 31, 147));
            }
        }
    }
}