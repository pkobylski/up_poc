namespace UpPoc.Api.Services.Interfaces;

public interface IKubernetesUpdateService
{
    Task UpdateClusterAsync(CancellationToken cancellationToken = default);
    Task UpdateSelfAsync(CancellationToken cancellationToken = default);
}
