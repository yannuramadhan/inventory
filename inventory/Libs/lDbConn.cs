using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace inventory.Libs
{
    public class lDbConn
    {
        private lConvert lc = new lConvert();

        #region SettingDB

        public string constringName(string cstr)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return "" + config.GetSection("DBConstring:" + cstr).Value.ToString();
        }

        public string constringList(string strname)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();
            var configPass = lc.Decrypt(config.GetSection("configPass:passwordDB").Value.ToString());
            //var configPass = lc.Encrypt(config.GetSection("configPass:passwordDB").Value.ToString());
            var configDB = config.GetSection("DbContextSettings:" + strname).Value.ToString();
            var repPass = configDB.Replace("{pass}", configPass);

            return "" + repPass;
        }

        public string conStringDynamic(string database)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();
            var configPass = lc.Decrypt(config.GetSection("Encript:passDB").Value.ToString());

            var configDB = config.GetSection("DbContextSettings:ConnectionString_Dynamic").Value.ToString();
            var repDB = configDB.Replace("{database}", database);
            var repPass = repDB.Replace("{pass}", configPass);

            return "" + repPass;
        }

        #endregion

        #region SettingGateaway

        public string SettingSms(string param)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return "" + config.GetSection("SettingSms:" + param).Value.ToString();
        }
        public string SettingSmsV2(string param)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return "" + config.GetSection("SettingSms_v2:" + param).Value.ToString();
        }

        public string SettingEmail(string param)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return config.GetSection("SettingEmail:" + param).Value.ToString();
        }

        public string TemplateEmail(string param)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return config.GetSection("EmailTemplate:" + param).Value.ToString();
        }

        public string TemplateSMS(string param)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return config.GetSection("SMSTemplate:" + param).Value.ToString();
        }

        public string SettingOtp(string param)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return config.GetSection("SettingOtp:" + param).Value.ToString();
        }

        #endregion

        #region SettingLogin

        public string SettingLogin(string param)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return "" + config.GetSection("SettingLogin:" + param).Value.ToString();
        }

        #endregion

        #region SettingDomain

        public string domainGetTokenCredential(string param)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return config.GetSection("TokenAuthenticationSuperApp:" + param).Value.ToString();
        }

        public string domainGetApi(string api)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();
            return "" + config.GetSection("APISettings:" + api).Value.ToString();
        }

        #endregion

        #region SettingIntegration

        public string SettingIntegration(string param)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return config.GetSection("SettingIntegration:" + param).Value.ToString();
        }
        public string Defaultkantorcabang(string cstr)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return "" + config.GetSection(cstr).Value.ToString();
        }

        public string ANSstatus(string cstr)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return "" + config.GetSection(cstr).Value.ToString();
        }

        #endregion

        #region InfoReference
        public string SettingInfoReference(string param)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return config.GetSection("InfoReference:" + param).Value.ToString();
        }

        #endregion
    }
}
