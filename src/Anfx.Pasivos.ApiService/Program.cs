using Anfx.Pasivos.ApiService;
using Anfx.Pasivos.ApiService.Infrastructure;
using Anfx.Pasivos.Application;
using Anfx.Pasivos.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;



// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);

builder.AddWebServices();
builder.Services.AddControllers();


builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["server"] = Environment.MachineName;
    };
});


var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? ""))
    };
});

builder.Services.AddAuthorization();

// Configuración de CORS
// Los orígenes adicionales de producción se pueden agregar vía appsettings.Production.json:
//   "AllowedOrigins": ["http://dev.anfexi.com", "https://dev.anfexi.com"]
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Environment.IsDevelopment()
        ? new[] { "http://localhost:4200" }
        : builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
          ?? ["http://dev.anfexi.com", "https://dev.anfexi.com"];

    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();

    //app.MapScalarApiReference(options =>
    //{
    //    options.WithTitle("Yggdrasil API Documentation");
    //    options.WithTheme(ScalarTheme.Saturn);
    //    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    //    options.HideSearch = true;// Habilita/Deshabilita el buscador (Ctrl+K)
    //    options.ShowSidebar = true; // Muestra u oculta la barra lateral
    //    options.DarkMode = true;
    //});
}

#if (!UseAspire)
app.UseHealthChecks("/health");
#endif
app.UseExceptionHandler(options => { });

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();

// Swagger y Scalar solo en desarrollo — no exponer en producción
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Pasivos V1");
    });
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("API Services Catalogos");
        options.WithTheme(ScalarTheme.DeepSpace);
        options.WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.AsyncHttp);
        options.HideSearch = true;
        options.ShowSidebar = true;
        options.DarkMode = false;
    });

    app.Map("/", () => Results.Redirect("/scalar"));
}


#if (UseAspire)
app.MapDefaultEndpoints();
#endif

app.MapEndpoints();

app.MapControllers();

app.Run();
