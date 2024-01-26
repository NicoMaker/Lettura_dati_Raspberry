using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Lettura_dati_Raspberry;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;

namespace lettura_dati_Raspberry;

class Data
{
    public List<SensorData> GetRamInfo()
    {
        List<SensorData> sensorData = new List<SensorData>();

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

                            sensorData.Add(
                                new SensorData // istaznzio già i dati popolandoli con i dati interessati
                                {
                                    Name = "RAM/Free",
                                    Value = freeRamPercentage.ToString(CultureInfo.InvariantCulture),
                                    Unit = "%"
                                }
                             );

                            sensorData.Add(
                                new SensorData
                                {
                                    Name = "RAM/Used",
                                    Value = usedRamPercentage.ToString(CultureInfo.InvariantCulture),
                                    Unit = "%"
                                });

                            sensorData.Add(
                                new SensorData
                                {
                                    Name = "RAM/Total",
                                    Value = totalRam.ToString(CultureInfo.InvariantCulture),
                                    Unit = "MB"
                                }
                                );
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return sensorData;
    }



    public List<SensorData> GetRomInfo()
    {

        List<SensorData> sensorData = new List<SensorData>();

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

                            double usedrompercentual = (usedRom / totalRom) * 100;
                            double freerompercentual = (freeRom / totalRom) * 100;
                            double totalrommb = totalRom / (1024 * 1024);

                            sensorData.Add(
                                new SensorData // istaznzio già i dati popolandoli con i dati interessati
                                {
                                    Name = "ROM/Free",
                                    Value = usedrompercentual.ToString(CultureInfo.InvariantCulture),
                                    Unit = "%"
                                }
                             );

                            sensorData.Add(
                                new SensorData
                                {
                                    Name = "ROM/Used",
                                    Value = freerompercentual.ToString(CultureInfo.InvariantCulture),
                                    Unit = "%"
                                });

                            sensorData.Add(
                                new SensorData
                                {
                                    Name = "ROM/Total",
                                    Value = totalrommb.ToString(CultureInfo.InvariantCulture),
                                    Unit = "MB"
                                }
                                );
                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return sensorData;
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
}
