using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;

namespace PhoneStats
{
	[Activity(Label = "Phone Stats", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button startButton = FindViewById<Button>(Resource.Id.startButton);

			startButton.Click += delegate { 
				StartService(new Intent(this, typeof(CollectService)));
				Toast.MakeText(this, startButton.Text, ToastLength.Short).Show(); 
			};

			Button stopButton = FindViewById<Button>(Resource.Id.stopButton);

			stopButton.Click += delegate
			{
				StopService(new Intent(this, typeof(CollectService)));
				Toast.MakeText(this, stopButton.Text, ToastLength.Short).Show();
			};
		}

		protected override void OnStop()
		{
			base.OnStop();
			// Clean up: shut down the service when the Activity is no longer visible.
			// StopService(new Intent(this, typeof(CollectService)));
		}
	}
}
