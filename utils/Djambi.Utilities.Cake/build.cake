//#addin Cake.Service

var target = Argument("target", task_help);

var env = new CakeEnvironment(new CakePlatform(), new CakeRuntime(), null);
var root = Directory(System.IO.Path.GetFullPath(env.WorkingDirectory + @"..\..\..\"));
Information(root);

var task_buildDotNet = "build-dotnet";
var task_cleanSql = "clean-sql";
var task_help = "help";

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

Task(task_cleanSql)
    .IsDependentOn(task_buildDotNet)
    .Does(() => 
    {
        var path = root + File(@"utils\Djambi.Utilities.DatabaseReset\Djambi.Utilities.DatabaseReset.fsproj");
        DotNetCoreRun(path);
    });

Task(task_help)
    .Does(() => 
    {
        Information("TODO: Add help info here");
    });

RunTarget(target);
