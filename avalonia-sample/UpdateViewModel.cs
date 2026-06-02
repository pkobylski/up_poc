using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AvaloniaSample;

public sealed class UpdateViewModel : INotifyPropertyChanged
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("http://localhost:5080")
    };

    private bool _isBusy;
    private string _statusMessage = "Ready";
    private bool _restartRequired;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public bool RestartRequired
    {
        get => _restartRequired;
        set
        {
            _restartRequired = value;
            OnPropertyChanged();
        }
    }

    public async Task StartUpdateAsync()
    {
        IsBusy = true;
        StatusMessage = "Starting update...";

        var response = await _httpClient.PostAsync("/api/update/start", null);
        response.EnsureSuccessStatusCode();

        while (true)
        {
            var status = await _httpClient.GetFromJsonAsync<UpdateStatusDto>("/api/update/status");
            if (status is null)
            {
                StatusMessage = "No status returned.";
                break;
            }

            StatusMessage = $"{status.State}: {status.Message}";
            RestartRequired = status.RestartRequired;

            if (status.State is "Completed" or "Failed")
            {
                break;
            }

            await Task.Delay(2000);
        }

        IsBusy = false;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed class UpdateStatusDto
    {
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("restartRequired")]
        public bool RestartRequired { get; set; }
    }
}
