using System.Diagnostics;
using Microsoft.Extensions.Options;
using UpPoc.Api.Configuration;
using UpPoc.Api.Services.Interfaces;

namespace UpPoc.Api.Services;

public sealed class DesktopUpdateService : IDesktopUpdateService
{
    private readonly UpdateSettings _settings;
    private readonly ILogger<DesktopUpdateService> _logger;

    public DesktopUpdateService(
        IOptions<UpdateSettings> settings,
        ILogger<DesktopUpdateService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task TriggerUpdateAsync(CancellationToken cancellationToken = default)
    {
        var scriptPath = Path.GetFullPath(_settings.DesktopApp.UpdateScriptPath);
        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"Desktop update script not found: {scriptPath}");
        }

        var arguments = _settings.DesktopApp.Arguments ?? string.Empty;

        var startInfo = new ProcessStartInfo
        {
            FileName = scriptPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _logger.LogInformation("Starting desktop update script: {Script} {Arguments}", scriptPath, arguments);

        using var process = new Process { StartInfo = startInfo };

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                _logger.LogInformation("[desktop-update] {Line}", e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                _logger.LogWarning("[desktop-update][stderr] {Line}", e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Desktop update script failed with exit code {process.ExitCode}.");
        }

        _logger.LogInformation("Desktop update script completed successfully.");
    }
}
