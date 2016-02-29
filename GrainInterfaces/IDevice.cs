using System.Threading.Tasks;
using Orleans;
using System;

namespace GrainInterfaces
{
	/// <summary>
	/// Grain interface for Device
	/// </summary>
	public interface IDevice : IGrainWithIntegerKey
	{
		/// <summary>
		/// assigns device to controller. device sends it's average temperature to the controller every second
		/// </summary>
		/// <param name="controllerId"></param>
		/// <returns></returns>
		Task AssignToController(Guid controllerId);	
	}
}
