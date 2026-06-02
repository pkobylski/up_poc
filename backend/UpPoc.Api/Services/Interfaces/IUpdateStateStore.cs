using UpPoc.Api.Models;

namespace UpPoc.Api.Services.Interfaces;

public interface IUpdateStateStore
{
    UpdateStatusResponse Get();
    void SetState(string state, string message, bool restartRequired = false);
}
