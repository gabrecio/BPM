using BPM.Maps.Interfaces;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using WebApi.Providers;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using BPM.DependencyInjection;
using Unity.WebApi;
using System.Web.Optimization;

[assembly: OwinStartup(typeof(WebApi.Startup))]
namespace WebApi
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public void Configuration(IAppBuilder app)
        {

         

            var config = new HttpConfiguration();

            ConfigureOAuth(app);
           
            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
           
            UnityContainer container = UnityConfig.RegisterComponents();
            //GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            config.DependencyResolver = new UnityDependencyResolver(container);
            app.UseWebApi(config);
          //  GlobalConfiguration.Configure(WebApiConfig.Register);
          
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            //use a cookie to temporarily store information about a user logging in with a third party login provider
           // app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

           
            var OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),

                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new SimpleAuthorizationServerProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

        }

    }
}