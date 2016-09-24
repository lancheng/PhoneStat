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
		static readonly int TimerWait = 4000;
		Timer _timer;
		SMSReceiver smsReceiver = new SMSReceiver();
		//SMSSender smsSender = new SMSSender();

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Debug(TAG, "OnStartCommand called at {2}, flags={0}, startid={1}", flags, startId, DateTime.UtcNow);
			_timer = new Timer(o => { Log.Debug(TAG, "Hello from SimpleService. {0}", DateTime.UtcNow); },
							   null,
							   0,
							   TimerWait);

			RegisterReceiver(smsReceiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
			//RegisterReceiver(smsSender, new IntentFilter("android.provider.Telephony.SMS_SENT"));

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();

			_timer.Dispose();
			_timer = null;

			Log.Debug(TAG, "SimpleService destroyed at {0}.", DateTime.UtcNow);
			UnregisterReceiver(smsReceiver);
			//UnregisterReceiver(smsSender);

		}

		public override IBinder OnBind(Intent intent)
		{
			// This example isn't of a bound service, so we just return NULL.
			return null;
		}

	}
}
