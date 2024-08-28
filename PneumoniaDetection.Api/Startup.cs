using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PneumoniaDetection.Api.Commands;
using PneumoniaDetection.Api.Dtos;
using PneumoniaDetection.Api.Repository;
using PneumoniaDetection.Api.Worker;

namespace PneumoniaDetection.Api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddControllers();
            services.AddMediatR(typeof(UploadImageCommandHandler));
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PneumoniaDetection.Api", Version = "v1" });
            });

            services.Configure<ScoresToKeepOptions>(Configuration.GetSection(ScoresToKeepOptions.ScoresToKeep));

            services.AddSingleton<IBackgroundWorkerModel, BackgroundWorkerModel>();
            services.AddTransient<IModelConsumerRepository, ModelConsumerRepository>();
            services.AddTransient<ISaveFileRepository, SaveFileRepository>();
            services.AddTransient<IRemoveFileRepository, RemoveFileRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PneumoniaDetection.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
