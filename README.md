# TextToColor
A set of extensions to convert text to System.Drawing.Color.

[![Build Status](https://berserkerdotnet.visualstudio.com/GitHubProjects/_apis/build/status/BerserkerDotNet.TextToColor?branchName=master)](https://berserkerdotnet.visualstudio.com/GitHubProjects/_build/latest?definitionId=14&branchName=master)

[![Nuget](https://buildstats.info/nuget/TextToColor?v=1.0.0)](https://www.nuget.org/packages/TextToColor)

### Usage
1. Install `TextToColor` package
1. Add a using statement ` using TextToColor`
1. Call `ToColor()` or `ToHsl()`extension method on any string

```csharp
using TextToColor;

...

var color = "Foo".ToColor();

or 

var hslaColor = "Foo".ToHsl();
```

Result: `"{Name=ffd27997, ARGB=(255, 210, 121, 151)}"` and `"HSLA = (340, 0.5, 0.65, 1)"`


### Configuration

TextToColor allows to configure every aspect of a color generation.

#### Set `alpha` value of resulting color:
```csharp
var color = "Foo".ToColor(c => c.WithAlpha(0.75f));
```
Result: `"{Name=bfd27997, ARGB=(191, 210, 121, 151)}"`

NOTE: Only integer part of the `alpha` value is beign considered. For if alpha is set to `0.756f`, resulting value will be `192`, because `255 * 0.756 = 192.78`.

#### Set possible saturation and/or ligtness values

```
var saturationValues = new[] { 0.1f, 0.2f, 0.3f, 0.4f };
var lightnessValues = new[] { 0.8f, 0.9f, 1.0f };
var color = "Foo".ToHsl(c => c
    .WithPossibleSaturationValues(saturationValues)
    .WithPossibleLightnessValues(lightnessValues));
```
Result: `HSLA = (340, 0.2, 1, 1)`

In this example resulting values of saturation can only be 10%, 20%, or 30% and resulting value of lightness can only be 80%, 90%, or 100%.

Default values are:

Saturation: `new[] { 0.35f, 0.5f, 0.65f }`

Lightness: `new[] { 0.35f, 0.5f, 0.65f }`

#### Hashing algorithm:

TextToColor comes with two hashing algorithms out of the box, MD5 and SHA256. Default is MD5 but this can be changed by providing following settings:

```csharp
var color = "Foo".ToColor(c => c.WithSHA256HashProvider());
```

Result: `"{Name=ff6ce089, ARGB=(255, 108, 224, 137)}"`

Custom algorithms supported by implementing `IHashProvider` interface.

```csharp
var color = "Foo".ToColor(c => c.WithHashProvider(new MyCoolHashProvider()));

public class MyCoolHashProvider : IHashProvider
{
    public ulong Hash(string text)
    {
        return 42;
    }
}
```

#### Combining multiple settings

```csharp
var color = "Foo".ToColor(c => c
    .WithSHA256HashProvider()
    .WithPossibleLightnessValues(lightnessValues)
    .WithPossibleSaturationValues(saturationValues)
    .WithAlpha(0.25f));
```

### Performance

``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-9700K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET Core SDK=3.1.101
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  DefaultJob : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT


```
| Method | TextLength |       Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------- |----------- |-----------:|----------:|----------:|-------:|------:|------:|----------:|
|    **MD5** |          **1** |   **826.5 ns** |   **2.78 ns** |   **2.32 ns** | **0.1106** |     **-** |     **-** |     **696 B** |
| SHA256 |          1 | 1,017.0 ns |   3.35 ns |   3.13 ns | 0.1221 |     - |     - |     776 B |
|    **MD5** |         **10** |   **833.6 ns** |  **11.00 ns** |  **10.29 ns** | **0.1106** |     **-** |     **-** |     **696 B** |
| SHA256 |         10 | 1,016.8 ns |  19.42 ns |  22.36 ns | 0.1221 |     - |     - |     776 B |
|    **MD5** |        **100** | **1,152.8 ns** |  **23.07 ns** |  **30.80 ns** | **0.1106** |     **-** |     **-** |     **696 B** |
| SHA256 |        100 | 1,635.0 ns |  37.34 ns |  38.35 ns | 0.1221 |     - |     - |     776 B |
|    **MD5** |       **1000** | **4,772.3 ns** |  **93.57 ns** | **128.08 ns** | **0.1068** |     **-** |     **-** |     **696 B** |
| SHA256 |       1000 | 8,814.3 ns | 172.48 ns | 177.12 ns | 0.1221 |     - |     - |     776 B |