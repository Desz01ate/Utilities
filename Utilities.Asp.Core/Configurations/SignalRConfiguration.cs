using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Utilities.Asp.Core.Configurations
{
    /// <summary>
    /// Due to technical issues, configuration for SignalR is not available at the moment. Please follow the instruction below.
    /// </summary>
    public static class SignalRConfiguration
    {
        private static void ConfigureServices(IServiceCollection services)
        {
            //services.AddSignalR();
            //-- if you inject any service into the hub, please add in above the hubs config.
            //-- ex.
            //---- services.AddSingleton<YourService>();
            //services.AddScoped<YourHub1>();
            //...
            //services.AddScoped<YourHubN>();
        }
        private static void Configure(IApplicationBuilder app)
        {
            /*
             app.UseEndpoints(endpoints => {
                --optional, for all-in-one app MVC
                --- endpoints.MapRazorPages();

                endpoints.MapHub<YourHub1>();
                ...
                endpoints.MapHub<YourHubN>();

                --optional
                ---- endpoints.MapControllers();
             });
             */
        }
    }
}
