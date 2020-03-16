using Api.Utils;
using Logic.AppServices;
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

            var connectionString = new CommandsConnectionString(Configuration["ConnectionString"]);
            services.AddSingleton(connectionString);
            var quriesConnectionString = new QueriesConnectionString(Configuration["QueriesConnectionString"]);
            services.AddSingleton(quriesConnectionString);

            services.AddSingleton<SessionFactory>();
            services.AddTransient<UnitOfWork>();
            services.AddTransient<ICommandHandler<EditPersonalInfoCommand>>(provider =>
            new AuditLogDecorator<EditPersonalInfoCommand>(
            new DatabaseRetryDecorator<EditPersonalInfoCommand>
                (new EditPersonalInfoCommand.EditPersonalInfoCommandHandler(provider.GetService<SessionFactory>()))
            ));
            services.AddTransient<ICommandHandler<RegisterStudentCommand>, RegisterStudentCommand.RegisterStudentCommandHandler>();
            services.AddTransient<ICommandHandler<UnRegisterStudentCommand>, UnRegisterStudentCommand.UnRegisterStudentCommandHandler>();
            services.AddTransient<ICommandHandler<EnrollStudentCommand>, EnrollStudentCommand.EnrollStudentCommandHandler>();
            services.AddTransient<ICommandHandler<DisEnrollStudentCommand>, DisEnrollStudentCommand.DisEnrollStudentCommandHandler>();
            services.AddTransient<ICommandHandler<TransferStudentCommand>, TransferStudentCommand.TransferStudentCommandHandler>();
            services.AddTransient<IQueryHandler<GetStudentsQuery, List<StudentDto>>, GetStudentsQuery.GetStudentsQueryHandler>();
            services.AddSingleton<Messages>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandler>();
            app.UseMvc();
        }
    }
}
