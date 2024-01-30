using Lettura_dati_Raspberry;
using System;
using System.Runtime.Intrinsics.Arm;
namespace lettura_dati_Raspberry;
class Program
{
    static async Task Main(string[] args)
    {
        Data data = new Data();
        

        var timer = new Timer(async _ =>
        {
            await DateperMinute(data);
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

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

        Console.WriteLine("Data sent to MQTT.");
    }
}