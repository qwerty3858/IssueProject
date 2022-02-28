using EmailService;
using Hangfire;
using IssueProject.Entity.Context;
using IssueProject.Hangfire;
using IssueProject.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Context;
using System;
using System.Globalization;
using System.Text;

namespace IssueProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            if (!Environment.IsDevelopment())
            {
                services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
                services.AddHangfireServer();
            }
        
            services.AddDbContext<_2Mes_ConceptualContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging(true);
                //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddHttpContextAccessor();
            services.AddSingleton(emailConfig);
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IEmailSender, EmailSender>();
            // services.AddTransient<IJob, JobManager>();
            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            services.AddScoped<DepartmentService>();
            services.AddScoped<RoleService>();
            services.AddScoped<IssueService>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IssueProject", Version = "v1" });

            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver(); //For PascalCase Fields
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz"; //
                /*options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";*/
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                options.SerializerSettings.Formatting = Formatting.None;
                options.SerializerSettings.FloatParseHandling = FloatParseHandling.Decimal;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Culture = CultureInfo.GetCultureInfo("tr-TR");
                options.SerializerSettings.Error = (sender, args) =>
                {
                    using (LogContext.PushProperty("CurrentObject", args.CurrentObject))
                        Log.Logger.Error(args.ErrorContext.Error, "{ErrorMessage}",
                            args.ErrorContext.Error.Message);
                };
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });/*.AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)*/;
            //services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true) // allow any origin
                    .AllowCredentials());
            });
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("Appsettings:Token").Value);

            services.ConfigureAuthentication();
          
            /*
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key), //Belirtecin imzasýný doðrula
                    ValidateIssuer = false,  //Belirteci oluþturan sunucuyu doðrulayýn.
                    ValidateAudience = false //Belirteci alýcýnýn almaya yetkili olduðunu doðrulayýn.
                };
            });
            */
            //        .AllowAnyOrigin()
            //        .WithOrigins("http://localhost:8080", "http://10.0.0.203:8080", "http://10.0.0.206:8080", "http://192.168.1.24:8080"));
            //});
            // services.AddHttpsRedirection(options =>
            //{
            //    options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
            //    options.HttpsPort = 5001;
            //});
            //app.UseCors(x => x
            //    .AllowAnyMethod()
            //    .AllowAnyHeader()
            //    .SetIsOriginAllowed(origin => true) // allow any origin
            //    .AllowCredentials()

            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseExceptionHandler("/Error");

                app.UseHangfireDashboard();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //  app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IssueProject v1"));
                      
            app.UseSerilogRequestLogging();

           // app.UseHttpsRedirection();


            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
            //    RequestPath = new PathString("/Resources")
            //});

            app.UseRouting();

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //  RecurringJob.AddOrUpdate("SendMailJob", () => JobMethod(), "* * * * *", TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"));


            if (!env.IsDevelopment())
                JobManager.Create(serviceProvider);

        }
    }
}
