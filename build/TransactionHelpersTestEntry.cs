using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using NukeBuildHelpers;
using NukeBuildHelpers.Attributes;
using NukeBuildHelpers.Enums;
using NukeBuildHelpers.Models;
using NukeBuildHelpers.Models.RunContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _build;

public class TransactionHelpersTestEntry : AppTestEntry<Build>
{
    public override RunsOnType RunsOn => RunsOnType.Ubuntu2204;

    public override Type[] AppEntryTargets => [typeof(TransactionHelpersEntry)];

    public override void Run(AppTestRunContext appTestContext)
    {
        var projPath = RootDirectory / "TransactionHelpers.UnitTest" / "TransactionHelpers.UnitTest.csproj";

        DotNetTasks.DotNetClean(_ => _
            .SetProject(projPath));
        DotNetTasks.DotNetTest(_ => _
            .SetProjectFile(projPath));
    }
}
