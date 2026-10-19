using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace EasyClinic.API.Filters
{
    public class LoggingActionFilter : IActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;
        private Stopwatch _stopwatch = new();

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch.Restart();
            _logger.LogInformation(
                "Executing {Action} on {Controller}",
                context.ActionDescriptor.RouteValues["action"],
                context.ActionDescriptor.RouteValues["controller"]);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            _logger.LogInformation(
                "Executed {Action} on {Controller} in {Elapsed}ms",
                context.ActionDescriptor.RouteValues["action"],
                context.ActionDescriptor.RouteValues["controller"],
                _stopwatch.ElapsedMilliseconds);
        }
    }
}