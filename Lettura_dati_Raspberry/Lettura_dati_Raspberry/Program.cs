using lettura_dati_Raspberry;

Data data = new Data();

string ramInfo = data.GetRamInfo();
string romInfo = data.GetRomInfo();
string cpuInfo = data.GetCpuInfo();

Console.WriteLine(ramInfo);
Console.WriteLine(romInfo);
Console.WriteLine(cpuInfo);