using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Grains
{
	// returns observable, that produces a list of all incoming values as observable collection 
	// before windowed stream reaches first window opening
	public static class ImmediateWindowExtension
	{
		public static IObservable<IObservable<T>> ImmediateWindow<T>(this IObservable<T> obs, TimeSpan timeWindow, TimeSpan shiftTime) {
			var elapsedTime = TimeSpan.FromMilliseconds(0);
			var list = new List<T>();
			var q = obs.Subscribe(x=> { list.Add(x); });

			// produce observable collections of all incoming values
			var head = Observable.Interval(shiftTime)
				.Select(x => {
					elapsedTime += shiftTime;
					return list.ToObservable();
				})
				// while window is not open
				.TakeWhile(_ => elapsedTime < timeWindow)
				// when window is opened, stop it
				.Finally(() => q.Dispose());
			
			var headStream = head.Select(x => x);
			var windowedStream = obs.Window(timeWindow, shiftTime);

			return Observable.Merge(
				head,
				windowedStream
			);
		}
	}
}
