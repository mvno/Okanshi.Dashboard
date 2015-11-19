using System;
using System.Collections.Generic;
using System.Linq;

namespace Okanshi.Dashboard.Models
{
	public static class OkanshiMetricExtensions
	{
		public static Metric ToMetric(this OkanshiMetric metric)
		{
			return new Metric
			{
				Name = metric.Name,
				WindowSize = metric.WindowSize,
				Measurements =
					metric.Measurements == null
						? Enumerable.Empty<Measurement<DateTime, decimal>>()
						: metric.Measurements.ToMeasurements(metric.WindowSize).ToArray()
			};
		}

		private static IEnumerable<Measurement<DateTime, decimal>> ToMeasurements(this IEnumerable<OkanshiMeasurement> measurements, double windowSize)
		{
			var evaluatedMeasurements = measurements.ToArray();
			if (!evaluatedMeasurements.Any())
			{
				yield break;
			}

			var orderedMeasurements = evaluatedMeasurements.OrderBy(x => x.StartTime).ToArray();
			var halfWindowSize = TimeSpan.FromMilliseconds(windowSize / 2);
			var numberOfMeasurements = orderedMeasurements.Length;
			for (var i = 0; i < numberOfMeasurements; i++)
			{
				var currentMeasurement = orderedMeasurements[i];
				var time = currentMeasurement.StartTime.Add(halfWindowSize);
				yield return new Measurement<DateTime, decimal> { X = time, Y = currentMeasurement.Average };
				var nextIndex = i + 1;
				if (nextIndex >= numberOfMeasurements)
				{
					break;
				}

				var nextMeasurementStartTime = orderedMeasurements[nextIndex].StartTime;
				var firstPossibleEmptyPoint = currentMeasurement.EndTime.Add(halfWindowSize);
				if (firstPossibleEmptyPoint < nextMeasurementStartTime)
				{
					yield return new Measurement<DateTime, decimal> { X = firstPossibleEmptyPoint, Y = 0m };
					var lastPossibleEmptyPoint = nextMeasurementStartTime.Subtract(halfWindowSize);
					if (lastPossibleEmptyPoint >= firstPossibleEmptyPoint)
					{
						yield return new Measurement<DateTime, decimal> { X = lastPossibleEmptyPoint, Y = 0m };
					}
				}
			}

			var maxTime = orderedMeasurements.Max(x => x.EndTime).Add(halfWindowSize);
			var now = DateTime.Now;
			if (maxTime < now)
			{
				yield return new Measurement<DateTime, decimal> { X = maxTime, Y = 0 };
				yield return new Measurement<DateTime, decimal> { X = now, Y = 0 };
			}
		} 
	}
}