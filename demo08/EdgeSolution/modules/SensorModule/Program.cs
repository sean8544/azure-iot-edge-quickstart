namespace SensorModule
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Client.Transport.Mqtt;
    using Microsoft.Azure.Devices.Shared;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;


    class Program
    {
        static readonly Random Rnd = new Random();
        static TimeSpan messageDelay;
               

        public static int Main() => MainAsync().Result;

        static async Task<int> MainAsync()
        {
            Console.WriteLine("Sensor Module Main() started.");

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config/appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            messageDelay = configuration.GetValue("MessageDelay", TimeSpan.FromSeconds(5));
                            
            ModuleClient client =  await ModuleClient.CreateFromEnvironmentAsync(new ITransportSettings[] { new MqttTransportSettings(TransportType.Mqtt_Tcp_Only)});
            await client.OpenAsync();

            await SendCustomMsg(client,"Sensor Module Start");
            
            await SendEvents(client);
            Console.WriteLine("SimulatedSensor Main() finished.");
            return 0;
        }
  
        static async Task SendCustomMsg( ModuleClient moduleClient,string Msg)
        {                  
                    string sensorDtaBuffer = JsonConvert.SerializeObject(Msg);
                    var sensorEventMessage = new Message(Encoding.UTF8.GetBytes(sensorDtaBuffer));
                    sensorEventMessage.Properties.Add("MsgType", "CustomMsg");

                    await moduleClient.SendEventAsync("sensorOutput", sensorEventMessage);                 
                    
                    Console.WriteLine($"message sent:{sensorDtaBuffer}");              
           

        }
        
        static async Task SendEvents( ModuleClient moduleClient)
        {                   

            while (true)
            {       
                    var sensorData = new SensorData
                                        {
                                            Temperature=Rnd.Next(20,35),
                                            Humidity=Rnd.Next(55,80)
                                        };

                    string sensorDtaBuffer = JsonConvert.SerializeObject(sensorData);
                    var sensorEventMessage = new Message(Encoding.UTF8.GetBytes(sensorDtaBuffer));
                    sensorEventMessage.Properties.Add("MsgType", "Telemetry");
                    if(sensorData.Temperature>30)
                    {
                         sensorEventMessage.Properties.Add("Alert", "TemperatureAlert");
                    }

                    await moduleClient.SendEventAsync("sensorOutput", sensorEventMessage);                 
                    
                     Console.WriteLine($"message sent:{sensorDtaBuffer}");

                await Task.Delay(messageDelay);
            }

           
        }

      
     
     
        class SensorData
        {
            public double Temperature { get; set; }

            public double Humidity { get; set; }            
        }
    }
}
