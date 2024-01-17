# visualizza i dati della RAM, ROM totale Libera e usata e CPU dI Raspberry

prima parte di configurazione degli using

- Management 

![scaricamneto Nuget](Scaricamento_nuget.png)


intanto parte di configurazione degli using e namespace


```C#
using System;
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

### mi coonnetto via ssh

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

![lettura dati Raspberry](Lettura_dati1.png)
![lettura dati Raspberry](Lettura_dati2.png)


# Dati con MQTT

## Implementami la funzionalità per usare MQTT nel Raspberry

aggiungi pacchetti MQTTnet

```bash
dotnet add package MQTTnet
```

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

Implementazione degli using e Namespace della classe Data

```bash
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace lettura_dati_Raspberry;
```

Creo diversi metodi dentro la classe Data

- PublishMqttMessageAsync -> per publiccare i vari valori della RAM ROM e CPU

```C#
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
```

- PublishRamInfoMqttAsyn -> publica i dati della RAM

```C#
public async Task PublishRamInfoMqttAsync()
{
    string ramInfo = GetRamInfo();
    await PublishMqttMessageAsync("ram_info_topic", ramInfo);
}
```

- PublishRomInfoMqttAsync -> publica i dati della ROM

```C#
public async Task PublishRomInfoMqttAsync()
{
    string romInfo = GetRomInfo();
    await PublishMqttMessageAsync("rom_info_topic", romInfo);
}
```

- PublishCpuInfoMqttAsyn -> publica i dati della CPU

```C#
public async Task PublishCpuInfoMqttAsync()
{
    string cpuInfo = GetCpuInfo();
    await PublishMqttMessageAsync("cpu_info_topic", cpuInfo);
}
```

infine per avviare il progetto 
```bash
dotnet run #avvia il progetto
```