namespace TrashMob.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[ApiController]
[ApiVersion("1.0")]
public abstract class BaseController : ControllerBase
{
    private ILogger logger;

    public BaseController()
    {
    }

    public BaseController(ILogger logger)
    {
        this.logger = logger;
    }

    protected ILogger Logger
    {
        get => logger ?? (logger = HttpContext.RequestServices.GetService<ILoggerFactory>()?.CreateLogger(GetType()));
        private set => logger = value;
    }

    protected void TrackEvent(string eventName)
    {
        Logger?.LogInformation("TrackEvent: {EventName}", eventName);
    }
}

