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
        ActivityListener _listener = new ActivityListener();
        private static readonly Gauge requestDuration = Metrics.CreateGauge("http_request_duration_ms", "Request processing duration in milliseconds.", new GaugeConfiguration
        {
            LabelNames = new string[] { "method", "path" }

        });

        public RequestDurationMeasurementMiddleware(RequestDelegate next, 
            ILogger<RequestDurationMeasurementMiddleware> logger)
        {
            _next = next;
            this.logger = logger;

            _listener.ShouldListenTo = src => src.Name == Program.AppActivitySource.Name;

            ActivitySource.AddActivityListener(_listener);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/metrics")
            {
                await _next(context);
                return;
            }
            var activityFeature = context.Features.Get<IHttpActivityFeature>();

            Func<HttpContext, Action<Activity>> highGet = httpContext => 
            {
                return (Activity activity) =>
                {
                    logger.LogInformation($"Took {activity.Duration.TotalMilliseconds} ms to process request {context.Request.Method} {context.Request.Path}.");

                    requestDuration
                        .WithLabels(httpContext.Request.Method, httpContext.Request.Path)
                        .Set(activity.Duration.TotalMilliseconds);
                };
            };

            _listener.ActivityStopped = highGet(context);      
            await _next(context);
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
