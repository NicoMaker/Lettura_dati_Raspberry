using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;

namespace lettura_dati_Raspberry;

class Data
{
    private static IMqttClient _mqttClient;
    private static string _brokerAddress = "indirizzo server";
    private static int _brokerPort = 1883;
    private static string _username = "nome utente";
    private static string _password = "password";
    private static string _clientId = "your-client-id";
    private static MqttProtocolVersion _protocolVersion = MqttProtocolVersion.V311;
    private static string _baseTopic = "base_topic"; // Replace with your desired base topic

    public Data() { } // Private constructor to prevent external instantiation

    public string GetRamInfo()
    {
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "free",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processStartInfo))
            {
                using (var reader = process.StandardOutput)
                {
                    string output = reader.ReadToEnd();
                    string[] lines = output.Split('\n');

                    if (lines.Length >= 2)
                    {
                        string[] values = lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (values.Length >= 4)
                        {
                            ulong totalRam = ulong.Parse(values[1]);
                            ulong usedRam = ulong.Parse(values[2]);
                            ulong freeRam = ulong.Parse(values[3]);

                            // Converti almeno uno dei valori in double per ottenere risultati decimali
                            double usedRamPercentage = (double)usedRam / totalRam * 100;
                            double freeRamPercentage = (double)freeRam / totalRam * 100;

                            return $"RAM Used: {usedRamPercentage:F2}%, RAM Free: {freeRamPercentage:F2}%, RAM Total: {totalRam} MB";
                        }
                    }
                }
            }

            return "Failed to retrieve RAM information";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }



    public string GetRomInfo()
    {
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "df",
                Arguments = "-h /",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processStartInfo))
            {
                using (var reader = process.StandardOutput)
                {
                    string output = reader.ReadToEnd();
                    string[] lines = output.Split('\n');

                    if (lines.Length >= 2)
                    {
                        string[] values = lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (values.Length >= 6)
                        {
                            string totalRomStr = values[1].TrimEnd('G');
                            string usedRomStr = values[2].TrimEnd('G');
                            string freeRomStr = values[3].TrimEnd('G');

                            double totalRom = double.Parse(totalRomStr, CultureInfo.InvariantCulture); // Convert GB to MB
                            double usedRom = double.Parse(usedRomStr, CultureInfo.InvariantCulture); // Convert GB to MB
                            double freeRom = double.Parse(freeRomStr, CultureInfo.InvariantCulture); // Convert GB to MB

                            return $"ROM Used: {(usedRom / totalRom) * 100} %, ROM Free: {(freeRom / totalRom) * 100} %, ROM Total: {totalRom / (1024 * 1024)} MB";
                        }
                    }
                }
            }

            return "Failed to retrieve ROM information";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }



    public string GetCpuInfo()
    {
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cat",
                Arguments = "/proc/cpuinfo",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processStartInfo))
            {
                using (var reader = process.StandardOutput)
                {
                    string output = reader.ReadToEnd();
                    return $"CPU Info:\n{output}";
                }
            }
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

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
        var GetRaminfo = GetRamInfo();
        var GetRominfo = GetRomInfo();
        var GetCpuinfo = GetCpuInfo();

        return $"Info RAM: {GetRaminfo} , Info ROM {GetRomInfo}, Info CPU {GetCpuinfo}";
    }

    public async Task PublishDataInfoMqttAsync(string dataType)
    {
        try
        {
            string dataInfo = GetDataInfo(dataType);
            string fullTopic = $"{Data.GetFullTopic(dataType)}";

            await Data.PublishMqttMessageAsync(fullTopic, dataInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing {dataType} info MQTT message: {ex.Message}");
        }
    }

}
