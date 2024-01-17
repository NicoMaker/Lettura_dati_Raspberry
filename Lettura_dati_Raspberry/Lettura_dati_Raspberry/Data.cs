using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace lettura_dati_Raspberry;

class Data
{
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

    private async Task PublishMqttMessageAsync(string topic, string payload)
    {
        try
        {
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("your-mqtt-broker-address", 1883) // Replace with your MQTT broker address and port
                .WithClientId("your-client-id") // Replace with your desired client ID
                .Build();

            await mqttClient.ConnectAsync(options);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();

            await mqttClient.PublishAsync(message);

            await mqttClient.DisconnectAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing MQTT message: {ex.Message}");
        }
    }

    public async Task PublishRamInfoMqttAsync()
    {
        string ramInfo = GetRamInfo();
        await PublishMqttMessageAsync("ram_info_topic", ramInfo);
    }

    public async Task PublishRomInfoMqttAsync()
    {
        string romInfo = GetRomInfo();
        await PublishMqttMessageAsync("rom_info_topic", romInfo);
    }

    public async Task PublishCpuInfoMqttAsync()
    {
        string cpuInfo = GetCpuInfo();
        await PublishMqttMessageAsync("cpu_info_topic", cpuInfo);
    }

}
