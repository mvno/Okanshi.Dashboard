using System;
using System.Linq;
using FluentAssertions;
using Okanshi.Dashboard.Models;
using Xunit;

namespace Okanshi.Dashboard.Tests.Models
{
	public class OkanshiMetricExtensionsTest
	{
		[Fact]
		public void Name_is_preserved_when_converting()
		{
			var okanshiMetric = new OkanshiMetric { Name = "OriginalName" };

			var name = okanshiMetric.ToMetric().Name;

			name.Should().Be(okanshiMetric.Name);
		}

		[Fact]
		public void Window_size_is_preserved_when_converting()
		{
			var okanshiMetric = new OkanshiMetric { WindowSize = 100 };

			var windowSize = okanshiMetric.ToMetric().WindowSize;

			windowSize.Should().Be(okanshiMetric.WindowSize);
		}

		[Fact]
		public void No_okanshi_measurements_are_converted_to_no_measurements()
		{
			var okanshiMetric = new OkanshiMetric { Measurements = Enumerable.Empty<OkanshiMeasurement>() };

			var measurements = okanshiMetric.ToMetric().Measurements;

			measurements.Should().BeEmpty();
		}

		[Fact]
		public void Measurements_are_ordered_according_to_x_value()
		{
			var okanshiMeasurements = new[]
			{
				new OkanshiMeasurement { StartTime = DateTime.Now.AddMinutes(3), EndTime = DateTime.Now.AddMinutes(4) },
				new OkanshiMeasurement { StartTime = DateTime.Now.AddMinutes(1), EndTime = DateTime.Now.AddMinutes(2) }
			};
			var okanshiMetric = new OkanshiMetric { Measurements = okanshiMeasurements };

			var measurements = okanshiMetric.ToMetric().Measurements.ToArray();

			measurements.Should().BeInAscendingOrder(x => x.X);
		}

		[Fact]
		public void Measurements_ending_in_the_past_have_two_empty_data_points_inserted_in_the_end()
		{
			var startTime = DateTime.Now.AddHours(-1);
			var endTime = startTime.AddMinutes(1);
			var okanshiMetric = new OkanshiMetric { Measurements = new[] { new OkanshiMeasurement { EndTime = endTime, StartTime = startTime } } };

			var measurements = okanshiMetric.ToMetric().Measurements.ToArray();

			measurements.Skip(1).Should()
				.OnlyContain(m => m.Y == 0)
				.And.HaveCount(2);
		}

		[Fact]
		public void Measurements_ending_in_the_past_have_two_data_points_inserted_in_the_end_the_first_should_have_x_value_equal_last_acutal_data_point_end_time_plus_a_half_windowSize()
		{
			var startTime = DateTime.Now.AddHours(-1);
			var endTime = startTime.AddMinutes(1);
			var windowSize = (long)TimeSpan.FromMinutes(1).TotalMilliseconds;
			var halfWindowSize = windowSize / 2;
			var okanshiMetric = new OkanshiMetric { Measurements = new[] { new OkanshiMeasurement { EndTime = endTime, StartTime = startTime } }, WindowSize = windowSize };

			var measurements = okanshiMetric.ToMetric().Measurements.ToArray();

			measurements.Skip(1).Take(1).First().X
				.Should().Be(endTime.AddMilliseconds(halfWindowSize));
		}

		[Fact]
		public void Measurements_ending_in_the_past_have_two_data_points_inserted_in_the_end_the_second_should_have_x_value_close_to_now()
		{
			var startTime = DateTime.Now.AddHours(-1);
			var endTime = startTime.AddMinutes(1);
			var okanshiMetric = new OkanshiMetric { Measurements = new[] { new OkanshiMeasurement { EndTime = endTime, StartTime = startTime } }, WindowSize = (long)TimeSpan.FromMinutes(1).TotalMilliseconds };

			var measurements = okanshiMetric.ToMetric().Measurements.ToArray();

			measurements.Skip(2).Take(1).First().X
				.Should().BeCloseTo(DateTime.Now);
		}

		[Fact]
		public void Start_time_added_with_a_half_a_window_size_is_the_x_value()
		{
			var now = DateTime.Now;
			var okanshiMeasurements = new[]
			{
				new OkanshiMeasurement { StartTime = now.AddMinutes(-1), EndTime = now.AddMinutes(1) }
			};
			var metric = new OkanshiMetric { Measurements = okanshiMeasurements, WindowSize = (long)TimeSpan.FromMinutes(2).TotalMilliseconds };

			var measurements = metric.ToMetric().Measurements.ToArray();

			measurements.First().X
				.Should().Be(now);
		}

		[Fact]
		public void When_measurements_contains_holes_empty_data_points_are_inserted()
		{
			var windowSize = (long)TimeSpan.FromMinutes(1).TotalMilliseconds;
			var now = DateTime.Now;
			var okanshiMeasurements = new[]
			{
				new OkanshiMeasurement { StartTime = now.AddHours(-1), EndTime = now.AddHours(-1).AddMilliseconds(windowSize), Average = 100 },
				new OkanshiMeasurement { StartTime = now, EndTime = now.AddMilliseconds(windowSize), Average = 100 }
			};
			var okanshiMetric = new OkanshiMetric { Measurements = okanshiMeasurements, WindowSize = windowSize };

			var measurements = okanshiMetric.ToMetric().Measurements.ToArray();

			measurements.Skip(1).Take(2).Should().OnlyContain(x => x.Y == 0);
			measurements.Skip(3).Take(1).Should().OnlyContain(x => x.Y != 0);
		}
	}
}
