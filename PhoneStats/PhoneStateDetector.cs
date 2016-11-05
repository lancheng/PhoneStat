using Android.App;
using Android.Content;
using Android.Telephony;
using Android.Locations;
using Android.Util;
using Android.OS;
using System;

namespace PhoneStats
{
	public class PhoneStateDetector : PhoneStateListener, ILocationListener
	{
		static readonly string TAG = "X:" + typeof(CollectService).Name;
		public override void OnCallStateChanged(CallState state, string incomingNumber)
		{
			if (state == CallState.Ringing)
			{
				Log.Debug(TAG, "Incommming call detected from " + incomingNumber);
				base.OnCallStateChanged(state, incomingNumber);
			}
		}

		public void OnLocationChanged(Location location) 
		{
			Log.Debug(TAG, "{0}, Lat: {1}, Lng: {2}", DateTime.Now.ToString(), location.Latitude, location.Longitude);
		}

		public void OnProviderDisabled(string provider) { }

		public void OnProviderEnabled(string provider) { }

		public void OnStatusChanged(string provider, Availability status, Bundle extras) { }


		public override void OnCellLocationChanged(CellLocation location)
		{
			Log.Debug(TAG, location.ToString());
			base.OnCellLocationChanged(location);
		}

		public override void OnCellInfoChanged(System.Collections.Generic.IList<CellInfo> cellInfo)
		{
			foreach (var info in cellInfo)
			{
				Log.Debug(TAG, info.ToString());
			}

			base.OnCellInfoChanged(cellInfo);
		}
	}
}
