﻿using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace Lettura_dati_Raspberry;

internal class DataSend
{
    private static IMqttClient _mqttClient;
    private static string _brokerAddress = "indirizzo server";
    private static int _brokerPort = 1883;
    private static string _username = "nome utente";
    private static string _password = "password";
    private static string _clientId = "your-client-id";
    private static MqttProtocolVersion _protocolVersion = MqttProtocolVersion.V311;
    private static string _baseTopic = "base_topic"; // Replace with your desired base topic

    private static void _initclient()
    {
        if (_mqttClient == null)
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
        }
    }

    private static async Task _connectclient()
    {
        _initclient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_brokerAddress, _brokerPort)
            .WithCredentials(_username, _password)
            .WithClientId(_clientId)
            .WithProtocolVersion(_protocolVersion)
            .Build();

        await _mqttClient.ConnectAsync(options);
    }

    public static async Task Send(string topic, string message)
    {
        await _connectclient();

        var fullTopic = $"{_baseTopic}/{topic}";
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(fullTopic)
            .WithPayload(message)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)  // Choose the appropriate QoS level
            .WithRetainFlag()
            .Build();


        await _mqttClient.PublishAsync(mqttMessage);
    }
}
