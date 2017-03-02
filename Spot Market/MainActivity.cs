using Android.App;
using Android.Widget;
using Android.OS;

namespace Spot_Market
{
    [Activity(Label = "Spot_Market", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        
        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            Button login = FindViewById<Button>(Resource.Id.button1); //Declare login button

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            login.Click += delegate {
                StartActivity(typeof(MainMenu)); //Load second activity
            };
        }
    }
}

