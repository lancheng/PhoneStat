using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Telephony;
using Android.Locations;
using Android.Widget;

namespace PhoneStats
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[] { Intent.ActionBootCompleted })]
	public class StartupReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			Toast.MakeText(context, intent.ToString(), ToastLength.Long);

			Intent serviceStart = new Intent(context, typeof(CollectService));
			context.StartService(serviceStart);

			PhoneStatLog.GetInstance().LogPhone(DateTime.Now.ToString(), "8");
		}
	}

	[Service]
	public class CollectService: Service
	{
		static readonly string TAG = "X:" + typeof(CollectService).Name;
		SMSReceiver smsReceiver = new SMSReceiver();
		PhoneCallReceiver phoneCallReceiver = new PhoneCallReceiver();
		ShutdownReceiver shutdownReceiver = new ShutdownReceiver();
		PhoneStateDetector phoneStateDecetor;

		GPSLocationLogger mGPSLocLogger;
		NetworkLocationLogger mNetworkLocaLogger;

		LocationManager locationManager;


        public override void OnCreate()
        {
            base.OnCreate();
            
            phoneStateDecetor = new PhoneStateDetector(this);
            mGPSLocLogger = new GPSLocationLogger();
            mNetworkLocaLogger = new NetworkLocationLogger();
        }

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Debug(TAG, "OnStartCommand called at {2}, flags={0}, startid={1}", flags, startId, DateTime.UtcNow);

			RegisterReceiver(smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
			RegisterReceiver(phoneCallReceiver, new IntentFilter("android.intent.action.PHONE_STATE"));
			RegisterReceiver(shutdownReceiver, new IntentFilter("android.intent.action.ACTION_SHUTDOWN"));

			var tm = (TelephonyManager)base.GetSystemService(TelephonyService);
            tm.Listen(phoneStateDecetor, PhoneStateListenerFlags.DataActivity | PhoneStateListenerFlags.CellLocation);

			PhoneStatLog.GetInstance().DeviceID = tm.DeviceId;

			locationManager = (LocationManager)base.GetSystemService(LocationService);
			locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 0, 0, mGPSLocLogger);
			locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, 0, 0, mNetworkLocaLogger);

            RegisterForegroundService();
		
			return StartCommandResult.Sticky;
		}

        void RegisterForegroundService()
        {
            var notification = new Notification.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                //.SetContentText(Resources.GetString(Resource.String.notification_text))
                //.SetSmallIcon(Resource.Drawable.ic_stat_name)
                //.SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                //.AddAction(BuildRestartTimerAction())
                //.AddAction(BuildStopServiceAction())
                .Build();


            // Enlist this instance of the service as a foreground service
            StartForeground(10000, notification);
        }

		public override void OnDestroy()
		{
			base.OnDestroy();

			Log.Debug(TAG, "SimpleService destroyed at {0}.", DateTime.UtcNow);
			UnregisterReceiver(smsReceiver);
			UnregisterReceiver(phoneCallReceiver);

			var tm = (TelephonyManager)base.GetSystemService(TelephonyService);
			tm.Listen(phoneStateDecetor, PhoneStateListenerFlags.None);

			PhoneStatLog.GetInstance().Close();

			locationManager.RemoveUpdates(mGPSLocLogger);
			locationManager.RemoveUpdates(mNetworkLocaLogger);

            StopForeground(true);

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(10000);

		}

		public override IBinder OnBind(Intent intent)
		{
			// This example isn't of a bound service, so we just return NULL.
			return null;
		}

	}
}
