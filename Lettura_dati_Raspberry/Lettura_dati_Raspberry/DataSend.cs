using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using lettura_dati_Raspberry;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
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

    Data data = new Data();

    public static IMqttClient GetMqttClient()
    {
        if (_mqttClient == null)
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
        }

        return _mqttClient;
    }

    public static async Task<bool> ConnectAsync()
    {
        try
        {
            if (_mqttClient == null)
            {
                var factory = new MqttFactory();
                _mqttClient = factory.CreateMqttClient();
            }

            if (_mqttClient.IsConnected)
            {
                return true;
            }

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_brokerAddress, _brokerPort)
                .WithCredentials(_username, _password)
                .WithClientId(_clientId)
                .WithProtocolVersion(_protocolVersion)
            .Build();

            await _mqttClient.ConnectAsync(options);
            return _mqttClient.IsConnected;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting MQTT client: {ex.Message}");
            return false;
        }
    }

    public static async Task DisconnectAsync()
    {
        try
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disconnecting MQTT client: {ex.Message}");
        }
    }

    public static async Task PublishMqttMessageAsync(string topic, string payload)
    {
        try
        {
            var mqttClient = GetMqttClient();

            // Connect using the shared MQTT client instance
            await ConnectAsync();

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();

            await mqttClient.PublishAsync(message);

            // Disconnect using the shared MQTT client instance
            await DisconnectAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing MQTT message: {ex.Message}");
        }
    }

    public static string GetFullTopic(string dataType)
    {
        // Concatenate the base topic with the specific data type
        return $"{_baseTopic}/{dataType}";
    }

    public string GetDataInfo(string datatype)
    {
        var GetRaminfo = data.GetRamInfo();
        var GetRominfo = data.GetRomInfo();
        var GetCpuinfo = data.GetCpuInfo();

        return $"Info RAM: {GetRaminfo} , Info ROM {GetRominfo}, Info CPU {GetCpuinfo}";
    }

    public async Task PublishDataInfoMqttAsync(string dataType)
    {
        try
        {
            string dataInfo = GetDataInfo(dataType);
            string fullTopic = $"{GetFullTopic(dataType)}";

            await PublishMqttMessageAsync(fullTopic, dataInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing {dataType} info MQTT message: {ex.Message}");
        }
    }
}
