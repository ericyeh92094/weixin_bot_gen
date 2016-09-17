using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;
using System.Net;

namespace ModelGen
{
    class LUISApp
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Culture { get; set; }
        public bool Active { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string PublishDate { get; set; }
        public string URL { get; set; }
        public string AuthKey { get; set; }
        public int NumberOfIntents { get; set; }
        public int NumberOfEntities { get; set; }
        public bool IsTrained { get; set; }
    }

    public class EntityLabel
    {
        public string EntityType { get; set; }
        public int StartToken { get; set; }
        public int EndToken { get; set; }
    }

    public class Label
    {
        public string ExampleText { get; set; }
        public string SelectedIntentName { get; set; }
        public List<EntityLabel> EntityLabels { get; set; }
    }

    public class Intent
    {
        public string Name { get; set; }
    }

    class LUISGen
    {
        public static string[] appNames, appIds, tipIds;
        public static string appNamePrefix = "BOTGEN_";
 
        public static ProgressBar progBar { get; set; }

        public static void initModelVar(int model_count)
        {
            appNames = new string[model_count];
            appIds = new string[model_count];
        }
        public static async Task<int> GenerateModels(DataTable dtCSV)
        {
            int totalrow = dtCSV.Rows.Count;
            int model_count = Convert.ToInt16(Math.Ceiling((double)((double)totalrow / 19.0))) + 1; //include main model

            initModelVar(model_count);
            tipIds = new string[totalrow];

            progBar.Maximum = totalrow * 2;
            progBar.Value = 1;

            appIds[0] = await GenerateMainModel(dtCSV);
             
            if (appIds[0] != null)
            {
                int intentidx = 0, modelidx = 1;
                appNames[modelidx] = appNamePrefix + String.Format("MODEL{0}", modelidx);
                string appId = await AddAppRequest(appNames[modelidx]);
                appIds[modelidx] = appId;
                modelidx++;

                foreach (DataRow row in dtCSV.Rows)
                {
                    tipIds[intentidx] = string.Format("TIP{0:D4}", intentidx + 1);
                    await AddIntentRequest(appId, tipIds[intentidx]);
                    await AddLabelRequest(appId, row[0].ToString(), tipIds[intentidx]);
                    intentidx++;

                    if ((intentidx % 19) == 0) // create new model
                    {
                        appNames[modelidx] = appNamePrefix + String.Format("MODEL{0}", modelidx);
                        appId = await AddAppRequest(appNames[modelidx]);
                        appIds[modelidx] = appId;
                        modelidx++;
                    }
                    progBar.Value++;
                }
            }

            return model_count;
        }

        static async Task<string> GenerateMainModel(DataTable dtCSV)
        {
            appNames[0] = appNamePrefix + "MAIN";
            string appId = await AddAppRequest(appNames[0]);
            appIds[0] = appId;
            await AddIntentRequest(appId, "TIPS");

            foreach (DataRow row in dtCSV.Rows)
            {
                await AddLabelRequest(appId, row[0].ToString(), "TIPS");
                progBar.Value++;
            }
            return appId;
        }

        static async Task<string> AddAppRequest(string appName)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            LUISApp newapp = new ModelGen.LUISApp();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ModelGenWindow.LUIS_subkey);

            var uri = "https://api.projectoxford.ai/luis/v1.0/prog/apps?" + queryString;

            HttpResponseMessage response;

            newapp.Name = appName;
            newapp.Description = "LUIS App Generated bu BOTGEN";
            newapp.Active = true;
            newapp.Culture = "zh-cn";
            newapp.NumberOfEntities = 0;
            newapp.NumberOfIntents = 20;
            newapp.IsTrained = true;

            string body = JsonConvert.SerializeObject(newapp);

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            string responseBodyAsText = await response.Content.ReadAsStringAsync();

            return responseBodyAsText.Replace("\"", "");
        }

        static async Task AddIntentRequest(string appId, string intentName)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            Intent newIntent = new ModelGen.Intent();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ModelGenWindow.LUIS_subkey);

            var uri = "https://api.projectoxford.ai/luis/v1.0/prog/apps/" + appId + "/intents?" + queryString;

            HttpResponseMessage response;

            newIntent.Name = intentName;
            string body = JsonConvert.SerializeObject(newIntent);

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }
        }

        static async Task AddLabelRequest(string appId, string uttrance, string intentName)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            Label newLabel = new ModelGen.Label();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ModelGenWindow.LUIS_subkey);

            var uri = "https://api.projectoxford.ai/luis/v1.0/prog/apps/" + appId + "/example?" + queryString;

            HttpResponseMessage response;

            newLabel.ExampleText = uttrance;
            newLabel.SelectedIntentName = intentName;

            string body = JsonConvert.SerializeObject(newLabel);

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }
        }

        public static async Task TrainModelRequest(string appId)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ModelGenWindow.LUIS_subkey);

            var uri = "https://api.projectoxford.ai/luis/v1.0/prog/apps/" + appId + "/train?" + queryString;

            HttpResponseMessage response;


            string body = JsonConvert.SerializeObject("{}");

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }
        }

        public static async Task<bool> TrainModelStatus(string appId)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ModelGenWindow.LUIS_subkey);

            var uri = "https://api.projectoxford.ai/luis/v1.0/prog/apps/" + appId + "/train?" + queryString;

            var response = await client.GetAsync(uri);

            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            else
                return false;

        }
        public static async Task PublishModelRequest(string appId)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ModelGenWindow.LUIS_subkey);

            var uri = "https://api.projectoxford.ai/luis/v1.0/prog/apps/" + appId + "/publish?" + queryString;

            HttpResponseMessage response;


            string body = JsonConvert.SerializeObject("{}");

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);

                string responseBodyAsText = await response.Content.ReadAsStringAsync();
            }
        }
    }
}
