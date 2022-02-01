using System;
using System.Timers;
using System.Text;
using System.Collections.Generic;
using RestSharp;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Reflection;
using MySql.Data.MySqlClient;
public class Timer1
{
    // private static Timer aTimer = new System.Timers.Timer(86400000);
    private static Timer aTimer = new System.Timers.Timer(60000);

    public static void Main()
    {
        aTimer.Elapsed += new ElapsedEventHandler(ExecuteEveryDayMethod);
        aTimer.Enabled = true;

        Console.WriteLine("Press the Enter key to exit the program.");
        Console.ReadLine();

    }
   
    //Specify what you want to happen when the Elapsed event is 
    // raised.
    private static void ExecuteEveryDayMethod(object source, ElapsedEventArgs e)
    {
        try
        {
            string cs = @"server=localhost;userid=root;database=currdb";

            using var con = new MySqlConnection(cs);
            con.Open();
            //Console.WriteLine($"MySQL version : {con.ServerVersion}");

            using (var client = new HttpClient())
            {

                var endpoint = new Uri("http://data.fixer.io/api/latest?access_key=9ef7187d085b36f641fe0384ae182401");
                var result = client.GetAsync(endpoint).Result;
                var jsonString = result.Content.ReadAsStringAsync().Result;

                JObject obj = JsonConvert.DeserializeObject<JObject>(jsonString);
                JObject innerObj = obj["rates"] as JObject;

                string date = DateTime.Now.Date.ToString("yyyy-MM-dd");
                foreach (var itm in innerObj)
                {
                    string sql = "INSERT INTO currdb.tbldailycurrencyrate VALUES ('" + null + "','" + date + "','" + itm.Key + "','" + itm.Value + "')";

                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    cmd.ExecuteNonQuery();

                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
}