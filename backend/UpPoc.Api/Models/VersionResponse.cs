namespace UpPoc.Api.Models;

public sealed class VersionResponse
{
    public string CurrentVersion { get; set; } = string.Empty;
    public string TargetVersion { get; set; } = string.Empty;
    public bool UpdateAvailable { get; set; }
}
