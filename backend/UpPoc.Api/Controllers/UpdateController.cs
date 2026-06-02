using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UpPoc.Api.Configuration;
using UpPoc.Api.Models;
using UpPoc.Api.Services.Interfaces;

namespace UpPoc.Api.Controllers;

[ApiController]
[Route("api/update")]
public sealed class UpdateController : ControllerBase
{
    private readonly IUpdateOrchestrator _orchestrator;
    private readonly IUpdateStateStore _stateStore;
    private readonly UpdateSettings _settings;

    public UpdateController(
        IUpdateOrchestrator orchestrator,
        IUpdateStateStore stateStore,
        IOptions<UpdateSettings> settings)
    {
        _orchestrator = orchestrator;
        _stateStore = stateStore;
        _settings = settings.Value;
    }

    [HttpGet("version")]
    public ActionResult<VersionResponse> GetVersion()
    {
        return Ok(new VersionResponse
        {
            CurrentVersion = _settings.CurrentVersion,
            TargetVersion = _settings.TargetVersion,
            UpdateAvailable = _settings.CurrentVersion != _settings.TargetVersion
        });
    }

    [HttpGet("status")]
    public ActionResult<UpdateStatusResponse> GetStatus()
    {
        return Ok(_stateStore.Get());
    }

    [HttpPost("start")]
    public IActionResult Start()
    {
        var started = _orchestrator.TryStart();
        if (!started)
        {
            return Conflict(new { message = "Update is already running." });
        }

        return Accepted(new { message = "Update started." });
    }
}
