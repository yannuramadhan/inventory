using inventory.Manager;
using Npgsql;
using System.Data;
using inventory.Data.Models;
using inventory.Data.Controllers;

namespace inventory.Libs
{
    public class lMessage
    {
        public string GetMessage(string code)
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("message.json");

            var config = builder.Build();
            return config.GetSection(code).Value.ToString();
        }

        public string GetMessageCode(int code)
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("message_code.json");

            var config = builder.Build();
            return config.GetSection("" + code).Value.ToString();
        }
    }
}
