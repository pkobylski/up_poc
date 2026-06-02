using UpPoc.Api.Configuration;
using UpPoc.Api.Services;
using UpPoc.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<UpdateSettings>(
    builder.Configuration.GetSection(UpdateSettings.SectionName));

builder.Services.AddSingleton<IUpdateStateStore, UpdateStateStore>();
builder.Services.AddSingleton<IKubernetesUpdateService, KubernetesUpdateService>();
builder.Services.AddSingleton<IDesktopUpdateService, DesktopUpdateService>();
builder.Services.AddSingleton<IUpdateOrchestrator, UpdateOrchestrator>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
