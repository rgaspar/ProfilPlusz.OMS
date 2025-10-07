
var builder = DistributedApplication.CreateBuilder(args);

/*var sql = builder
    .AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("sqldata");
*/
//var seq = builder.AddSeq("seq");

builder.AddProject("ASPNET", "../../Presentation/ASPNET/ASPNET.csproj")
    //.WithReference(db, "DefaultConnection")
    //.WithEnvironment("Serilog__WriteTo__1__Args__serverUrl", seq.GetEndpoint("http"))
    //.WaitFor(db)
    //.WaitFor(seq)
    ;

//builder.AddDashboard();

builder.Build().Run();
