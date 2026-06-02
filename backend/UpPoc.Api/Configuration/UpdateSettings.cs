namespace UpPoc.Api.Configuration;

public sealed class UpdateSettings
{
    public const string SectionName = "UpdateSettings";

    public string CurrentVersion { get; set; } = "1.0.0";
    public string TargetVersion { get; set; } = "1.0.1";
    public bool UseFakeClusterUpdate { get; set; } = true;
    public bool UseFakeDesktopUpdateScript { get; set; } = true;
    public bool SelfUpdateEnabled { get; set; } = false;
    public string SelfDeploymentName { get; set; } = string.Empty;
    public string SelfNamespace { get; set; } = "default";
    public DesktopAppConfig DesktopApp { get; set; } = new();
    public List<DeploymentConfig> Deployments { get; set; } = new();
}

public sealed class DeploymentConfig
{
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = "default";
    public string ContainerName { get; set; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;
}

public sealed class DesktopAppConfig
{
    public string PackageName { get; set; } = string.Empty;
    public string UpdateScriptPath { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;
}
