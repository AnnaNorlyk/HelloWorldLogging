using Serilog;
using System.Diagnostics;
using System.Reflection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using Serilog.Enrichers.Span;

namespace Monitoring
{
    public static class MonitorService
    {
        public static readonly string ServiceName = Assembly.GetCallingAssembly().GetName().Name ?? "Unknown";
        public static TracerProvider TracerProvider;
        public static ActivitySource ActivitySource = new ActivitySource(ServiceName);


        public static ILogger Log => Serilog.Log.Logger;

        static MonitorService()
        {

            // OpenTelemetry
            TracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddConsoleExporter()
                .AddZipkinExporter(options =>
                {
                    options.Endpoint = new Uri("http://zipkin:9411/api/v2/spans");
                })
                .AddSource(ActivitySource.Name)
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName))
                .Build();

            Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));
            //Serilog
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.Seq(serverUrl: "http://seq:5341")
                .Enrich.WithSpan()
                .CreateLogger();


        }

    }
}
