using System.Web.Http;
using WebActivatorEx;
using TelegramService;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace TelegramService
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration 
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "TelegramService");
                    })
                .EnableSwaggerUi(c =>
                    {
                    });
        }
    }
}
