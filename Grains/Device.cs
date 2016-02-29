using System.Threading.Tasks;
using Orleans;
using GrainInterfaces;
using System;
using System.Reactive.Linq;
using Orleans.Streams;

namespace Grains
{
	/// <summary>
	/// Grain implementation class Grain1.
	/// </summary>
	public class Device : Grain, IDevice
	{
		double lastTemp = 50;
		double averageTemp = 50;
		IObservable<double> tempGenerator;

		static Random rnd = new Random();

		public async override Task OnActivateAsync()
		{
			var dt = 2.0;
			// generator to imitate sensor data
			tempGenerator = Observable.Generate(lastTemp, x => true, x => x - dt + 2 * dt * rnd.NextDouble(), x => x, x => TimeSpan.FromMilliseconds((int)(rnd.NextDouble() * 4000) + 1000));
			// init with default temperature
			tempGenerator = Observable.FromAsync(() => Task.FromResult(lastTemp)).Concat(tempGenerator);

			var timeWindow = TimeSpan.FromMilliseconds(3000);
			var updateInterval = TimeSpan.FromMilliseconds(1000);

			var startTime = DateTime.Now;

			// update average temperature, if recieved new data in 3 seconds
			tempGenerator
				// create time window
				.ImmediateWindow(timeWindow, updateInterval)
				// update average temp 
				.Subscribe(async x =>
				{
					try
					{
						averageTemp = await x.Average();
					}
					catch (Exception e) { }
				});

			await base.OnActivateAsync();
		}

		public async override Task OnDeactivateAsync()
		{
			await base.OnDeactivateAsync();
		}

		Guid controllerId;

		public async Task AssignToController(Guid controllerId)
		{
			this.controllerId = controllerId;
			var stream = GetStreamProvider("SMSProvider")
						.GetStream<DeviceData>(controllerId, "TempData");
			// every second send average teperature to controller's stream
			RegisterTimer(async _ =>
				{
					await stream.OnNextAsync(new DeviceData(this.GetPrimaryKeyLong(), this.averageTemp));
				}, 
				this, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(1000)
			);
		}
	}
}
