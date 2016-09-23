using System;
using Android.App;
using Android.Content;
using Android.Widget;
//using Android.Telephony;

namespace PhoneStats
{
	[BroadcastReceiver(Label = "SMS Receiver")]
	[IntentFilter(new string[] { "android.provider.Telephony.SMS_RECEIVED" })]
	public class SMSReceiver : Android.Content.BroadcastReceiver
	{
		public static readonly string IntentAction = "android.provider.Telephony.SMS_RECEIVED";

		public override void OnReceive(Context context, Intent intent)
		{
			try
			{
				if (intent.Action != IntentAction) return;
				Toast.MakeText(context, String.Format("{0}: 3", DateTime.Now), ToastLength.Long).Show();

			}
			catch (Exception ex)
			{
				Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
			}
		}

	}
}
