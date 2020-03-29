using FluentAssertions;
using NUnit.Framework;
using System;

namespace TextToColor.Tests
{
    [TestFixture]
    public class TextToColorConfigurationTests
    {
        [Test]
        public void TestDefault()
        {
            var defaultValue = TextToColorConfiguration.Default;
            defaultValue.Alpha.Should().Be(1f);
            defaultValue.HashProvider.Should().BeOfType<MD5HashProvider>();
            defaultValue.SaturationPercentages.Should().BeEquivalentTo(new[] { 0.35f, 0.5f, 0.65f });
            defaultValue.LightnessPercentages.Should().BeEquivalentTo(new[] { 0.35f, 0.5f, 0.65f });
        }

        [Test]
        public void CanOverrideDefaultValues()
        {
            var lightnessValues = new[] { 0.2f, 0.9f, 0.7f, 0.5f };
            var saturationValues = new[] { 0.9f, 0.1f, 0.2f, 0.3f };
            var config = TextToColorConfiguration.Default
                .WithSHA256HashProvider()
                .WithAlpha(0.5f)
                .WithPossibleLightnessValues(lightnessValues)
                .WithPossibleSaturationValues(saturationValues);


            config.Alpha.Should().Be(0.5f);
            config.HashProvider.Should().BeOfType<SHA256HashProvider>();
            config.SaturationPercentages.Should().BeEquivalentTo(new[] { 0.9f, 0.1f, 0.2f, 0.3f });
            config.LightnessPercentages.Should().BeEquivalentTo(new[] { 0.2f, 0.9f, 0.7f, 0.5f });
        }

        [Test]
        public void ValidateValues()
        {
            TextToColorConfiguration.Default.Invoking(c => c.WithAlpha(-0.2f)).Should().Throw<ArgumentOutOfRangeException>();
            TextToColorConfiguration.Default.Invoking(c => c.WithAlpha(1.2f)).Should().Throw<ArgumentOutOfRangeException>();

            TextToColorConfiguration.Default.Invoking(c => c.WithPossibleSaturationValues()).Should().Throw<ArgumentOutOfRangeException>();
            TextToColorConfiguration.Default.Invoking(c => c.WithPossibleSaturationValues(null)).Should().Throw<ArgumentOutOfRangeException>();
            TextToColorConfiguration.Default.Invoking(c => c.WithPossibleSaturationValues(1.2f, 0.5f)).Should().Throw<ArgumentOutOfRangeException>();
            TextToColorConfiguration.Default.Invoking(c => c.WithPossibleSaturationValues(1f, -0.5f)).Should().Throw<ArgumentOutOfRangeException>();

            TextToColorConfiguration.Default.Invoking(c => c.WithPossibleLightnessValues()).Should().Throw<ArgumentOutOfRangeException>();
            TextToColorConfiguration.Default.Invoking(c => c.WithPossibleLightnessValues(null)).Should().Throw<ArgumentOutOfRangeException>();
            TextToColorConfiguration.Default.Invoking(c => c.WithPossibleLightnessValues(1.2f, 0.5f)).Should().Throw<ArgumentOutOfRangeException>();
            TextToColorConfiguration.Default.Invoking(c => c.WithPossibleLightnessValues(1f, -0.5f)).Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
