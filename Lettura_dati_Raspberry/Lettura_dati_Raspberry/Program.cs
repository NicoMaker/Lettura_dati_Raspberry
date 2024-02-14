using Lettura_dati_Raspberry;
using System;
using System.Globalization;
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
        SensorData sensordata  = new SensorData();

        //  dati  System/SN
        sensordata.Name = "System/SN";
        sensordata.Value = mac;
        sensordata.ContentType = "Text";

        await DataSend.Send($"measures/@{mac}/{sensordata.Name}", sensordata, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
        Thread.Sleep(1000);


        // dati System/EC
        sensordata.Name = "System/EC";
        sensordata.Value = "RPI4"; // decido di che tipo è
        sensordata.ContentType = "Text";

        await DataSend.Send($"measures/@{mac}/{sensordata.Name}", sensordata, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());

        
        // dati System/uptime
        sensordata.Name = "System/uptime";
        sensordata.Value = "Online";
        sensordata.ContentType = "Text";


        // invia i dati via MQTT
        while (true)
        {
            string ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            await DataSend.Send($"measures/@{mac}/{sensordata.Name}", sensordata, ts);

            foreach (SensorData sensorData in data.GetRamInfo().Concat(data.GetRomInfo()).Concat(data.GetCpuInfo()))
                await DataSend.Send($"measures/@{mac}/{sensorData.Name}", sensorData, ts);

            Thread.Sleep(60000); // esegue ogni minuto
        }
    }
}