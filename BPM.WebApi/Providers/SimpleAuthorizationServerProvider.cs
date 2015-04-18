using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using BPM.Maps.Implementations;
using BPM.Maps.Interfaces;
using BPM.Repositories.DataContext;
using BPM.Repositories.Implementations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using BPM.DependencyInjection;
using System.Web.Cors;

namespace WebApi.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {


        public SimpleAuthorizationServerProvider()
         {
            
         }
       
         public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
         {

             context.Validated();
             return Task.FromResult<object>(null);
         }



        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {


            context.OwinContext.Response.Headers.Add(CorsConstants.AccessControlAllowOrigin, new[] { "*" });
          //    context.OwinContext.Response.Headers.Add(CorsConstants.AccessControlAllowCredentials, new[] { "true" });


                var userMap = UnityConfig.ResolveDependency<UserMap>();
                var user = userMap.FindUser(context.UserName, context.Password);
                if (user == null)
                {
                    context.SetError("invalid_grant", "Usuario y/ó contraseña incorrectos.");
                    return;
                }
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                identity.AddClaim(new Claim("sub", context.UserName));
                identity.AddClaim(new Claim("role", "user"));

                var listPermission = string.Join("-", userMap.GetUserPermission(user.usuarioId));

                var props = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        { 
                            "UserName", user.Mail
                        },
                        {
                            "Rol",  (user.SisRols.FirstOrDefault() != null) ? user.SisRols.FirstOrDefault().Nombre:"nulo"
                        },
                        {
                            "PermissionList",listPermission
                        }
                    });

                var ticket = new AuthenticationTicket(identity, props);
                context.Validated(ticket);

        }


    }
}