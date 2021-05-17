using System.Linq;
using CSETWebCore.Authorization;
using CSETWebCore.Business.ACETDashboard;
using CSETWebCore.Business.AdminTab;
using CSETWebCore.Business.Assessment;
using CSETWebCore.Business.Common;
using CSETWebCore.Business.Contact;
using CSETWebCore.Business.Demographic;
using CSETWebCore.Business.Diagram;
using CSETWebCore.Business.Document;
using CSETWebCore.Business.Maturity;
using CSETWebCore.Business.Notification;
using CSETWebCore.Business.Question;
using CSETWebCore.Business.Sal;
using CSETWebCore.Business.Standards;
using CSETWebCore.Business.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CSETWebCore.Helpers;
using CSETWebCore.Interfaces.AdminTab;
using CSETWebCore.Interfaces.Assessment;
using CSETWebCore.Interfaces.Common;
using CSETWebCore.Interfaces.Contact;
using CSETWebCore.Interfaces.Helpers;
using CSETWebCore.Interfaces.Maturity;
using CSETWebCore.Interfaces.Question;
using CSETWebCore.Interfaces.Sal;
using CSETWebCore.Interfaces.Standards;
using CSETWebCore.DataLayer;
using CSETWebCore.Interfaces;
using CSETWebCore.Interfaces.ACETDashboard;
using CSETWebCore.Interfaces.Demographic;
using CSETWebCore.Interfaces.Document;
using CSETWebCore.Interfaces.Notification;
using CSETWebCore.Interfaces.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CSETWeb_ApiCore
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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });
            });
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddAuthorization();
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddDbContext<CSETContext>(
                options => options.UseSqlServer("name=ConnectionStrings:CSET_DB"));

            //Services
            services.AddTransient<IAdminTabBusiness, AdminTabBusiness>();
            services.AddTransient<IAssessmentBusiness, AssessmentBusiness>();
            services.AddTransient<IAssessmentModeData, AssessmentModeData>();
            services.AddTransient<IAssessmentUtil, AssessmentUtil>();
            services.AddTransient<IContactBusiness, ContactBusiness>();
            services.AddTransient<IDemographicBusiness, DemographicBusiness>();
            services.AddTransient<IDiagramManager, DiagramManager>();
            services.AddTransient<IDocumentBusiness, DocumentBusiness>();
            services.AddTransient<IHtmlFromXamlConverter, HtmlFromXamlConverter>();
            services.AddTransient<IMaturityBusiness, MaturityBusiness>();
            services.AddTransient<INotificationBusiness, NotificationBusiness>();
            services.AddTransient<IParameterContainer, ParameterContainer>();
            services.AddTransient<IPasswordHash, PasswordHash>();
            services.AddTransient<IQuestionBusiness, QuestionBusiness>();
            services.AddTransient<IQuestionPoco, QuestionPoco>();
            services.AddTransient<IQuestionRequirementManager, QuestionRequirementManager>();
            services.AddTransient<IRequirementBusiness, RequirementBusiness>();
            services.AddTransient<IResourceHelper, ResourceHelper>();
            services.AddTransient<ISalBusiness, SalBusiness>();
            services.AddTransient<IStandardsBusiness, StandardsBusiness>();
            services.AddTransient<IStandardSpecficLevelRepository, StandardSpecficLevelRepository>();
            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<IUserAuthentication, UserAuthentication>();
            services.AddTransient<IUserBusiness, UserBusiness>();
            services.AddTransient<IUtilities, Utilities>();
            services.AddTransient<ITrendDataProcessor, TrendDataProcessor>();
            services.AddTransient<IACETDashboardBusiness, ACETDashboardBusiness>();
           

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CSETWeb_ApiCore", Version = "v1" });
                c.ResolveConflictingActions(apiDescription => apiDescription.First());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CSETWeb_ApiCore v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAll");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
