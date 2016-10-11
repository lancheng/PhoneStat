using System;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace PhoneStats
{
	[Service]
	public class CollectService: Service
	{
		static readonly string TAG = "X:" + typeof(CollectService).Name;
		SMSReceiver smsReceiver = new SMSReceiver();
		PhoneCallReceiver phoneCallReceiver = new PhoneCallReceiver();

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Debug(TAG, "OnStartCommand called at {2}, flags={0}, startid={1}", flags, startId, DateTime.UtcNow);

			RegisterReceiver(smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
			RegisterReceiver(phoneCallReceiver, new IntentFilter("android.intent.action.PHONE_STATE"));

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();

			Log.Debug(TAG, "SimpleService destroyed at {0}.", DateTime.UtcNow);
			UnregisterReceiver(smsReceiver);
			UnregisterReceiver(phoneCallReceiver);

		}

		public override IBinder OnBind(Intent intent)
		{
			// This example isn't of a bound service, so we just return NULL.
			return null;
		}

	}
}
