using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Clients;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //if(User.Identity.IsAuthenticated)
            //{
            //    var claimsPrincipal = User as ClaimsPrincipal;
            //    return Content(claimsPrincipal.FindFirst("access_token").Value);
            //}
            //var url = "http://localhost:44335/connect/authorize" +
            //          "?client_id=socialnetwork_code" +
            //          "&redirect_uri=http://localhost:57919/home/authorizationcallback" +
            //          "&response_type=code" +
            //          "&scope=openid+profile" +
            //          "&response_mode=form_post";

            //return Redirect(url);

            return View();
        }

        public ActionResult AuthorizationCallBack(string code, string state, string error)
        {
            var tokenUrl = "http://localhost:44335/connect/token";

            var client = new OAuth2Client(new Uri(tokenUrl), "socialnetwork_code", "secret");

            var requestResult = client.RequestAccessTokenCode(code,
                                new Uri("http://localhost:57919/home/authorizationcallback"));

            var claims = new[]
            {
                new Claim("access_token", requestResult.AccessToken)
            };

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            Request.GetOwinContext().Authentication.SignIn(identity);

            return Redirect("/");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public async Task<ActionResult> Private()
        {
            var claimsPrincipal = User as ClaimsPrincipal;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer",
                    claimsPrincipal.FindFirst("access_token").Value);
                var profile = await client.GetAsync("http://localhost:3468/api/profiles");
                var profileContent = await profile.Content.ReadAsStringAsync();
            }
            
            return View(claimsPrincipal.Claims); 
        }

        [Authorize]
        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }
    }
}