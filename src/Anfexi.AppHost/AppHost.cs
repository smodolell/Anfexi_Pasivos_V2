var builder = DistributedApplication.CreateBuilder(args);

var apiPasivos = builder.AddProject<Projects.Anfx_Pasivos_ApiService>("anfx-pasivos-apiservice");

var frontEnd = builder.AddJavaScriptApp("pasivo-frontend", "..\\Anfx.Pasivos.Frontend")
    .WithRunScript("start:aspire")
    .WithHttpEndpoint(targetPort: 4200, name: "http", isProxied: false) // Importante: solo targetPort
    .WithEnvironment("NODE_ENV", "development");


frontEnd.WithReference(apiPasivos);



builder.Build().Run();
