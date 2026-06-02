namespace UpPoc.Api.Models;

public sealed class UpdateStatusResponse
{
    public string State { get; set; } = "Idle";
    public string Message { get; set; } = "No update started.";
    public string CurrentVersion { get; set; } = string.Empty;
    public string TargetVersion { get; set; } = string.Empty;
    public bool RestartRequired { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
