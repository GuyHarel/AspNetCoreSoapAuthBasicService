using TestProject.SoapServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AspNetCoreSoapAuthBasicService.Services;

namespace TestProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly ILogger<ServiceController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWcfClientService wcfClientService;

        public ServiceController(
            ILogger<ServiceController> logger,
            IConfiguration configuration,
            IWcfClientService wcfClientService)
        {
            _logger = logger;
            _configuration = configuration;
            this.wcfClientService = wcfClientService;
        }

        [HttpGet]
        [Route("/Service")]
        //[Authorize]
        public ActionResult Get()
        {
            var chaineHeure = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var hostName = _configuration.GetSection("Service:HostName").Value;

            hostName += "/BasicAuthDemoSoapService.asmx";
            var soapUser = Environment.GetEnvironmentVariable("SoapUser");
            var soapPassword = Environment.GetEnvironmentVariable("SoapPassword");

            string htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>AspNetCoreSoapAuthBasicService</title>
                </head>
                <body>
                    <ul>
                        <li><a href={hostName}>{hostName}</a></li>
                        <li>Heure: {chaineHeure} </li>
                        <li>SoapUser: {soapUser} </li>
                        <li>SoapPassword: {soapPassword} </li>
                    </ul>
                </body>
                </html>";

            return new ContentResult() { Content = htmlContent, ContentType= "text/html" };
        }

        [HttpPost]
        [Route("/Service/Asmx")]
        //[Authorize]
        public ActionResult GetAsmx()
        {
            return RedirectPermanent("https://localhost:7248/BasicAuthDemoSoapService.asmx");

        }

        [HttpPost]
        [Route("/Service/Authbasic")]
        public ActionResult GetAuthbasic()
        {
            string htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>AspNetCoreSoapAuthBasicService</title>
                </head>
                <body>
                   <p>public ActionResult GetAuthbasic()</p>
                </body>
                </html>";

            return new ContentResult() { Content = htmlContent, ContentType = "text/html" };

        }

        [HttpPost]
        [Route("/BasicAuthDemoSoapService.asmx")]
        //[Authorize]
        public ActionResult PostSoap()
        {
            return RedirectPermanent("https://localhost:7248/BasicAuthDemoSoapService.asmx");

        }

        [HttpGet]
        [Route("/Service/RecupererFichier")]
        public ActionResult RecupererFichier()
        {
            wcfClientService.RecupererFichier2();

            string htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>RecupererFichier</title>
                </head>
                <body>
                   <p>RecupererFichier</p>
                </body>
                </html>";

            return new ContentResult() { Content = htmlContent, ContentType = "text/html" };

        }
    }
}
