
using MQTTnet;
using MQTTnet.Diagnostics;

using RobotRemoteControl.Server;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton<IMqttNetLogger, MqttConsoleLogger>();
builder.Services.AddSingleton<MqttFactory>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
