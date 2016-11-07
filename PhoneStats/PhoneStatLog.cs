using Android.OS;
using System;
using System.IO;
namespace PhoneStats
{
	public class PhoneGPSLog
	{
		string logFileName;
		static PhoneGPSLog logInstance = null;
		StreamWriter logFile;

		private PhoneGPSLog(string device_id)
		{
			
		}

		public string DeviceID
		{
			set
			{
				logFileName = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/" + value + "_GPS_" + DateTime.Now.ToString("yyMMddHHmmss") + ".csv";
				logFile = new StreamWriter(logFileName, true);
			}
				
		}

		static public PhoneGPSLog GetInstance(string device_id)
		{
			if (logInstance != null)
			{
				return logInstance;
			}

			return new PhoneGPSLog(device_id);
		}

		public void Debug(string timestamp, string latitude, string longitude, string position_type)
		{
			logFile.WriteLine("{0},{1},{2},{3}", longitude, latitude, timestamp, position_type);
		}

		public void Close()
		{
			logFile.Close();	
		}
			
	}
}
