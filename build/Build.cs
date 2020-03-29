using GlobExpressions;
using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using System.Linq;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Package);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution.GetProject("TextToColor.Tests"))
                .SetConfiguration(Configuration)
                .SetLogger("trx")
                .SetResultsDirectory(ArtifactsDirectory / "TestResults")
                .EnableNoBuild()
                .EnableNoRestore());
        });

    Target Package => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .EnableIncludeSymbols()
                .EnableIncludeSource()
                .SetOutputDirectory(ArtifactsDirectory));
        });

    Target PublishAzureDevOpsArtifacts => _ => _
        .TriggeredBy(Test)
        .OnlyWhenStatic(() => AzurePipelines.Instance != null)
        .Executes(() =>
        {
            var testFiles = Glob.Files(ArtifactsDirectory / "TestResults", "*.trx")
                .Select(f => (string)(ArtifactsDirectory / "TestResults" / f));

            AzurePipelines.Instance.PublishTestResults("TextToColor unit tests",
                AzurePipelinesTestResultsType.VSTest,
                testFiles,
                mergeResults: true,
                configuration: Configuration);
            AzurePipelines.Instance.UploadArtifacts("Container", "drop", ArtifactsDirectory);
        });
}
