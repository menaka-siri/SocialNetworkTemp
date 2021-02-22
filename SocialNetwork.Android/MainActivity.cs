using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Xamarin.Auth;
using System;

namespace SocialNetwork.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var authenticator = new OAuth2Authenticator(
                "socialnetwork_implicit", "read", 
                new Uri("http://localhost:44335/connect/authorize"),
                new Uri("http://localhost:c")
                );

            authenticator.Completed += (sender, args) =>
            {

            };

            StartActivity(authenticator.GetUI(this));
        }
    }
}