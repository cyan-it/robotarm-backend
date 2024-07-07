using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Protocol;
using MQTTnet.Server;

using System.Threading;

namespace RobotRemoteControl.Server;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly MqttFactory _mqttFactory;

    public Worker(ILogger<Worker> logger, MqttFactory mqttFactory)
    {
        _logger = logger;
        _mqttFactory = mqttFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttServerOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpointPort(int.Parse(Environment.GetEnvironmentVariable("MQTT_SERVER_PORT")))
            .WithDefaultEndpoint()
            .Build();

        using var mqttServer = _mqttFactory.CreateMqttServer(mqttServerOptions);
        
        mqttServer.ValidatingConnectionAsync += e =>
        {
            if (e.ClientId != Environment.GetEnvironmentVariable("MQTT_CLIENTID"))
            {
                //e.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
            }

            if (e.UserName != Environment.GetEnvironmentVariable("MQTT_USERNAME"))
            {
                e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            }

            if (e.Password != Environment.GetEnvironmentVariable("MQTT_PASSWORD"))
            {
                e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            }

            return Task.CompletedTask;
        };

        await mqttServer.StartAsync();

        await Task.Delay(Timeout.Infinite, stoppingToken);

        await mqttServer.StopAsync();
    }
}

class MqttConsoleLogger(ILogger<MqttConsoleLogger> logger) : IMqttNetLogger
{
    public bool IsEnabled => true;

    public void Publish(MqttNetLogLevel logLevel, string source, string message, object[]? parameters, Exception? exception)
    {
        parameters = parameters ?? [];

        switch (logLevel)
        {
            case MqttNetLogLevel.Verbose:
                logger.LogDebug(message, parameters);
                break;

            case MqttNetLogLevel.Info:
                logger.LogInformation(message, parameters);
                break;

            case MqttNetLogLevel.Warning:
                logger.LogWarning(exception, message, parameters);
                break;

            case MqttNetLogLevel.Error:
                logger.LogError(exception, message, parameters);
                break;
        }
    }
}