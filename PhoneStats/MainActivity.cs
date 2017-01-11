using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;
using System.Linq;

namespace PhoneStats
{
	[Activity(Label = "Phone Stats", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		Button startButton;
		Button stopButton;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			startButton = FindViewById<Button>(Resource.Id.startButton);

			startButton.Click += delegate { 
				StartService(new Intent(this, typeof(CollectService)));
				HockeyApp.MetricsManager.TrackEvent("Click button to start to collect data");
				Toast.MakeText(this, startButton.Text, ToastLength.Short).Show(); 

				SetUIState();
			};

			stopButton = FindViewById<Button>(Resource.Id.stopButton);

			stopButton.Click += delegate
			{
				StopService(new Intent(this, typeof(CollectService)));
				HockeyApp.MetricsManager.TrackEvent("Click button to stop collecting");
				Toast.MakeText(this, stopButton.Text, ToastLength.Short).Show();

				SetUIState();
			};

			SetUIState();

			CrashManager.Register(this, "9a8084607e66447184a693b8c8775734");
			MetricsManager.Register(Application, "9a8084607e66447184a693b8c8775734");

		}

		private void SetUIState()
		{
			bool serviceRunning = IsServiceRunning();
			startButton.Enabled = !serviceRunning;
			stopButton.Enabled = serviceRunning;

		}

		private bool IsServiceRunning()
		{
			ActivityManager activityManager = (ActivityManager)GetSystemService(ActivityService);
            var serviceInstance = activityManager.GetRunningServices(int.MaxValue).ToList().FirstOrDefault(service => service.Service.PackageName.Contains("com.trafficindex.phone_stats") && service.Service.ShortClassName.Contains(".CollectService"));
			return serviceInstance != null;
		}



		protected override void OnStop()
		{
			base.OnStop();
			// Clean up: shut down the service when the Activity is no longer visible.
			// StopService(new Intent(this, typeof(CollectService)));
		}
	}
}
