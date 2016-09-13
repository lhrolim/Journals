using System;
using System.Web.Http;
using journals.commons.Model.Migration;
using Journals.Web.Api;
using Journals.Web.Api.Providers;
using Journals.Web.Api.SimpleInjectorUtil;
using log4net.Config;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SimpleInjector.Integration.WebApi;

[assembly: OwinStartup(typeof(Startup))]
namespace Journals.Web.Api {

    public class Startup {

        public void Configuration(IAppBuilder app) {
            XmlConfigurator.Configure();
            var container = WebSimpleInjectorScanner.InitDIController();
            var config = new HttpConfiguration() {
                DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container)
            };

            new MigratorExecutor().Migrate(runner => runner.MigrateUp());

            ConfigureOAuth(app);
            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
            

        }


        public void ConfigureOAuth(IAppBuilder app) {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions() {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }

    }
}