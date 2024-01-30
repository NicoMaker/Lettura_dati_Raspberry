# visualizza i dati della RAM, ROM totale Libera e usata e CPU dI Raspberry

prima parte di configurazione degli using

- Management

![scaricamneto Nuget](Immagini/Scaricamento_nuget.png)

intanto parte di configurazione degli using e namespace

```C#
using Lettura_dati_Raspberry;
using System;
using System.Runtime.Intrinsics.Arm;
namespace lettura_dati_Raspberry;
```

Leggi le informazioni sulla RAM, ROM e CPU dal PC locale e stampa in automatico anche l'stanza della clsee Data

```C#
class Program
{
    static void Main(string[] args)
    {
        // instanzio oggetto della classe Data
        Data data = new Data();

        // Stampare informazioni sulla RAM, ROM e CPU
        Console.WriteLine("Informazioni sulla RAM:");
        Console.WriteLine(data.GetRamInfo());

        Console.WriteLine("\nInformazioni sulla ROM:");
        Console.WriteLine(data.GetRomInfo());

        Console.WriteLine("\nInformazioni sulla CPU:");
        Console.WriteLine(data.GetCpuInfo());
    }
}
```

implementazione Classe Data con gli using e namespace

```C#
using System.Diagnostics;
using System.Globalizzazion;
namespace lettura_dati_Raspberry;
```

crea la classe Data

```C#
class Data
{
    // vari metodi
}
```

dentro la classe Data creo i vari metodi

- ottieni RAM

```C#
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
```

- Ottieni ROM

```C#
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

                        return $"ROM Used: {(usedRom / totalRom) * 100} %, ROM Free: {(freeRom / totalRom) * 100} %, ROM Total: {totalRom} GB";
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
```

- Ottieni Info CPU

```C#
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
```

## ora lo si bilda sul Raspberry

### mi connetto via ssh

```bash
sshnome@inidrizzo_ip

sudo service ssh status #vedi se ssh è attivo
sudo service ssh start #attivi ssh
```

### Scarico dotnet passando via scp e lo scarico

```bash
sudo rm -r nomecartella #elimini cartella
rm nomefile #elimini file
```

### Comprimi La cartella scaricando WinRar e lo passi via SCP al Raspberry con estensuibe .zip

```bash
scp C:\percorso\del\tuo\file.txt pi@192.168.1.2:/percorso/di/destinazione/ #sposti file
scp -r C:\percorso\del\tuo\file.txt pi@192.168.1.2:/percorso/di/destinazione/ #sposti carte
```

### Decompressione file .zip passato da Windows

```bash
sudo apt-get install unzip #scarico file per estensione .zip
unzip nome_file.zip #decomprime file
unzip nome_file.zip -d percorso/di/destinazione #sposto dove mi serve
```

### Scarico Dotnet

```bash
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel STS #scarico dotnet nel raspberry

echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc

dotnet --version

chmod +x nomefile #do opzione di esecuzione al file
dotnet run #esegue il codice dotnet
```

### Infine programma funzionate alla fine

![lettura dati Raspberry](Immagini/Lettura_dati1.png)
![lettura dati Raspberry](Immagini/Lettura_dati2.png)

# Dati con il Protocollo MQTT

## Implementami la funzionalità per usare MQTT nel Raspberry

aggiungi pacchetti per l'uso del Protocollo MQTTnet

```bash
dotnet add package MQTTnet
```

Nel progetto di Visual studio scarichi MQTTnet per poterci lavorare

![MQTTnet](Immagini/Scaricamento_nuget_MQTT.png)

Nel file csproj aggiungi questa configurazione

```bash
nano Lettura_dati_Raspberry.csproj #modifici file e aggiungi la riga che segue
```

```xml
 <ItemGroup>
    <PackageReference Include="MQTTnet" Version="3.0.0" /><!-- qui includo il pacchetto MQTTnet con la versione 3.0.0 -->
</ItemGroup>
```

Infine il seguente comando per bildare il progetto

```bash
dotnet build #bilda il progetto
```

Creo una classe dove fare le implementazioni di cosa mandare via MQTT

```C#
internal class DataSend{
    // vari metodi
}
```

Implementazione degli using e Namespace della classe DataSend

```C#
using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using MQTTnet.Server;
namespace Lettura_dati_Raspberry;
```

Aggiungere le variabili della classe dove mettere le credenzili

```C#
private static IMqttClient _mqttClient;
private static string _brokerAddress = "indirizzo server";
private static int _brokerPort = 1883;
private static string _username = "nome utente";
private static string _password = "password";
private static MqttProtocolVersion _protocolVersion = MqttProtocolVersion.V311;
```

### Creo le funzioni che mi servono

- Per inizializzare il client (\_initclient)

```C#
private static void _initclient()
{
    if (_mqttClient == null)
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();
    }
}
```

- Per connetersi al Client (\_connectclient)

```C#
private static async Task _connectclient()
{
    _initclient();

    if (!_mqttClient.IsConnected)
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_brokerAddress, _brokerPort)
            .WithCredentials(_username, _password)
            .WithClientId(_clientId)
            .WithProtocolVersion(_protocolVersion)
            .Build();

        await _mqttClient.ConnectAsync(options);
    }
}
```

- per inviare i dati nel topic che mi interssa con il messaggio interessato (Send)

```C#
public static async Task Send(string topic, string message)
{
    await _connectclient();


    var mqttMessage = new MqttApplicationMessageBuilder()
        .WithTopic(topic)
        .WithPayload(message)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)  // Choose the appropriate QoS level
        .Build();

    await _mqttClient.PublishAsync(mqttMessage);
}
```

Visualizzo l'invio dei dati nei topic cambaindo coosi il programma 

```C#
static async Task Main(string[] args)
{
    Data data = new Data();

    // Stampare informazioni sulla RAM, ROM e CPU
    Console.WriteLine("Informazioni sulla RAM:");
    string RAM = data.GetRamInfo();
    await DataSend.Send("RAM", RAM);
    Console.WriteLine(RAM);

    Console.WriteLine("\nInformazioni sulla ROM:");
    string ROM = data.GetRomInfo();
    await DataSend.Send("ROM", ROM);
    Console.WriteLine(ROM);

    Console.WriteLine("\nInformazioni sulla CPU:");
    string CPU = data.GetCpuInfo();
    await DataSend.Send("CPU", CPU);
    Console.WriteLine(CPU);
}
```

creo una classe SensorData dove ottengo i vari tipi di dati da mandare via MQTT
```C#
internal class SensorData{
    // codice 
}
```

creo i vari attributi della classe SensorData
```C#
public string Name { get; set; } = "";
public string Value { get; set; } = "";
public string Unit { get; set; } = "";
```

modifico i vari metodi della classe Data con le impostazioni che mando i dati via MQTT

- funzione GetRamInfo 
```C#
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
```

- Funzione GetRomInfo
```C#
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
                        double freeRom = double.Parse(freeRomStr, CultureInfo.IariantCulture); // Convert GB to MB

                        double usedrompercentual = (usedRom / totalRom) * 100;
                        double freerompercentual = (freeRom / totalRom) * 100;

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
                                Value = totalRom.ToString(CultureInfo.InvariantCulture),
                                Unit = "GB"
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
```

- Funzione GetCpuInfo
```C#
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
                string currentTopic = null;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    // Split the line into key and value
                    string[] parts = line.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        // Determine the topic based on the key or content
                        if (key.ToLower() == "processor")
                        {
                            currentTopic = "CPU/processor";
                        }
                        else if (key.ToLower() == "model name")
                        {
                            currentTopic = "CPU/Model Name";
                        }
                        // Add more conditions for other topics as needed

                        // Create a SensorData object
                        if (currentTopic != null)
                        {
                            sensorData.Add(new SensorData
                            {
                                Name = currentTopic,
                                Value = value,
                                Unit = "" // You can customize this based on your needs
                            });
                        }

                        if (currentTopic != null)
                        {
                            sensorData.Add(new SensorData
                            {
                                Name = "CPU/key",
                                Value = key,
                                Unit = "" // You can customize this based on your needs
                            });
                        }

                        if (currentTopic != null)
                        {
                            sensorData.Add(new SensorData
                            {
                                Name = "CPU/value",
                                Value = value,
                                Unit = "" // You can customize this based on your needs
                            });
                        }
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
```

infine mettere dati perchè arrivino ogni minuto i nuovi dati
```C#
class Program
{
    static async Task Main(string[] args)
    {
        Data data = new Data();
        
        var timer = new Timer(async _ => // va vedere il tempo di timer
        {
            await DateperMinute(data);
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); // manda una voolta al minuto

        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();
    }

    static async Task DateperMinute(Data data)
    {
        // invia i dati via MQTT

        foreach (var ramData in data.GetRamInfo())
            await DataSend.Send(ramData.Name, ramData.Value);

        foreach (var romData in data.GetRomInfo())
            await DataSend.Send(romData.Name, romData.Value);

        foreach (var cpuData in data.GetCpuInfo())
            await DataSend.Send(cpuData.Name, cpuData.Value);

        Console.WriteLine("Data sent to MQTT."); // manda messaggio che ti dice se è arrivato il messaggi 
    }
}
```

infine per avviare il progetto

```bash
dotnet run #avvia il progetto
```

### Dati Arrivati al Client MQTT


- RAM

![Immagine dati RAM](Immagini/Ram.png)

- ROM

![Immagine dati ROM](Immagini/Rom.png)

- CPU

![Immagine dati CPU](Immagini/Cpu.png)

User And Stakeholders -> chiunque ha l'utilità di monitorare i dati