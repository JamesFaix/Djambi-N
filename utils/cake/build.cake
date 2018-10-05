#addin Cake.Npm

using System.Diagnostics;

var target = Argument("target", task_help);

var env = new CakeEnvironment(new CakePlatform(), new CakeRuntime(), null);
var root = Directory(System.IO.Path.GetFullPath(env.WorkingDirectory + @"..\..\..\"));

var task_buildDotNet = "build-dotnet";
var task_buildWeb = "build-web";
var task_cleanSql = "clean-sql";
var task_full = "full";
var task_help = "help";
var task_runAll = "run-all";
var task_runApi = "run-api";
var task_runWeb = "run-web";


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
    .IsDependentOn(task_buildWeb)
    .IsDependentOn(task_cleanSql)
    .IsDependentOn(task_runAll);

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
            Arguments = "run api.fsproj",
            WorkingDirectory = (root + File("api\\src\\")).ToString()
        };

        Process.Start(info);
    });

Task(task_runWeb)
    .Does(() => 
    {
        var info = new ProcessStartInfo
        {
            FileName = "http-server",
            WorkingDirectory = (root + File("web\\dist")).ToString()
        };

        Process.Start(info);
    });

RunTarget(target);
