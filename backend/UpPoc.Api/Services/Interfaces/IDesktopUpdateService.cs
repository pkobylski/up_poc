namespace UpPoc.Api.Services.Interfaces;

public interface IDesktopUpdateService
{
    Task TriggerUpdateAsync(CancellationToken cancellationToken = default);
}
