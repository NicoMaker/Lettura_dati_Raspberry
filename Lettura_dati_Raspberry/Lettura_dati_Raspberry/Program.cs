using Lettura_dati_Raspberry;
using System;
namespace lettura_dati_Raspberry;
class Program
{
    static async Task Main(string[] args)
    {
        Data data = new Data();

        // Stampare informazioni sulla RAM, ROM e CPU
        Console.WriteLine("Informazioni sulla RAM:");
        Console.WriteLine(data.GetRamInfo());

        string RAM = data.GetRamInfo();
        DataSend.Send("RAM", RAM);

        Console.WriteLine("\nInformazioni sulla ROM:");
        Console.WriteLine(data.GetRomInfo());

        string ROM = data.GetRamInfo();
        DataSend.Send("ROM", ROM);

        Console.WriteLine("\nInformazioni sulla CPU:");
        Console.WriteLine(data.GetCpuInfo());

        string  CPU = data.GetRamInfo();
        DataSend.Send("CPU", CPU);
    }
}