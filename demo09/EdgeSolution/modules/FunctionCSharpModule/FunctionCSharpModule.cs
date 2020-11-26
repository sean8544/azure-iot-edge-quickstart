using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EdgeHub;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;






namespace Functions.Samples
{
    public static class FunctionCSharpModule

    {
        [FunctionName("FunctionCSharpModule")]
        public static async Task FilterMessageAndSendMessage(
                    [EdgeHubTrigger("input1")] Message messageReceived,
                    [EdgeHub(OutputName = "output1")] IAsyncCollector<Message> output,
                    ILogger logger)
        {
            byte[] messageBytes = messageReceived.GetBytes();
            var messageString = System.Text.Encoding.UTF8.GetString(messageBytes);

            if (!string.IsNullOrEmpty(messageString))
            {               
            
            //var dbstring=System.Environment.GetEnvironmentVariable("SQLConn");

             //var dbstring=System.Environment.GetEnvironmentVariable("Data Source =localhost;Initial Catalog =master;User Id = sa;Password =Strong!Passw0rd;");
               var dbstring="Data Source=SQLServerModule;Initial Catalog=master;User Id=SA;Password=Strong!Passw0rd;TrustServerCertificate=False;Connection Timeout=30;";
            
            string moduleID=messageReceived.ConnectionModuleId;
            string clientID=messageReceived.ConnectionDeviceId;
            Console.WriteLine(moduleID+"-"+clientID);
            try
            {
                 
                 var sensorData=new SensorData();
                 try
                    {
                     sensorData= JsonConvert.DeserializeObject<SensorData>(messageString);
                   
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"error{ex.Message}");
                    }
                
                using (SqlConnection con = new SqlConnection(dbstring))
                {
                    con.Open();
                    if (con.State ==   System.Data.ConnectionState.Open)
                    {
                    
                        string strCmd = $"insert into dbo.Telemetry(temperature,humidity,funcsavedt,deviceid) values ({sensorData.Temperature},{sensorData.Humidity},'{System.DateTime.Now}','{messageReceived.ConnectionDeviceId}' )";


                        SqlCommand sqlcmd = new SqlCommand(strCmd, con);
                        int   n = sqlcmd.ExecuteNonQuery();
                        if (n > 0)
                        {
                            logger.LogInformation("save to sql edge db successfully");
                        }
                        else
                        {
                            logger.LogError("save to sql edge db error");
                        }
                      
                }  
               con.Close();
               }
            }
            catch (Exception ex)
            {
               logger.LogInformation(ex.StackTrace);
            }


                logger.LogInformation("Info: Received one non-empty message");
                using (var pipeMessage = new Message(messageBytes))
                {
                    foreach (KeyValuePair<string, string> prop in messageReceived.Properties)
                    {
                        pipeMessage.Properties.Add(prop.Key, prop.Value);
                    }
                    await output.AddAsync(pipeMessage);
                    logger.LogInformation("Info: Piped out the message");
                }
            }
        }
    }




    /* public class Telemetry
        {
            public string deviceid{get;set;}

            public DateTime funcsavedt{get;set; }
            public double temperature { get; set; }

            public double humidity { get; set; }
        }
 */

        class SensorData
        {
            public double Temperature { get; set; }

            public double Humidity { get; set; }            
        }
}
