using AspNetCoreSoapAuthBasicService.Middlewares;
using AspNetCoreSoapAuthBasicService.SoapServices;
using Microsoft.AspNetCore.Authentication;
using SoapCore;
using SoapCore.Extensibility;
using AspNetCoreSoapAuthBasicService.Handlers;
using AspNetCoreSoapAuthBasicService.Transformers;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Logging.AzureAppServices;
using AspNetCoreSoapAuthBasicService.Azure;
using AspNetCoreSoapAuthBasicService;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;

// Add services to the container.

builder.Services.AddControllers(); // MVC, routage, controleur, mod�le, injection
builder.Services.AddSoapCore();

// Ajouter l'authentication basic (pour les route de controler avec  [Authorize], et pour le middleware soap ci bas
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddSingleton<IFaultExceptionTransformer, SoapCoreFaultExceptionTransformer>();
builder.Services.AddSingleton<IBasicAuthDemoSoapService, BasicAuthDemoSoapService>();
builder.Services.AddSingleton<ILoggingManager, LoggingManager>();

// Configurer le logging dans fichier text dans Azure
builder.Logging.AddAzureWebAppDiagnostics();
builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
    options.FileName = "logs-";
    options.FileSizeLimit = 50 * 1024;
    options.RetainedFileCountLimit = 5;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection(); // Renvoi automatique des HTTP vers HTTPS

// Inutile (mais recommend�)
app.UseAuthentication();  // Active le middle ware d'authentication
app.UseAuthorization();  // Active le middleware d'autorisation

app.MapControllers();  // configure pipeline routage, map des endpoints, routes

app.UseMiddleware<AspNetCoreSoapAuthBasicService.Middlewares.SoapRequestMiddleware>();  // Gestion des URL asmx

app.UseSoapEndpoint<IBasicAuthDemoSoapService>("/BasicAuthDemoSoapService.asmx", new SoapEncoderOptions()); // cr�ation du asmx par SoapCore


app.Run();


