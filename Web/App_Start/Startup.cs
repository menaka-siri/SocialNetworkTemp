using Microsoft.AspNet.Identity;
//using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Clients;

[assembly: OwinStartup(typeof(Web.App_Start.Startup))]

namespace Web.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            JwtSecurityTokenHandler.InboundClaimTypeMap
                = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "socialnetwork_code",
                Authority = "http://localhost:44335",
                RedirectUri = "http://localhost:57919/",
                ResponseType = "code id_token",
                Scope = "openid profile",
                SignInAsAuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                PostLogoutRedirectUri = "http://localhost:57919/",

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived =  async notification =>
                    {
                        var requestResponse = await OidcClient.CallTokenEndpointAsync(
                            new Uri("http://localhost:44335/connect/token"),
                            new Uri("http://localhost:57919/"),
                            notification.Code,
                            "socialnetwork_code",
                            "secret");

                        var identity = notification.AuthenticationTicket.Identity;

                        identity.AddClaim(new Claim("access_token", requestResponse.AccessToken));
                        identity.AddClaim(new Claim("id_token", requestResponse.IdentityToken));
                        identity.AddClaim(new Claim("refresh_token", requestResponse.RefreshToken));

                        notification.AuthenticationTicket = new AuthenticationTicket(
                            identity, notification.AuthenticationTicket.Properties);
                    },
                    RedirectToIdentityProvider = notification =>
                    {
                        if (notification.ProtocolMessage.RequestType != OpenIdConnectRequestType.LogoutRequest)
                        {
                            return Task.FromResult(0);
                        }

                        notification.ProtocolMessage.IdTokenHint =
                            notification.OwinContext.Authentication.User.FindFirst("id_token").Value;

                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}
