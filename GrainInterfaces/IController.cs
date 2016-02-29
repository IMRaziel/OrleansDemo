using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
	/// <summary>
	/// Grain interface for Contoller
	/// </summary>
	public interface IController : IGrainWithGuidKey
	{
		Task<string> SayHello();
	}
}
