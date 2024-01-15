using System.Diagnostics;
namespace lettura_dati_Raspberry;

class Data
{
    public string GetRamInfo()
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
                string[] values = lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                ulong totalRam = ulong.Parse(values[1]);
                ulong usedRam = ulong.Parse(values[2]);
                ulong freeRam = ulong.Parse(values[3]);

                return $"RAM Used: {usedRam / (1024 * 1024)} MB, RAM Free: {freeRam / (1024 * 1024)} MB, RAM Total: {totalRam / (1024 * 1024)} MB";
            }
        }
    }

    public string GetRomInfo()
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
                string[] values = lines[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                ulong totalRom = ulong.Parse(values[1]);
                ulong usedRom = ulong.Parse(values[2]);
                ulong freeRom = ulong.Parse(values[3]);

                return $"ROM Used: {usedRom / (1024 * 1024 * 1024)} GB, ROM Free: {freeRom / (1024 * 1024 * 1024)} GB, ROM Total: {totalRom / (1024 * 1024 * 1024)} GB";
            }
        }
    }

    public string GetCpuInfo()
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
}
