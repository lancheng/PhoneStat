using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.Telephony;
using Android.Util;

namespace PhoneStats
{
	public class ShutdownReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			PhoneStatLog.GetInstance().LogPhone(DateTime.Now.ToString(), "9");
		}
	}

	public class SMSReceiver : BroadcastReceiver
	{
		public static readonly string IntentAction = "android.provider.Telephony.SMS_RECEIVED";

		public override void OnReceive(Context context, Intent intent)
		{
			try
			{
				if (intent.Action != IntentAction) return;
				PhoneStatLog.GetInstance().LogPhone(DateTime.Now.ToString(), "4");
			}
			catch (Exception ex)
			{
				Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
			}
		}

	}

	interface PhoneCallState
	{
		PhoneCallState Ring();
		PhoneCallState Offhook();
		PhoneCallState Idle();
	};

	class PhoneCallRing : PhoneCallState
	{
		private static PhoneCallRing ringState = null;
		private PhoneCallRing() { }

		static public PhoneCallState GetInstance()
		{
			if (ringState == null)
			{
				ringState = new PhoneCallRing();
			}

			return ringState;
		}

		public PhoneCallState Ring()
		{
			return PhoneCallRing.GetInstance();
		}

		public PhoneCallState Offhook()
		{
			PhoneStatLog.GetInstance().LogPhone(DateTime.Now.ToString(), "2");
			//Log.Debug("X:" + typeof(PhoneCallReceiver).Name, "1: Incoming call answered.");
			return PhoneCallOffhook.GetInstance();
		}

		public PhoneCallState Idle()
		{
			PhoneStatLog.GetInstance().LogPhone(DateTime.Now.ToString(), "3");
			//Log.Debug("X:" + typeof(PhoneCallReceiver).Name, "2: Incoming call ended.");
			return PhoneCallIdle.GetInstance();
		}
	}

	class PhoneCallOffhook: PhoneCallState
	{
		private static PhoneCallOffhook offhookState = null;
		private PhoneCallOffhook() { }

		static public PhoneCallState GetInstance()
		{
			if (offhookState == null)
			{
				offhookState = new PhoneCallOffhook();
			}

			return offhookState;
		}

		public PhoneCallState Ring()
		{
			Log.Debug("X:" + typeof(PhoneCallReceiver).Name, "Exception");
			return PhoneCallRing.GetInstance();
		}

		public PhoneCallState Offhook()
		{
			return PhoneCallOffhook.GetInstance();
		}

		public PhoneCallState Idle()
		{
			PhoneStatLog.GetInstance().LogPhone(DateTime.Now.ToString(), "3");
			return PhoneCallIdle.GetInstance();
		}
	}

	class PhoneCallIdle : PhoneCallState
	{
		private static PhoneCallIdle idleState = null;
		private PhoneCallIdle() { }

		static public PhoneCallState GetInstance()
		{
			if (idleState == null)
			{
				idleState = new PhoneCallIdle();
			}

			return idleState;
		}

		public PhoneCallState Ring()
		{
			PhoneStatLog.GetInstance().LogPhone(DateTime.Now.ToString(), "1");
			return PhoneCallRing.GetInstance();
		}

		public PhoneCallState Offhook()
		{
			PhoneStatLog.GetInstance().LogPhone(DateTime.Now.ToString(), "0");
			return PhoneCallOffhook.GetInstance();
		}

		public PhoneCallState Idle()
		{
			return PhoneCallIdle.GetInstance();
		}
	}

	public class PhoneCallReceiver : BroadcastReceiver
	{
		private PhoneCallState currentState = PhoneCallIdle.GetInstance();

		public override void OnReceive(Context context, Intent intent)
		{
			if (intent.Extras != null)
			{
				string state = intent.GetStringExtra(TelephonyManager.ExtraState);
				if (state == TelephonyManager.ExtraStateRinging)
				{
					currentState = currentState.Ring();
				}
				else if (state == TelephonyManager.ExtraStateOffhook)
				{
					currentState = currentState.Offhook();
				}
				else if (state == TelephonyManager.ExtraStateIdle)
				{
					currentState = currentState.Idle();
				}
			}
		}
	}

	/*[BroadcastReceiver(Label = "SMS Sender")]
	[IntentFilter(new string[] { "android.provider.Telephony.SMS_SENT" })]
	public class SMSSender : Android.Content.BroadcastReceiver
	{
		public static readonly string IntentAction = "android.provider.Telephony.SMS_SENT";

		public override void OnReceive(Context context, Intent intent)
		{
			try
			{
				if (intent.Action != IntentAction) return;
				String log_text = String.Format("{0}: 4", DateTime.Now);
				Toast.MakeText(context, log_text, ToastLength.Long).Show();

			}
			catch (Exception ex)
			{
				Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
			}
		}

	}*/
}
