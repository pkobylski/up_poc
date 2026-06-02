using UpPoc.Api.Services.Interfaces;

namespace UpPoc.Api.Services;

public sealed class UpdateOrchestrator : IUpdateOrchestrator
{
    private readonly IKubernetesUpdateService _kubernetesUpdateService;
    private readonly IDesktopUpdateService _desktopUpdateService;
    private readonly IUpdateStateStore _stateStore;
    private readonly ILogger<UpdateOrchestrator> _logger;
    private int _running;

    public UpdateOrchestrator(
        IKubernetesUpdateService kubernetesUpdateService,
        IDesktopUpdateService desktopUpdateService,
        IUpdateStateStore stateStore,
        ILogger<UpdateOrchestrator> logger)
    {
        _kubernetesUpdateService = kubernetesUpdateService;
        _desktopUpdateService = desktopUpdateService;
        _stateStore = stateStore;
        _logger = logger;
    }

    public bool TryStart()
    {
        if (Interlocked.Exchange(ref _running, 1) == 1)
        {
            return false;
        }

        _ = Task.Run(RunAsync);
        return true;
    }

    private async Task RunAsync()
    {
        try
        {
            _stateStore.SetState("Starting", "Update started.");

            _stateStore.SetState("UpdatingCluster", "Updating cluster workloads...");
            await _kubernetesUpdateService.UpdateClusterAsync();

            _stateStore.SetState("UpdatingDesktop", "Updating desktop package on host...");
            await _desktopUpdateService.TriggerUpdateAsync();

            _stateStore.SetState("RestartRequired", "Desktop package updated. Restart the desktop app.", restartRequired: true);

            _stateStore.SetState("UpdatingSelf", "Updating backend service as the final step...");
            await _kubernetesUpdateService.UpdateSelfAsync();

            _stateStore.SetState("Completed", "Update completed successfully.", restartRequired: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update failed.");
            _stateStore.SetState("Failed", ex.Message);
        }
        finally
        {
            Interlocked.Exchange(ref _running, 0);
        }
    }
}
