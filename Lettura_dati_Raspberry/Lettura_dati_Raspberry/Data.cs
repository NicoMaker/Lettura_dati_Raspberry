﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Lettura_dati_Raspberry;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using System.IO.Ports;
using System.Device.Gpio;
using System.Net.Mail;

namespace lettura_dati_Raspberry;

class Data
{
    private GpioController gpioController;
    private int numberOfPins;

    public Data()
    {
        gpioController = new GpioController();
        numberOfPins = gpioController.PinCount;
    }

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
                                    Unit = "%",
                                    ContentType = "Numeric"
                                }
                             );

                            sensorData.Add(
                                new SensorData
                                {
                                    Name = "RAM/Used",
                                    Value = usedRamPercentage.ToString(CultureInfo.InvariantCulture),
                                    Unit = "%",
                                    ContentType = "Numeric"
                                });

                            sensorData.Add(
                                new SensorData
                                {
                                    Name = "RAM/Total",
                                    Value = totalRam.ToString(CultureInfo.InvariantCulture),
                                    Unit = "MB",
                                    ContentType = "Numeric"
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

                            sensorData.Add(
                                new SensorData // istaznzio già i dati popolandoli con i dati interessati
                                {
                                    Name = "ROM/Free",
                                    Value = usedrompercentual.ToString(CultureInfo.InvariantCulture),
                                    Unit = "%",
                                    ContentType = "Numeric"
                                }
                             );

                            sensorData.Add(
                                new SensorData
                                {
                                    Name = "ROM/Used",
                                    Value = freerompercentual.ToString(CultureInfo.InvariantCulture),
                                    Unit = "%",
                                    ContentType = "Numeric"
                                });

                            sensorData.Add(
                                new SensorData
                                {
                                    Name = "ROM/Total",
                                    Value = totalRom.ToString(CultureInfo.InvariantCulture),
                                    Unit = "GB",
                                    ContentType = "Numeric"
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



    public List<SensorData> GetCpuInfo()
    {
        List<SensorData> sensorData = new List<SensorData>();

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

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        // Split the line into key and value
                        string[] parts = line.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();


                            sensorData.Add(new SensorData
                            {
                                Name = $"CPU/{key}",
                                Value = value,
                                Unit = "",
                                ContentType = "Text"
                            });
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

    public List<SensorData> GetMacAddress()
    {
        List<SensorData> sensorData = new List<SensorData>();
        string macAddress = string.Empty;

        try
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    macAddress = networkInterface.GetPhysicalAddress().ToString();

                    sensorData.Add(new SensorData
                    {
                        Name = "MAC ADDRESS",
                        Value = macAddress,
                        Unit = ""
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return sensorData;
    }


    public List<SensorData> ReadSerialData()
    {
        List<SensorData> sensorData = new List<SensorData>();

        try
        { 
            for (int i = 0; i < numberOfPins; i++)
            {
                var pin = gpioController.OpenPin(i, PinMode.Input);
                var pinValue = pin.Read();
                gpioController.ClosePin(i);

                sensorData.Add(new SensorData
                {
                    Name = $"GPIO/pin{i}",
                    Value = (pinValue == PinValue.High) ? "True" : "False",
                    ContentType = "Bool"
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore durante la lettura dei dati del sensore: {ex.Message}");
        }
        finally
        {
            gpioController.Dispose();
        }

        return sensorData;
    }

}

