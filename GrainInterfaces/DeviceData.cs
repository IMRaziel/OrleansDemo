using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces
{
	// data structure for sending temperature data from device to controller
	public struct DeviceData 
	{
		public long DeviceId;
		public double Temp;

		public DeviceData(long deviceId, double averageTemp) : this()
		{
			this.DeviceId = deviceId;
			this.Temp = averageTemp;
		}
	}
}
