# visualizza i dati della RAM, ROM totale Libera e usata e CPU dI Raspberry

prima parte di configurazione degli using

- Management

![scaricamneto Nuget](Scaricamento_nuget.png)

intanta parte di configurazione degli using

```C#
using lettura_dati_Raspberry;
```

instanzio oggetto della classe Data

```C#
Data data = new Data();
```

Leggi le informazioni sulla RAM, ROM e CPU dal PC locale

```C#
string ramInfo = data.GetRamInfo();
string romInfo = data.GetRomInfo();
string cpuInfo = data.GetCpuInfo();
```

Stampare le informazioni sulla console

```c#
Console.WriteLine(ramInfo);
Console.WriteLine(romInfo);
Console.WriteLine(cpuInfo);
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