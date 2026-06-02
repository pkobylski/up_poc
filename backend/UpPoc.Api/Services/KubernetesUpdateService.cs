using k8s;
using k8s.Models;
using Microsoft.Extensions.Options;
using UpPoc.Api.Configuration;
using UpPoc.Api.Services.Interfaces;

namespace UpPoc.Api.Services;

public sealed class KubernetesUpdateService : IKubernetesUpdateService
{
    private readonly UpdateSettings _settings;
    private readonly ILogger<KubernetesUpdateService> _logger;

    public KubernetesUpdateService(
        IOptions<UpdateSettings> settings,
        ILogger<KubernetesUpdateService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task UpdateClusterAsync(CancellationToken cancellationToken = default)
    {
        if (_settings.UseFakeClusterUpdate)
        {
            foreach (var deployment in _settings.Deployments)
            {
                _logger.LogInformation(
                    "[FAKE] Updating deployment {Namespace}/{Deployment} to image {Image}:{Version}",
                    deployment.Namespace,
                    deployment.Name,
                    deployment.ImageName,
                    _settings.TargetVersion);

                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }

            return;
        }

        var config = KubernetesClientConfiguration.InClusterConfig();
        var client = new Kubernetes(config);

        foreach (var deployment in _settings.Deployments)
        {
            var patch = new V1Deployment
            {
                Spec = new V1DeploymentSpec
                {
                    Template = new V1PodTemplateSpec
                    {
                        Spec = new V1PodSpec
                        {
                            Containers = new List<V1Container>
                            {
                                new()
                                {
                                    Name = deployment.ContainerName,
                                    Image = $"{deployment.ImageName}:{_settings.TargetVersion}"
                                }
                            }
                        }
                    }
                }
            };

            await client.AppsV1.PatchNamespacedDeploymentAsync(
                new V1Patch(patch, V1Patch.PatchType.StrategicMergePatch),
                deployment.Name,
                deployment.Namespace,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Patched deployment {Namespace}/{Deployment}",
                deployment.Namespace,
                deployment.Name);
        }
    }

    public async Task UpdateSelfAsync(CancellationToken cancellationToken = default)
    {
        if (!_settings.SelfUpdateEnabled)
        {
            _logger.LogInformation("Self update is disabled.");
            return;
        }

        if (_settings.UseFakeClusterUpdate)
        {
            _logger.LogInformation("[FAKE] Updating self deployment {Namespace}/{Deployment}", _settings.SelfNamespace, _settings.SelfDeploymentName);
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            return;
        }

        var target = _settings.Deployments.FirstOrDefault(x =>
            string.Equals(x.Name, _settings.SelfDeploymentName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.Namespace, _settings.SelfNamespace, StringComparison.OrdinalIgnoreCase));

        if (target is null)
        {
            _logger.LogWarning("Self deployment was not found in configuration.");
            return;
        }

        var config = KubernetesClientConfiguration.InClusterConfig();
        var client = new Kubernetes(config);

        var patch = new V1Deployment
        {
            Spec = new V1DeploymentSpec
            {
                Template = new V1PodTemplateSpec
                {
                    Spec = new V1PodSpec
                    {
                        Containers = new List<V1Container>
                        {
                            new()
                            {
                                Name = target.ContainerName,
                                Image = $"{target.ImageName}:{_settings.TargetVersion}"
                            }
                        }
                    }
                }
            }
        };

        await client.AppsV1.PatchNamespacedDeploymentAsync(
            new V1Patch(patch, V1Patch.PatchType.StrategicMergePatch),
            target.Name,
            target.Namespace,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Self deployment patched as the last step.");
    }
}
