using Lettura_dati_Raspberry;
using System;
namespace lettura_dati_Raspberry;
class Program
{
    static async Task Main(string[] args)
    {
        Data data = new Data();
        
        List <SensorData> Ram = new List <SensorData>();
        List <SensorData> Rom = new List <SensorData>();
        List <SensorData> Cpu = new List <SensorData>();

        Ram = data.GetRamInfo();
        Rom = data.GetRomInfo();
        Cpu = data.GetCpuInfo(); 

        // invia i dati via MQTT

        for(int i = 0; i < Ram.Count; i++)
            await DataSend.Send(Ram[i].Name, Ram[i].Value);

        for (int i = 0; i < Rom.Count; i++)
            await DataSend.Send(Rom[i].Name, Rom[i].Value);

        for (int i = 0; i < Cpu.Count; i++)
            await DataSend.Send(Cpu[i].Name, Cpu[i].Value);
    }
}