using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Prometheus;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace GrafanaMetricsDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            //builder.Services
            //    .AddOpenTelemetry()
            //    .WithTracing(builder => {
            //        builder.AddAspNetCoreInstrumentation();
            //        builder.AddConsoleExporter();
            //    })
            //    .WithMetrics(builder => {
            //        builder.AddAspNetCoreInstrumentation();
            //        builder.AddPrometheusExporter();
            //        builder.AddConsoleExporter();
            //    });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseRequestDurationMeasurement();
            app.UseAuthorization();
            app.MapControllers();
            app.MapMetrics();

            app.Run();
        }

        static void OnMeasurementRecorded<T>(Instrument instrument, T measurement, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
        {
            Console.WriteLine($"--> {instrument.Name} recorded measurement {measurement}");
        }
    }

}
