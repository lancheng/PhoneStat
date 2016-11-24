using Android.OS;
using System;
using System.IO;
namespace PhoneStats
{
	public class PhoneStatLog
	{
		static PhoneStatLog logInstance = null;

		string mGPSLogFileName;
		StreamWriter mGPSLog;

		string mCellLogFileName;
		StreamWriter mCellLog;

		string mPhoneLogFileName;
		StreamWriter mPhoneLog;

		public string DeviceID
		{
			set
			{
				string folder_name = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + @"/com.sinocloudbase.phonestats/";
				Directory.CreateDirectory(folder_name);

				mGPSLogFileName = folder_name + value + "_GPS_" + DateTime.Now.ToString("yyMMddHHmmss") + ".txt";
				mGPSLog = new StreamWriter(mGPSLogFileName, true);

				mCellLogFileName = folder_name + value + "_CELL_" + DateTime.Now.ToString("yyMMddHHmmss") + ".txt";
				mCellLog = new StreamWriter(mCellLogFileName, true);

				mPhoneLogFileName = folder_name + value + "_EVENT_" + DateTime.Now.ToString("yyMMddHHmmss") + ".txt";
				mPhoneLog = new StreamWriter(mPhoneLogFileName, true);
			}
				
		}

		static public PhoneStatLog GetInstance()
		{
			if (logInstance == null)
			{
				logInstance = new PhoneStatLog();
			}
			 
			return logInstance;
		}

		public void LogGPS(string timestamp, string latitude, string longitude, string position_type)
		{
			mGPSLog.WriteLine("{0},{1},{2},{3}", longitude, latitude, timestamp, position_type);
			mGPSLog.Flush();
		}

		public void LogCell(string timestamp, string cell_id, string lac, string latitude, string longitude, string position_type, string carrier, string flag)
		{
			mCellLog.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}", timestamp, cell_id, lac, longitude, latitude, position_type, carrier, flag);
			mCellLog.Flush();
		}

		public void LogPhone(string timestamp, string event_type)
		{
			mPhoneLog.WriteLine("{0},{1}", timestamp, event_type);
			mPhoneLog.Flush();
		}

		public void Close()
		{
			mGPSLog.Close();
			mCellLog.Close();
			mPhoneLog.Close();
		}
			
	}
}
