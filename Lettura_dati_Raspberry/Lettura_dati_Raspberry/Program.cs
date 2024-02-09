using Lettura_dati_Raspberry;
using System;
using System.Runtime.Intrinsics.Arm;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace lettura_dati_Raspberry;
class Program
{
    static async Task Main(string[] args)
    {
        Data data = new Data();
        List<SensorData> macs = data.GetMacAddress();

        if (macs.Count == 0) throw new Exception("No MacAddress found");

        Task dataTask = Task.Run(() => DateperMinute(data, macs[0].Value)); // richamo la funzione 

        while (true)
            Thread.Sleep(60000); // manda i messaggi al mqtt anche se il programma non è avviato ma la macchina deve essere accesa

    }

    static async Task DateperMinute(Data data, string mac)
    {
        // invia i dati via MQTT
        while (true)
        {
            foreach (SensorData sensorData in data.GetRamInfo().Concat(data.GetRomInfo()).Concat(data.GetCpuInfo()))
                await DataSend.Send($"measures/@{mac}/{sensorData.Name}", sensorData.Value);

            Thread.Sleep(60000); // esegue ogni minuto
        }
    }
}