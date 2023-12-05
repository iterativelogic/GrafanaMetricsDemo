using Microsoft.AspNetCore.Http.Features;
using Prometheus;
using System.Diagnostics;
using System.Globalization;

namespace GrafanaMetricsDemo
{
    public class RequestDurationMeasurementMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestDurationMeasurementMiddleware> logger;
        private static readonly Gauge requestDuration = Metrics.CreateGauge("http_request_duration_ms", "Request processing duration in milliseconds.", new GaugeConfiguration
        {
            LabelNames = new string[] { "method", "path" }

        });

        public RequestDurationMeasurementMiddleware(RequestDelegate next, ILogger<RequestDurationMeasurementMiddleware> logger)
        {
            _next = next;
            this.logger = logger;


        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/metrics")
            {
                await _next(context);
                return;
            }

            var st = new Stopwatch();
            st.Start();
            await _next(context);
            st.Stop();

            logger.LogInformation($"Took {st.ElapsedMilliseconds} ms to process request {context.Request.Method} {context.Request.Path}.");
            requestDuration
                .WithLabels(context.Request.Method, context.Request.Path)
                .Set(st.ElapsedMilliseconds);
        }
    }

    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestDurationMeasurement(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestDurationMeasurementMiddleware>();
        }
    }
}
