using System;
using System.Threading;

using Android.App;
using Android.Widget;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Net;
using Android.Telephony;
using Android.Locations;
using System.Collections.Generic;
using System.Linq;

namespace PhoneStats
{
	[Service]
	public class CollectService: Service
	{
		static readonly string TAG = "X:" + typeof(CollectService).Name;
		string _locationProvider;
		SMSReceiver smsReceiver = new SMSReceiver();
		PhoneCallReceiver phoneCallReceiver = new PhoneCallReceiver();
		NetworkStatReceiver networkStatReceiver = new NetworkStatReceiver();
		PhoneStateDetector phoneStateDecetor;

		LocationManager locationManager;


		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Debug(TAG, "OnStartCommand called at {2}, flags={0}, startid={1}", flags, startId, DateTime.UtcNow);

			RegisterReceiver(smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
			RegisterReceiver(phoneCallReceiver, new IntentFilter("android.intent.action.PHONE_STATE"));
			RegisterReceiver(networkStatReceiver, new IntentFilter(ConnectivityManager.ConnectivityAction));

			phoneStateDecetor = new PhoneStateDetector();

			var tm = (TelephonyManager)base.GetSystemService(TelephonyService);
			tm.Listen(phoneStateDecetor, PhoneStateListenerFlags.CellInfo);
			tm.Listen(phoneStateDecetor, PhoneStateListenerFlags.CellLocation);
			Log.Debug(TAG, tm.NetworkOperatorName);
			Log.Debug(TAG, tm.NetworkType.ToString());

			PhoneGPSLog.GetInstance(tm.DeviceId);

			locationManager = (LocationManager)base.GetSystemService(LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};

			IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				_locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				_locationProvider = string.Empty;
			}

			Toast.MakeText(this, _locationProvider, ToastLength.Short).Show();

			locationManager.RequestLocationUpdates(_locationProvider, 0,0,phoneStateDecetor);

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();

			Log.Debug(TAG, "SimpleService destroyed at {0}.", DateTime.UtcNow);
			UnregisterReceiver(smsReceiver);
			UnregisterReceiver(phoneCallReceiver);
			UnregisterReceiver(networkStatReceiver);

			var tm = (TelephonyManager)base.GetSystemService(TelephonyService);
			tm.Listen(phoneStateDecetor, PhoneStateListenerFlags.None);

			PhoneGPSLog.GetInstance(tm.DeviceId).Close();

			locationManager.RemoveUpdates(phoneStateDecetor);
		}

		public override IBinder OnBind(Intent intent)
		{
			// This example isn't of a bound service, so we just return NULL.
			return null;
		}

	}
}
