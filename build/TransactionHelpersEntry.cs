using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using NukeBuildHelpers;
using NukeBuildHelpers.Attributes;
using NukeBuildHelpers.Enums;
using NukeBuildHelpers.Models.RunContext;
using Semver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _build;

public class TransactionHelpersEntry : AppEntry<Build>
{
    public override RunsOnType BuildRunsOn => RunsOnType.Ubuntu2204;

    public override RunsOnType PublishRunsOn => RunsOnType.Ubuntu2204;

    [SecretVariable("NUGET_AUTH_TOKEN")]
    readonly string NuGetAuthToken;

    [SecretVariable("GITHUB_TOKEN")]
    readonly string GithubToken;

    public override bool MainRelease => true;

    public override void Build(AppRunContext appRunContext)
    {
        if (appRunContext is AppBumpRunContext appBumpRunContext)
        {
            OutputDirectory.DeleteDirectory();
            DotNetTasks.DotNetClean(_ => _
                .SetProject(NukeBuild.Solution.TransactionHelpers));
            DotNetTasks.DotNetBuild(_ => _
                .SetProjectFile(NukeBuild.Solution.TransactionHelpers)
                .SetConfiguration("Release"));
            DotNetTasks.DotNetPack(_ => _
                .SetProject(NukeBuild.Solution.TransactionHelpers)
                .SetConfiguration("Release")
                .SetNoRestore(true)
                .SetNoBuild(true)
                .SetIncludeSymbols(true)
                .SetSymbolPackageFormat("snupkg")
                .SetVersion(appBumpRunContext.AppVersion.Version?.ToString() ?? "0.0.0")
                .SetPackageReleaseNotes(appBumpRunContext.AppVersion.ReleaseNotes)
                .SetOutputDirectory(OutputDirectory));
        }
    }

    public override void Publish(AppRunContext appRunContext)
    {
        if (appRunContext is AppBumpRunContext)
        {
            DotNetTasks.DotNetNuGetPush(_ => _
                .SetSource("https://nuget.pkg.github.com/kiryuumaru/index.json")
                .SetApiKey(GithubToken)
                .SetTargetPath(OutputDirectory / "**"));
            DotNetTasks.DotNetNuGetPush(_ => _
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetApiKey(NuGetAuthToken)
                .SetTargetPath(OutputDirectory / "**"));
        }
    }
}
