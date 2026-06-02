using Microsoft.Extensions.Options;
using UpPoc.Api.Configuration;
using UpPoc.Api.Models;
using UpPoc.Api.Services.Interfaces;

namespace UpPoc.Api.Services;

public sealed class UpdateStateStore : IUpdateStateStore
{
    private readonly object _lock = new();
    private UpdateStatusResponse _current;

    public UpdateStateStore(IOptions<UpdateSettings> settings)
    {
        var value = settings.Value;
        _current = new UpdateStatusResponse
        {
            State = "Idle",
            Message = "No update started.",
            CurrentVersion = value.CurrentVersion,
            TargetVersion = value.TargetVersion,
            RestartRequired = false,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public UpdateStatusResponse Get()
    {
        lock (_lock)
        {
            return new UpdateStatusResponse
            {
                State = _current.State,
                Message = _current.Message,
                CurrentVersion = _current.CurrentVersion,
                TargetVersion = _current.TargetVersion,
                RestartRequired = _current.RestartRequired,
                UpdatedAt = _current.UpdatedAt
            };
        }
    }

    public void SetState(string state, string message, bool restartRequired = false)
    {
        lock (_lock)
        {
            _current.State = state;
            _current.Message = message;
            _current.RestartRequired = restartRequired;
            _current.UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
