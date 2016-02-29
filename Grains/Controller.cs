using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using GrainInterfaces;
using System.Reactive.Linq;
using Orleans.Streams;
using System.Reactive.Subjects;

namespace Grains
{
	[ImplicitStreamSubscription("TempData")]
	class Controller : Grain, IController
	{
		public Task<string> SayHello()
		{
			return Task.FromResult("q");
		}

		double temperatureLimit = 55;

		public override async Task OnActivateAsync()
		{
			var timeWindow = TimeSpan.FromMilliseconds(10000);
			var updateInterval = TimeSpan.FromMilliseconds(1000);

			double averageTemp = 50;

			var deviceDataStream = new Subject<DeviceData>();

			// read device data from stream
			var stream = GetStreamProvider("SMSProvider")
									.GetStream<DeviceData>(this.GetPrimaryKey(), "TempData");
			// send data from orleans stream to local RX observable
			await stream.SubscribeAsync(async (data, token) => deviceDataStream.OnNext(data));

			// log recieved data to console
			deviceDataStream.Subscribe(x =>
			{
				Console.WriteLine("Device data recieved: DeviceId: {0} Temp: {1}", x.DeviceId, x.Temp);
			});

			// log devices that are above temperature threshold 
			deviceDataStream
				.Where(x => x.Temp > temperatureLimit)
				.Subscribe(x => Console.WriteLine("Alert for Device {0}! Temp is {1}",  x.DeviceId, x.Temp));

			// calculate average temperature for all device temperature data collected in a time window
			var averageTempStream = deviceDataStream
				.Select(x => x.Temp)
				// create time window
				.ImmediateWindow(timeWindow, updateInterval)
				.SelectMany(async x => {
					try {
						averageTemp = await x.Average();
					}
					catch (Exception e){}
					return averageTemp;
				});


			// log average temperature between all devices, assigned to controller
			averageTempStream.Subscribe(x => Console.WriteLine("Controler average temperature: {0}", x));

			await base.OnActivateAsync();
		}
	}
}
