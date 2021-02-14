using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SendLicense
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "winners.json"));
            var data = JsonSerializer.Deserialize<RuffleData[]>(json);
            RuffleMail mails = new RuffleMail();
            foreach (var m in data)
            {
                await mails.SendMail(m);
            }
        }
    }

}
