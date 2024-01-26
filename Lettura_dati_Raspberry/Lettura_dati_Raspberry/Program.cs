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
}