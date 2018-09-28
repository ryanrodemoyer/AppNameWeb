using AppName.Web.GraphQL;
using AppName.Web.Managers;
using AppName.Web.Models;
using AppName.Web.Repositories;
using AppName.Web.Services;
using AppName.Web.Transport;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppName.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Common/AccessDenied";
                    options.LoginPath = "/account/login";
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("GraphQL", policy =>
                    policy.Requirements.Add(new NoOpRequirement()));
            });

            services.AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddSingleton<AppSchemaInternal>();
            services.AddSingleton<Query>();
            services.AddSingleton<Mutation>();
            services.AddSingleton<ISchema, AppSchema>();
            services.AddSingleton<IData, AirportRepository>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IAuthorizationHandler, NoOpHandler>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ISignInManager, SignInManager>();

            services.AddGraphQL(_ =>
                {
                    _.EnableMetrics = true;
                    _.ExposeExceptions = true;
                });
            //.AddUserContextBuilder(httpContext => new GraphQLUserContext { User = httpContext.User });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // When the app runs in the Development environment:
                //   Use the Developer Exception Page to report app runtime errors.
                //   Use the Database Error Page to report database runtime errors.
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // When the app doesn't run in the Development environment:
                //   Enable the Exception Handler Middleware to catch exceptions
                //     thrown in the following middlewares.
                //   Use the HTTP Strict Transport Security Protocol (HSTS)
                //     Middleware.
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Use HTTPS Redirection Middleware to redirect HTTP requests to HTTPS.
            app.UseHttpsRedirection();

            // Return static files and end the pipeline.
            app.UseStaticFiles();

            // Use Cookie Policy Middleware to conform to EU General Data 
            // Protection Regulation (GDPR) regulations.
            app.UseCookiePolicy();

            // Authenticate before the user accesses secure resources.
            app.UseAuthentication();

            //// If the app uses session state, call Session Middleware after Cookie 
            //// Policy Middleware and before MVC Middleware.
            //app.UseSession();

            app.UseMvc(routes =>
                        {
                            routes.MapRoute(
                                name: "default",
                                template: "{controller=Home}/{action=Index}/{id?}");
                        });

            // require an authenticated user for anything else in the pipeline after this point
            // this verifies that the requester is authenticated and satisfies the policy named GraphQL
            // for now, this protects the /graphql and /ui/playground routes
            app.Use(async (context, next) =>
            {
                if (context.User?.Identity?.IsAuthenticated == false)
                {
                    string redirect = "/account/login";
                    if (context.Request.Path == "/ui/playground")
                    {
                        redirect += "?ReturnUrl=%2Fui%2Fplayground";
                    }

                    context.Response.Redirect(redirect, false);
                }
                else
                {
                    var authService = context.RequestServices.GetRequiredService<IAuthorizationService>();

                    AuthorizationResult result = await authService.AuthorizeAsync(context.User, context, "GraphQL");
                    if (result.Succeeded)
                    {
                        await next();
                    }
                    else
                    {
                        string redirect = "/account/login";
                        if (context.Request.Path == "/ui/playground")
                        {
                            redirect += "?ReturnUrl=%2Fui%2Fplayground";
                        }

                        context.Response.Redirect(redirect, false);

                        //throw new InvalidOperationException("Policy is missing.");
                    }
                }
            });

            // add http for Schema at default url /graphql
            app.UseGraphQL<ISchema>("/graphql");

            // use graphql-playground at default url /ui/playground
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions
            {
                Path = "/ui/playground"
            });
        }
    }
}
