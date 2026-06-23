
var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sql-password", "Profilplusz!Dev2024", secret: true);

var sql = builder
    .AddSqlServer("sql", password: sqlPassword, port: 1433)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("profilplusz-sql-data");

var db = sql.AddDatabase("sqldata");

//var seq = builder.AddSeq("seq");

builder.AddProject("ASPNET", "../../Presentation/ASPNET/ASPNET.csproj")
    .WithReference(db, "DefaultConnection")
    //.WithEnvironment("Serilog__WriteTo__1__Args__serverUrl", seq.GetEndpoint("http"))
    .WaitFor(db)
    //.WaitFor(seq)
    ;

//builder.AddDashboard();

builder.Build().Run();
