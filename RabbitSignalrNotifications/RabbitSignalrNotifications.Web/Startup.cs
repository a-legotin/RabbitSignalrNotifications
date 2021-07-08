using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitSignalrNotifications.Shared;
using RabbitSignalrNotifications.Web.Notifications;
using RabbitSignalrNotifications.Web.Repositories;

namespace RabbitSignalrNotifications.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "RabbitSignalrNotifications.Web", Version = "v1"});
            });
            services.AddSignalR();
            services.AddSingleton<IWeatherForecastRepo, WeatherForecastRepo>();
            services.AddSingleton<IConnectionsRepo, ConnectionsRepo>();

            var publisher = new RabbitMqPublisher(RabbitMqOptions.RabbitExchange, RabbitConnectionFactory.GetDefault());
            var subscriber = new RabbitMqSubscriber<WeatherForecast>("web",
                RabbitMqOptions.RabbitExchange,
                RabbitMqRouting.ServiceToWeb,
                RabbitConnectionFactory.GetDefault());
            services.AddSingleton<NotificationHubContext>();
            subscriber.SetMessageHandler(forecast => { return Task.Run(() => true); });
            subscriber.StartConsuming();
            services.AddSingleton<IWeatherForecastNotifier>(new WeatherForecastNotificationService(publisher));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(
                    c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "RabbitSignalrNotifications.Web v1"); });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/api/notifications");
            });
        }
    }
}