# visualizza i dati della RAM, ROM totale Libera e usata e CPU dI Raspberry

prima parte di configurazione degli using

- Management

![scaricamneto Nuget](Scaricamento_nuget.png)

intanta parte di configurazione degli using e namespace

```C#
using System;
namespace lettura_dati_Raspberry;
```

instanzio oggetto della classe Data

```C#
Data data = new Data();
```

Leggi le informazioni sulla RAM, ROM e CPU dal PC locale e stampa in automatico

```C#
class Program
{
    static void Main(string[] args)
    {
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
using System.Management;
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
    var processStartInfo = new System.Diagnostics.ProcessStartInfo
    {
        FileName = "free",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (var process = System.Diagnostics.Process.Start(processStartInfo))
    {
        using (var reader = process.StandardOutput)
        {
            string output = reader.ReadToEnd();

            // Estrai informazioni sulla RAM libera, usata e totale
            string[] lines = output.Split('\n');
            string[] values = lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            ulong totalRam = ulong.Parse(values[1]);
            ulong usedRam = ulong.Parse(values[2]);
            ulong freeRam = ulong.Parse(values[3]);

            return $"RAM Used: {usedRam / (1024 * 1024)} MB, RAM Free: {freeRam / (1024 * 1024)} MB, RAM Total: {totalRam / (1024 * 1024)} MB";
        }
    }
}
```

- Ottieni ROM

```C#
public string GetRomInfo()
{
    var processStartInfo = new System.Diagnostics.ProcessStartInfo
    {
        FileName = "df",
        Arguments = "-h /",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (var process = System.Diagnostics.Process.Start(processStartInfo))
    {
        using (var reader = process.StandardOutput)
        {
            string output = reader.ReadToEnd();

            // Estrai informazioni sulla ROM libera, usata e totale
            string[] lines = output.Split('\n');
            string[] values = lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            ulong totalRom = ulong.Parse(values[1]);
            ulong usedRom = ulong.Parse(values[2]);
            ulong freeRom = ulong.Parse(values[3]);

            return $"ROM Used: {usedRom / (1024 * 1024 * 1024)} GB, ROM Free: {freeRom / (1024 * 1024 * 1024)} GB, ROM Total: {totalRom / (1024 * 1024 * 1024)} GB";
        }
    }
}
```

- Ottieni Info CPU

```C#
public string GetCpuInfo()
{
    var processStartInfo = new System.Diagnostics.ProcessStartInfo
    {
        FileName = "cat",
        Arguments = "/proc/cpuinfo",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (var process = System.Diagnostics.Process.Start(processStartInfo))
    {
        using (var reader = process.StandardOutput)
        {
            string output = reader.ReadToEnd();
            return $"CPU Info:\n{output}";
        }
    }
}
```

## ora lo si bilda sul Raspberry

mi coonnettoo via ssh 

```bash
sshnome@inidrizzo_ip

sudo service ssh status #vedi se ssh è attivo
sudo service ssh start #attivi ssh
```

## Scarico dotnet passando via scp e lo scarico

```bash
scp C:\percorso\del\tuo\file.txt pi@192.168.1.2:/percorso/di/destinazione/ #sposti file 
scp -r C:\percorso\del\tuo\file.txt pi@192.168.1.2:/percorso/di/destinazione/ #sposti carte

curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel STS #scarico dotnet nel raspberry

echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc

dotnet --version

chmod +x nomefile #do opzione di esecuzione al file
dotnet run #esegue il codice dotnet 
```