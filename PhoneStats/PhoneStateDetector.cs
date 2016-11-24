using Android.App;
using Android.Content;
using Android.Telephony;
using Android.Locations;
using Android.Util;
using Android.OS;
using System;

namespace PhoneStats
{
	public class PhoneStateDetector : PhoneStateListener
	{
		Context mContext;

		public PhoneStateDetector(Context context)
		{
			this.mContext = context;
		}

		public override void OnDataActivity(DataActivity direction)
		{
			if (direction == DataActivity.None || direction == DataActivity.Dormant)
			{
				PhoneStatLog.GetInstance().LogPhone(DateTime.Now.ToString(), "7");
			}
			else
			{
				PhoneStatLog.GetInstance().LogPhone(DateTime.Now.ToString(), "6");
			}
				
			base.OnDataActivity(direction);
		}

		public override void OnCellLocationChanged(CellLocation location)
		{
			var locationManager = (LocationManager)mContext.GetSystemService(Context.LocationService);
			var teleManager = (TelephonyManager)mContext.GetSystemService(Context.TelephonyService);

			int cid = -1;
			int lac = -1;

			if (location is Android.Telephony.Gsm.GsmCellLocation)
			{
				cid = ((Android.Telephony.Gsm.GsmCellLocation)location).Cid;
				lac = ((Android.Telephony.Gsm.GsmCellLocation)location).Lac;
			}
			else if (location is Android.Telephony.Cdma.CdmaCellLocation)
			{
				cid = ((Android.Telephony.Cdma.CdmaCellLocation)location).BaseStationId;
				lac = ((Android.Telephony.Cdma.CdmaCellLocation)location).NetworkId;
			}

			double lat = -1.0;
			double lng = -1.0;
			string gps_type = "unknown";

			Location gps_pos = null;
			Location network_pos = null;

			if (locationManager.IsProviderEnabled(LocationManager.GpsProvider))
			{
				gps_pos = locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
			}

			if (locationManager.IsProviderEnabled(LocationManager.NetworkProvider))
			{
				network_pos = locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
			}

			if (gps_pos != null && network_pos != null)
			{
				if (gps_pos.Time >= network_pos.Time)
				{
					lat = gps_pos.Latitude;
					lng = gps_pos.Longitude;
					gps_type = "GPS";
				}
				else
				{
					lat = network_pos.Latitude;
					lng = network_pos.Longitude;
					gps_type = "Network";
				}
			}
			else if (gps_pos != null)
			{
				lat = gps_pos.Latitude;
				lng = gps_pos.Longitude;
				gps_type = "GPS";
			}
			else if (network_pos != null)
			{
				lat = network_pos.Latitude;
				lng = network_pos.Longitude;
				gps_type = "Network";
			}

			PhoneStatLog.GetInstance().LogCell(DateTime.Now.ToString(),
			                                   cid.ToString(), lac.ToString(),
				                               lat.ToString(), lng.ToString(), gps_type, 
			                                   teleManager.NetworkOperatorName, teleManager.NetworkType.ToString());
			base.OnCellLocationChanged(location);
		}
	}

	public class GPSLocationLogger : Java.Lang.Object, ILocationListener
	{
		public void OnLocationChanged(Location location)
		{
			PhoneStatLog.GetInstance().LogGPS(DateTime.Now.ToString(), location.Latitude.ToString(), location.Longitude.ToString(), "GPS");
		}

		public void OnProviderDisabled(string provider) { }

		public void OnProviderEnabled(string provider) { }

		public void OnStatusChanged(string provider, Availability status, Bundle extras) {
		}
	}


	public class NetworkLocationLogger : Java.Lang.Object, ILocationListener
	{
		public void OnLocationChanged(Location location)
		{
			PhoneStatLog.GetInstance().LogGPS(DateTime.Now.ToString(), location.Latitude.ToString(), location.Longitude.ToString(), "Network");
		}

		public void OnProviderDisabled(string provider) { }

		public void OnProviderEnabled(string provider) { }

		public void OnStatusChanged(string provider, Availability status, Bundle extras) { }
	}
}
