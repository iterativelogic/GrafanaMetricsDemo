using Microsoft.AspNetCore.Http.Features;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
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
            var services = builder.Services;

            services.AddControllers();

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("GrafanaDemoApp"))
                .WithTracing(builder =>
                    builder
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddZipkinExporter(o => o.HttpClientFactory = () =>
                    {
                        HttpClient client = new HttpClient();
                        client.DefaultRequestHeaders.Add("X-MyCustomHeader", "value");
                        return client;
                    }));



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

            app.UseHttpMetrics();
            app.MapMetrics();

            app.Run();
        }

        static void OnMeasurementRecorded<T>(Instrument instrument, T measurement, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
        {
            Console.WriteLine($"--> {instrument.Name} recorded measurement {measurement}");
        }
    }

}
