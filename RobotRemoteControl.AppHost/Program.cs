using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var mqttClientId = builder.AddParameter("MQTTClientId", secret: true);
var mqttUsername = builder.AddParameter("MQTTUsername", secret: true);
var mqttPassword = builder.AddParameter("MQTTPassword", secret: true);

var mqttServer = builder.AddProject<RobotRemoteControl_Server>("mqtt-server")
    .WithEnvironment("MQTT_CLIENTID", mqttClientId)
    .WithEnvironment("MQTT_USERNAME", mqttUsername)
    .WithEnvironment("MQTT_PASSWORD", mqttPassword)
    .WithEndpoint(port: 1883, scheme: "tcp", env: "MQTT_SERVER_PORT", isProxied: false);

builder.AddProject<RobotRemoteControl_UI>("robotremotecontrol-ui")
    .WithEnvironment("MQTT_CLIENTID", mqttClientId)
    .WithEnvironment("MQTT_USERNAME", mqttUsername)
    .WithEnvironment("MQTT_PASSWORD", mqttPassword)
    .WithReference(mqttServer);

builder.Build().Run();
