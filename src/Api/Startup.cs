using Api.Utils;
using Logic.Decorators;
using Logic.Dtos;
using Logic.Students;
using Logic.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton(new SessionFactory(Configuration["ConnectionString"]));
            services.AddTransient<UnitOfWork>();
            services.AddTransient<ICommandHandler<EditPersonalInfoCommand>>(provider =>
            new AuditLogDecorator<EditPersonalInfoCommand>(
            new DatabaseRetryDecorator<EditPersonalInfoCommand>
                (new EditPersonalInfoCommandHandler(provider.GetService<SessionFactory>()))
            ));
            services.AddTransient<ICommandHandler<RegisterStudentCommand>, RegisterStudentCommandHandler>();
            services.AddTransient<ICommandHandler<UnRegisterStudentCommand>, UnRegisterStudentCommandHandler>();
            services.AddTransient<ICommandHandler<EnrollStudentCommand>, EnrollStudentCommandHandler>();
            services.AddTransient<ICommandHandler<DisEnrollStudentCommand>, DisEnrollStudentCommandHandler>();
            services.AddTransient<ICommandHandler<TransferStudentCommand>, TransferStudentCommandHandler>();
            services.AddTransient<IQueryHandler<GetStudentsQuery, List<StudentDto>>, GetStudentsQueryHandler>();
            services.AddSingleton<Messages>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandler>();
            app.UseMvc();
        }
    }
}
