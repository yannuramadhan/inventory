using inventory.Libs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Npgsql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace inventory.Data.Controllers
{
    public class BaseController
    {
        private lDbConn dbconn = new lDbConn();
        private lMessage mc = new lMessage();
        private int timeout = 5;

        public string execExtAPIPost(string api, string path, string json)
        {
            var WebAPIURL = dbconn.domainGetApi(api);
            string requestStr = WebAPIURL + path;

            var serviceProvider = new ServiceCollection().AddHttpClient()
            .Configure<HttpClientFactoryOptions>("HttpClientWithSSLUntrusted", options =>
                options.HttpMessageHandlerBuilderActions.Add(builder =>
                    builder.PrimaryHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
                    }))
            .BuildServiceProvider();
            var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            HttpClient client = _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            client.BaseAddress = new Uri(requestStr);
            client.Timeout = TimeSpan.FromMinutes(timeout);
            var contentData = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(requestStr, contentData).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            client.Dispose();
            return result;
        }

        public string execExtAPIPostV2(string url, string json)
        {
            var serviceProvider = new ServiceCollection().AddHttpClient()
            .Configure<HttpClientFactoryOptions>("HttpClientWithSSLUntrusted", options =>
                options.HttpMessageHandlerBuilderActions.Add(builder =>
                    builder.PrimaryHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
                    }))
            .BuildServiceProvider();
            var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            HttpClient client = _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromMinutes(timeout);
            var contentData = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, contentData).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            client.Dispose();
            return result;
        }

        public string execExtAPIGet(string url, string header)
        {
            var serviceProvider = new ServiceCollection().AddHttpClient()
            .Configure<HttpClientFactoryOptions>("HttpClientWithSSLUntrusted", options =>
                options.HttpMessageHandlerBuilderActions.Add(builder =>
                    builder.PrimaryHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
                    }))
            .BuildServiceProvider();
            var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            HttpClient client = _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromMinutes(timeout);

            JObject jHeader = JObject.Parse(header);
            foreach (var x in jHeader)
            {
                if (x.ToString() != "" && x.Key.ToString().ToLower() != "content-type")
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    client.DefaultRequestHeaders.Add(x.Key.ToString(), x.Value.ToString());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            HttpResponseMessage response = client.GetAsync(url).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            client.Dispose();

            if (result != "")
                if (!response.IsSuccessStatusCode)
                    result = "";
            return result;
        }

        public List<dynamic> GetDataObj(NpgsqlDataReader dr)
        {
            var retObject = new List<dynamic>();
            while (dr.Read())
            {
                var dataRow = new ExpandoObject() as IDictionary<string, object>;
                for (int i = 0; i < dr.FieldCount; i++)
                {
#pragma warning disable CS8604 // Possible null reference argument.
                    dataRow.Add(
                               dr.GetName(i),
                               dr.IsDBNull(i) ? null : dr[i] // use null instead of {}
                       );
#pragma warning restore CS8604 // Possible null reference argument.
                }
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                retObject.Add((ExpandoObject)dataRow);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }

            return retObject;
        }

        public string execExtAPIGetWithToken(string api, string path, string credential)
        {
            var WebAPIURL = dbconn.domainGetApi(api);
            string requestStr = WebAPIURL + path;

            var serviceProvider = new ServiceCollection().AddHttpClient()
           .Configure<HttpClientFactoryOptions>("HttpClientWithSSLUntrusted", options =>
               options.HttpMessageHandlerBuilderActions.Add(builder =>
                   builder.PrimaryHandler = new HttpClientHandler
                   {
                       ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
                   }))
           .BuildServiceProvider();

            var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            HttpClient client = _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            client.BaseAddress = new Uri(requestStr);
            client.Timeout = TimeSpan.FromMinutes(timeout);

            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }
            else
            {
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }

            HttpResponseMessage response = client.GetAsync(requestStr).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            client.Dispose();
            return result;
        }

        public int execExtAPIGetWithTokenReturnStatusCode(string api, string path, string credential)
        {
            var WebAPIURL = dbconn.domainGetApi(api);
            string requestStr = WebAPIURL + path;



            var serviceProvider = new ServiceCollection().AddHttpClient()
            .Configure<HttpClientFactoryOptions>("HttpClientWithSSLUntrusted", options =>
            options.HttpMessageHandlerBuilderActions.Add(builder =>
            builder.PrimaryHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
            }))
            .BuildServiceProvider();
            var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            HttpClient client = _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            client.BaseAddress = new Uri(requestStr);
            client.Timeout = TimeSpan.FromMinutes(timeout);
            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + credential);
            }
            else
            {
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }

            HttpResponseMessage response = client.GetAsync(requestStr).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            client.Dispose();
            return (int)response.StatusCode;
        }

        public string execExtAPIPostWithToken(string api, string path, string json, string credential)
        {
            string result = "";
            #region call other api version v.1 di remark
            //var WebAPIURL = dbconn.domainGetApi(api);
            //string requestStr = WebAPIURL + path;

            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Authorization", credential);
            //var contentData = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            //contentData.Headers.Add("Authorization", credential);

            //HttpResponseMessage response = client.PostAsync(requestStr, contentData).Result;
            //result = response.Content.ReadAsStringAsync().Result;
            #endregion
            result = execExtAPIPostWithTokenAwait(api, path, json, credential).Result;

            return result;
        }

        public string execExtAPIPutWithToken(string api, string path, string json, string credential)
        {
            string result = "";
            #region call other api version v.1 di remark
            //var WebAPIURL = dbconn.domainGetApi(api);
            //string requestStr = WebAPIURL + path;

            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Authorization", credential);
            //var contentData = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            //contentData.Headers.Add("Authorization", credential);

            //HttpResponseMessage response = client.PostAsync(requestStr, contentData).Result;
            //result = response.Content.ReadAsStringAsync().Result;
            #endregion
            result = execExtAPIPutWithTokenAwait(api, path, json, credential).Result;

            return result;
        }

        public string execExtAPIPostWithTokenNoHeaderValidation(string api, string path, string json, string credential)
        {
            string result = "";
            #region call other api version v.1 di remark
            //var WebAPIURL = dbconn.domainGetApi(api);
            //string requestStr = WebAPIURL + path;

            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Authorization", credential);
            //var contentData = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            //contentData.Headers.Add("Authorization", credential);

            //HttpResponseMessage response = client.PostAsync(requestStr, contentData).Result;
            //result = response.Content.ReadAsStringAsync().Result;
            #endregion
            result = execExtAPIPostWithTokenAwaitNoValidationHeader(api, path, json, credential).Result;

            return result;
        }

        public async Task<string> execExtAPIPostWithTokenAwait(string api, string path, string json, string credential)
        {
            var WebAPIURL = dbconn.domainGetApi(api);
            string requestStr = WebAPIURL + path;

            HttpClient client = new HttpClient(
                 new HttpClientHandler()
                 {
                     ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                 }
            );

            client.BaseAddress = new Uri(requestStr);
            client.Timeout = TimeSpan.FromMinutes(timeout);

            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }
            else
            {
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }

            var contentData = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(requestStr, contentData);
            string result = await response.Content.ReadAsStringAsync();

            client.Dispose();
            return result;
        }

        public async Task<string> execExtAPIPutWithTokenAwait(string api, string path, string json, string credential)
        {
            var WebAPIURL = dbconn.domainGetApi(api);
            string requestStr = WebAPIURL + path;

            HttpClient client = new HttpClient(
                 new HttpClientHandler()
                 {
                     ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                 }
            );

            client.BaseAddress = new Uri(requestStr);
            client.Timeout = TimeSpan.FromMinutes(timeout);

            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }
            else
            {
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }

            var contentData = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(requestStr, contentData);
            string result = await response.Content.ReadAsStringAsync();

            client.Dispose();
            return result;
        }

        public async Task<string> execExtAPIPostWithTokenAwaitNoValidationHeader(string api, string path, string json, string credential)
        {
            #region call others api version : v.3
            string result = "";
            var WebAPIURL = dbconn.domainGetApi(api);
            string requestStr = WebAPIURL + path;
            HttpClient client = new HttpClient(
                 new HttpClientHandler()
                 {
                     ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                 }
            );

            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", credential);
            }
            else
            {
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", credential);
            }
            var contentData = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(requestStr, contentData);
            result = await response.Content.ReadAsStringAsync();
            #endregion

            return result;
        }

        public string execExtAPIPostWithTokenUrlencode(string api, string path, string json, string credential)
        {
            string result = "";
            #region call other api version v.1 di remark
            //var WebAPIURL = dbconn.domainGetApi(api);
            //string requestStr = WebAPIURL + path;

            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Authorization", credential);
            //var contentData = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            //contentData.Headers.Add("Authorization", credential);

            //HttpResponseMessage response = client.PostAsync(requestStr, contentData).Result;
            //result = response.Content.ReadAsStringAsync().Result;
            #endregion
            result = execExtAPIPostWithTokenAwaitUrlencode(api, path, json, credential).Result;

            return result;
        }

        public async Task<string> execExtAPIPostWithTokenAwaitUrlencode(string api, string path, string body_urlencode, string credential)
        {
            #region call others api version : v.3
            string result = "";
            var WebAPIURL = dbconn.domainGetApi(api);
            string requestStr = WebAPIURL + path;

            HttpClient client = new HttpClient(
                new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                }
            );

            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }
            else
            {
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }

            var contentData = new StringContent(body_urlencode, Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = await client.PostAsync(requestStr, contentData);
            result = await response.Content.ReadAsStringAsync();
            #endregion

            return result;
        }

        public string execExtAPIPostWithCustomHeader(string api, string path, string custom_header, string json, string credential)
        {
            string result = "";
            #region call other api version v.1 di remark
            //var WebAPIURL = dbconn.domainGetApi(api);
            //string requestStr = WebAPIURL + path;

            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Authorization", credential);
            //var contentData = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            //contentData.Headers.Add("Authorization", credential);

            //HttpResponseMessage response = client.PostAsync(requestStr, contentData).Result;
            //result = response.Content.ReadAsStringAsync().Result;
            #endregion
            result = execExtAPIPostCustomHeader(api, path, custom_header, json, credential).Result;

            return result;
        }

        public async Task<string> execExtAPIPostCustomHeader(string api, string path, string custom_header, string json, string credential)
        {
            #region call others api version : v.3
            string result = "";
            var WebAPIURL = dbconn.domainGetApi(api);
            string requestStr = WebAPIURL + path;

            HttpClient client = new HttpClient(
                 new HttpClientHandler()
                 {
                     ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                 }
            );

            if (!client.DefaultRequestHeaders.Contains(custom_header))
            {
                client.DefaultRequestHeaders.Add(custom_header, credential);
            }
            else
            {
                client.DefaultRequestHeaders.Remove(custom_header);
                client.DefaultRequestHeaders.Add(custom_header, credential);
            }
            var contentData = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(requestStr, contentData);
            result = await response.Content.ReadAsStringAsync();
            #endregion

            return result;
        }

        public List<dynamic> GetDataObjPgsql(NpgsqlDataReader dr)
        {
            var retObject = new List<dynamic>();
            while (dr.Read())
            {
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                var dataRow = new ExpandoObject() as IDictionary<string, object>;
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    dataRow.Add(
                               dr.GetName(i),
                               dr.IsDBNull(i) ? null : dr[i] // use null instead of {}
                       );
                }
                retObject.Add((ExpandoObject)dataRow);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning restore CS8604 // Possible null reference argument.
            }

            return retObject;
        }

        public List<dynamic> ExecSqlWithReturnCustomSplit(string strname, string cstsplit, string schema, string spname, params string[] list)
        {
            var retObject = new List<dynamic>();
            StringBuilder sb = new StringBuilder();
            var conn = dbconn.constringList(strname);

            spname = schema + "." + spname;
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            nconn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;

            if (list != null && list.Count() > 0)
            {
                foreach (var item in list)
                {
                    var pars = item.Split(cstsplit);

                    if (pars.Count() > 2)
                    {
                        if (pars[2] == "i")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), Convert.ToInt32(pars[1]));
                        }
                        else if (pars[2] == "s")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), Convert.ToString(pars[1]));
                        }
                        else if (pars[2] == "d")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), Convert.ToDecimal(pars[1]));
                        }
                        else if (pars[2] == "dt")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), DateTime.ParseExact(pars[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                        }
                        else if (pars[2] == "dtb")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                        }
                        else if (pars[2] == "b")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), Convert.ToBoolean(pars[1]));
                        }
                        else if (pars[2] == "bg")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), Convert.ToInt64(pars[1]));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), pars[1]);
                        }
                    }
                    else if (pars.Count() > 1)
                    {
                        cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), pars[1]);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), pars[0]);
                    }
                }
            }

            try
            {
                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr == null || dr.FieldCount == 0)
                {
                    nconn.Close();
                    if (nconn.State.Equals(ConnectionState.Open))
                    {
                        nconn.Close();
                    }
                    NpgsqlConnection.ClearPool(nconn);
                    return retObject;
                }

                retObject = GetDataObjPgsql(dr);
                nconn.Close();
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);

            }
            catch (Exception ex)
            {
                var err = ex.Message;
                nconn.Close();
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }

            return retObject;
        }

        public string ExecSqlSplitWithoutReturn(string strname, string cstsplit, string schema, string spname, params string[] list)
        {
            var strReturn = "";
            StringBuilder sb = new StringBuilder();
            var conn = dbconn.constringList(strname);

            spname = schema + "." + spname;
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            nconn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;

            if (list != null && list.Count() > 0)
            {
                foreach (var item in list)
                {
                    var pars = item.Split(cstsplit);

                    if (pars.Count() > 2)
                    {
                        if (pars[2] == "i")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), Convert.ToInt32(pars[1]));
                        }
                        else if (pars[2] == "s")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), Convert.ToString(pars[1]));
                        }
                        else if (pars[2] == "d")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), Convert.ToDecimal(pars[1]));
                        }
                        else if (pars[2] == "dt")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), DateTime.ParseExact(pars[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                        }
                        else if (pars[2] == "b")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), Convert.ToBoolean(pars[1]));
                        }
                        else if (pars[2] == "bg")
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), Convert.ToInt64(pars[1]));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), pars[1]);
                        }
                    }
                    else if (pars.Count() > 1)
                    {
                        cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), pars[1]);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue(pars[0].ToString().Replace("@", "p_"), pars[0]);
                    }
                }
            }

            try
            {
                cmd.ExecuteNonQuery();
                strReturn = mc.GetMessage("execdb_success");
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                nconn.Close();
                NpgsqlConnection.ClearPool(nconn);
                strReturn = mc.GetMessage("execdb_failed");
            }

            nconn.Close();
            NpgsqlConnection.ClearPool(nconn);
            return strReturn;
        }

        public List<dynamic> getDataToObject(string strname, string spname, params string[] list)
        {
            var conn = dbconn.constringList(strname);
            StringBuilder sb = new StringBuilder();
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            var retObject = new List<dynamic>();

            nconn.Open();
            //NpgsqlTransaction tran = nconn.BeginTransaction();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;

            if (list != null && list.Count() > 0)
            {
                foreach (var item in list)
                {
                    var pars = item.Split(',');

                    if (pars.Count() > 2)
                    {
                        if (pars[2] == "i")
                        {
                            cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                        }
                        else if (pars[2] == "s")
                        {
                            cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                        }
                        else if (pars[2] == "d")
                        {
                            cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                        }
                        else if (pars[2] == "b")
                        {
                            cmd.Parameters.AddWithValue(pars[0], Convert.ToBoolean(pars[1]));
                        }
                        else if (pars[2] == "bg")
                        {
                            cmd.Parameters.AddWithValue(pars[0], Convert.ToInt64(pars[1]));
                        }
                        else if (pars[2] == "dt")
                        {
                            cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                    }
                    else if (pars.Count() > 1)
                    {
                        cmd.Parameters.AddWithValue(pars[0], pars[1]);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue(pars[0], pars[0]);
                    }
                }
            }

            NpgsqlDataReader dr = cmd.ExecuteReader();

            if (dr == null || dr.FieldCount == 0)
            {

                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
                return retObject;
            }

            retObject = GetDataObj(dr);

            if (nconn.State.Equals(ConnectionState.Open))
            {
                nconn.Close();
            }
            NpgsqlConnection.ClearPool(nconn);
            return retObject;
        }

        public void execSqlWithExecption(string strname, string spname, params string[] list)
        {
            var conn = dbconn.constringList(strname);
            string message = "";
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            nconn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;
            if (list != null && list.Count() > 0)
            {
                foreach (var item in list)
                {
                    var pars = item.Split(',');

                    if (pars.Count() > 2)
                    {
                        if (pars[2] == "i")
                        {
                            cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                        }
                        else if (pars[2] == "s")
                        {
                            cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                        }
                        else if (pars[2] == "d")
                        {
                            cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                        }
                        else if (pars[2] == "dt")
                        {
                            cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                        }
                        else if (pars[2] == "b")
                        {
                            cmd.Parameters.AddWithValue(pars[0], Convert.ToBoolean(pars[1]));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                    }
                    else if (pars.Count() > 1)
                    {
                        cmd.Parameters.AddWithValue(pars[0], pars[1]);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue(pars[0], pars[0]);
                    }
                }
            }
            try
            {
                cmd.ExecuteNonQuery();
                message = "success";
            }
            catch (NpgsqlException e)
            {
                message = e.Message;
            }
            finally
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }
            //return message;
        }

        public string execExtAPIPostExToken(string api, string path, string json)
        {
            var WebAPIURL = dbconn.domainGetApi(api);
            string requestStr = WebAPIURL + path;

            var serviceProvider = new ServiceCollection().AddHttpClient()
            .Configure<HttpClientFactoryOptions>("HttpClientWithSSLUntrusted", options =>
                options.HttpMessageHandlerBuilderActions.Add(builder =>
                    builder.PrimaryHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
                    }))
            .BuildServiceProvider();

            var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            HttpClient client = _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            client.BaseAddress = new Uri(requestStr);
            client.Timeout = TimeSpan.FromMinutes(timeout);

            var contentData = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(requestStr, contentData).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            client.Dispose();
            return result;
        }

        internal void SaveFile(JObject json, string rshid, string applname, string group, string prefix)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var folder = "Files/" + group + "/";
            var filepath = folder + today;

            var filename = "";
            if (string.IsNullOrWhiteSpace(rshid))
            {
                filename = prefix + applname + ".json";
            }
            else
            {
                filename = prefix + rshid + "_" + applname + ".json";
            }

            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            var filenamePath = filepath + "/" + filename;
            FileInfo fi = new FileInfo(filenamePath);

            //check 1st check file jika sudah ada di delete dan generate baru
            if (fi.Exists)
            {
                fi.Delete();
            }

            // Create a new file     
            using (FileStream fs = fi.Create())
            {
                byte[] txt = new UTF8Encoding(true).GetBytes("New file.");
                fs.Write(txt, 0, txt.Length);
                byte[] author = new UTF8Encoding(true).GetBytes("idxteam");
                fs.Write(author, 0, author.Length);
            }
            File.WriteAllText(filenamePath, json.ToString());
        }

        public string PostExternalServiceNoToken(string url, string json)
        {
            string requestStr = url;

            HttpClient client = new HttpClient(
                 new HttpClientHandler()
                 {
                     ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                 }
            );

            client.BaseAddress = new Uri(requestStr);
            client.Timeout = TimeSpan.FromMinutes(timeout);

            var contentData = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(requestStr, contentData).Result;
            string result = response.Content.ReadAsStringAsync().Result;

            client.Dispose();
            return result;
        }

        public string execExtAPIPostWithTokenAsync(string api, string path, string json, string credential)
        {
            #region call others api async
            string result = "";
            result = CallPostWithTokenAsync(api, path, json, credential);
            #endregion
            return result;
        }

        public string execExtAPIPostExTokenAsync(string api, string path, string json)
        {
            string result = "";
            #region call others api async
            result = CallPostWithoutTokenAsync(api, path, json);
            #endregion

            return result;
        }

        public string CallPostWithTokenAsync(string api, string path, string json, string credential)
        {
            string strOutput = "";
            string method = "POST";
            HttpWebRequest webRequest = GenerateWebRequestWithCredential(method, api, path, credential);

            //convert request body to bitye
            byte[] postBytes = Encoding.UTF8.GetBytes(json.ToString());
            using (Stream stream = webRequest.GetRequestStream())
            {
                stream.Write(postBytes);
            }
            webRequest.GetResponseAsync();

            return strOutput;
        }

        internal string CallPostWithoutTokenAsync(string api, string path, string json)
        {
            string strOutput = "";
            string method = "POST";
            HttpWebRequest webRequest = GenerateWebRequestWithoutCredential(method, api, path);

            //convert request body to bitye
            byte[] postBytes = Encoding.UTF8.GetBytes(json.ToString());
            using (Stream stream = webRequest.GetRequestStream())
            {
                stream.Write(postBytes);
            }

            webRequest.GetResponseAsync();

            return strOutput;
        }

        internal HttpWebRequest GenerateWebRequestWithCredential(string method, string api, string path, string credential)
        {
            var WebAPIURL = dbconn.domainGetApi(api);
            string url = WebAPIURL + path;

            byte[] credentialBuffer = new UTF8Encoding().GetBytes(credential);
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            webRequest.Headers.Add("Authorization", credential);
            webRequest.ContentType = "application/json;charset=\"utf-8\"";
            webRequest.Accept = "application/json";
            webRequest.Method = method;
            return webRequest;
        }

        internal HttpWebRequest GenerateWebRequestWithoutCredential(string method, string api, string path)
        {
            var WebAPIURL = dbconn.domainGetApi(api);
            string url = WebAPIURL + path;

#pragma warning disable SYSLIB0014 // Type or member is obsolete
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            webRequest.ContentType = "application/json;charset=\"utf-8\"";
            webRequest.Accept = "application/json";
            webRequest.Method = method;
            return webRequest;
        }

    }
}
