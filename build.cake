#addin Cake.Npm

using System.Diagnostics;

var target = Argument("target", task_help);

var env = new CakeEnvironment(new CakePlatform(), new CakeRuntime(), null);
var root = Directory(env.WorkingDirectory + "\\");

var task_buildDotNet = "build-dotnet";
var task_buildWeb = "build-web";
var task_cleanSql = "clean-sql";
var task_full = "full";
var task_genClient = "gen-client";
var task_genPolygon = "gen-polygon";
var task_help = "help";
var task_runAll = "run-all";
var task_runApi = "run-api";
var task_runWeb = "run-web";
var task_testApi = "test-api";
var task_testApiContract = "test-api-contract";
var task_testApiIntegration = "test-api-integration";
var task_testApiUnit = "test-api-unit";

Task(task_buildDotNet)
    .Does(() =>
    {
        var path = root + File(@"Djambi.sln");

        var settings = new DotNetCoreBuildSettings
        {
            Configuration = "Debug"
        };

        DotNetCoreBuild(path, settings);
    });

Task(task_buildWeb)
    .Does(() =>
    {
        var dir = root + Directory("web\\");

        NpmInstall(new NpmInstallSettings
        {
            WorkingDirectory = dir
        });

        NpmRunScript(new NpmRunScriptSettings
        {
            ScriptName = "build",
            WorkingDirectory = dir
        });
    });

Task(task_cleanSql)
    .IsDependentOn(task_buildDotNet)
    .Does(() =>
    {
        var path = root + File(@"utils\db-reset\db-reset.fsproj");
        DotNetCoreRun(path);
    });

Task(task_full)
    .IsDependentOn(task_buildDotNet)
    .IsDependentOn(task_genClient)
    .IsDependentOn(task_buildWeb)
    .IsDependentOn(task_cleanSql)
    .IsDependentOn(task_runAll)
    .IsDependentOn(task_testApi);

Task(task_genClient)
    .IsDependentOn(task_buildDotNet)
    .Does(() =>
    {
        var path = root + File(@"utils\client-generator\client-generator.fsproj");
        DotNetCoreRun(path);
    });

Task(task_genPolygon)
    .Does(() =>
    {
        var path = root + File(@"utils\polygon-data-generator\polygon-data-generator.fsproj");
        DotNetCoreRun(path);
    });

Task(task_help)
    .Does(() =>
    {
        Information("TODO: Add help info here");
    });

Task(task_runAll)
    .IsDependentOn(task_runApi)
    .IsDependentOn(task_runWeb);

Task(task_runApi)
    .Does(() =>
    {
        var info = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run api.host.fsproj",
            WorkingDirectory = (root + File("api\\api.host\\")).ToString()
        };

        Process.Start(info);
    });

Task(task_runWeb)
    .Does(() =>
    {
        var info = new ProcessStartInfo
        {
            FileName = "http-server",
            WorkingDirectory = (root + Directory("web\\")).ToString()
        };

        Process.Start(info);
    });

Task(task_testApiUnit)
    .Does(() =>
    {
        var path = root + File("api\\tests\\api.unitTests\\api.unitTests.fsproj");
        var settings = new DotNetCoreTestSettings
        {

        };

        DotNetCoreTest(path, settings);
    });

Task(task_testApiIntegration)
    .Does(() =>
    {
        var path = root + File("api\\tests\\api.integrationTests\\api.integrationTests.fsproj");
        var settings = new DotNetCoreTestSettings
        {

        };

        DotNetCoreTest(path, settings);
    });

Task(task_testApiContract)
    .Does(() =>
    {
        var path = root + File("api\\tests\\api.contractTests\\api.contractTests.fsproj");
        var settings = new DotNetCoreTestSettings
        {

        };

        DotNetCoreTest(path, settings);
    });

Task(task_testApi)
    .IsDependentOn(task_testApiUnit)
    .IsDependentOn(task_testApiIntegration)
    .IsDependentOn(task_testApiContract);

RunTarget(target);