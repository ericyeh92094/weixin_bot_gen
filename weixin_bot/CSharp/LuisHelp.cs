using System;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System.Text;
using System.IO;

namespace weixin_bot
{


    public class LuisHelp
    {

        public static async Task<LUISModel> GetEntityFromLUIS(string Query)
        {
            string appId = BotParameters.LUIS_app_Id;
            string subKey = BotParameters.LUIS_sub_key;

            Query = Uri.EscapeDataString(Query);
            LUISModel Data = new LUISModel();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=" + appId + "&subscription-key=" + subKey + "&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<LUISModel>(JsonDataResponse);
                }
            }
            return Data;
        }

        public async Task<string> GetBingSearch(string query, string mainTitle, string mainMsg, string fromUser, string toUser)
        {
            string sub_key = BotParameters.Bing_key;
            RootObject searchResults = new RootObject();

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", sub_key);

            // Request parameters
            //queryString["q"] = query;
            queryString["count"] = "10";
            queryString["offset"] = "0";
            queryString["mkt"] = "zh-cn";
            queryString["safeSearch"] = "Moderate";
            var uri = "https://api.cognitive.microsoft.com/bing/v5.0/search?q=" + query + queryString;

            var response = await client.GetStringAsync(uri);
            searchResults = JsonConvert.DeserializeObject<RootObject>(response);

            string resultStr = string.Format(ReplyType.Message_News_Item, mainTitle, mainMsg,
                    BotParameters.WEIXIN_PICURL,
                    BotParameters.WEIXIN_MAINURL);

            for (int i = 0; i < 5; i++)
            {
                resultStr += string.Format(ReplyType.Message_News_Item, searchResults.webPages.value[i].name, searchResults.webPages.value[i].snippet,
                    searchResults.webPages.value[i].thumbnailUrl, searchResults.webPages.value[i].url);
            }
            string itemData = string.Format(ReplyType.Message_News_Main, fromUser, toUser, DateTime.Now.Ticks,
                    "5", resultStr);

            return itemData;

        }

        public string GetTunling123(string query, string mainTitle, string mainMsg, string fromUser, string toUser)
        {
            String APIKEY = BotParameters.Tunling123_key;
            string result = null;

            try
            {
                HttpWebResponse Response = null;

                String _strMessage = query;
                String INFO = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(_strMessage));

                String getURL = "http://www.tuling123.com/openapi/api?key=" + APIKEY + "&info=" + INFO;
                HttpWebRequest MyRequest = (HttpWebRequest)HttpWebRequest.Create(getURL);
                HttpWebResponse MyResponse = (HttpWebResponse)MyRequest.GetResponse();

                Response = MyResponse;
                using (Stream MyStream = MyResponse.GetResponseStream())
                {
                    long ProgMaximum = MyResponse.ContentLength;
                    long totalDownloadedByte = 0;
                    byte[] by = new byte[1024];
                    int osize = MyStream.Read(by, 0, by.Length);
                    Encoding encoding = Encoding.UTF8;
                    while (osize > 0)
                    {
                        totalDownloadedByte = osize + totalDownloadedByte;
                        result += encoding.GetString(by, 0, osize);
                        long ProgValue = totalDownloadedByte;
                        osize = MyStream.Read(by, 0, by.Length);
                    }
                }

                JsonReader reader = new JsonTextReader(new StringReader(result));
                while (reader.Read())
                {
                    if (reader.Path == "text")
                    {
                        result = reader.Value.ToString();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            result = string.Format(ReplyType.Message_Text,
                    fromUser,
                    toUser,
                    DateTime.Now.Ticks, result);
            return result;
        }

        public async Task<LUISModel> GetModel(string appId, string subKey, string Query)
        {
            Query = Uri.EscapeDataString(Query);
            LUISModel Model = new LUISModel();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=" + appId + "&subscription-key=" + subKey + "&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Model = JsonConvert.DeserializeObject<LUISModel>(JsonDataResponse);
                }
            }
            return Model;
        }
        public async Task<Intent[]> GetTipIntent(string Query)
        {
            int model_count = BotParameters.LUIS_model_count;

            LUISModel[] models = new LUISModel[model_count - 1]; // start from 1, 0 = main model

            for (int m = 0; m < model_count - 1; m++)
            {
                models[m] = await GetModel(BotParameters.LUIS_model_Ids[m], BotParameters.LUIS_sub_key, Query);
            }
            Intent[] score_intent = new Intent[LUISModel.MAX_INTENT * 4];

            int i = 0;
            for (int m = 0; m < model_count - 1; m++)
            {
                foreach (Intent intent in models[m].intents)
                    if (intent.score > BotParameters.model_threshold && intent.intent != "None")
                        score_intent[i++] = intent;
            }

            //Array.Sort(score_intent);
            return score_intent;
        }

        public string GetKB(string intent_id)
        {
            string response = "";
            string accountName = BotParameters.Azure_storage_account;
            string key = BotParameters.Azure_storage_key;

            //Adapt intent id to rowkey for performance 
            intent_id = intent_id.Remove(0, 3);
            int id_num = Int32.Parse(intent_id);
            intent_id = string.Format("{0:D8}", id_num - 1);

            StorageCredentials creds = new StorageCredentials(accountName, key);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(BotParameters.Azure_storage_table);

            TableQuery<KBDoc> intentQuery = new TableQuery<KBDoc>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "1"),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, intent_id)));
            var results = table.ExecuteQuery(intentQuery);

            if (results.Any())
            {
                foreach (KBDoc kb in results)
                {
                    string url = kb.URL.Replace("&amp;", "&");
                    response = string.Format(ReplyType.Message_News_Item, kb.Description, "", kb.IconURL, url);
                }
            }
            return response;
        }
        public async Task<string> GetTips(LUISModel model, string fromUser, string toUser)
        {
            Intent[] maxscore_intent = await GetTipIntent(model.query);

            int count = maxscore_intent.Count();

            string infoStr = string.Format(ReplyType.Message_News_Item, BotParameters.WEIXIN_TITLE, BotParameters.WEIXIN_TITLE,
                                     BotParameters.WEIXIN_PICURL,
                                     BotParameters.WEIXIN_MAINURL);
            int cnt = 1;
            for (int i = 0; i < count; i++)
            {
                if (maxscore_intent[i] != null)
                {
                    infoStr += GetKB(maxscore_intent[i].intent);
                    cnt++;
                    if (cnt > BotParameters.max_intent_display) break;
                }
            }

            string responseContent = string.Format(ReplyType.Message_News_Main, fromUser, toUser,
                DateTime.Now.Ticks,
                cnt.ToString(),
                infoStr);

            return responseContent;
        }
    }
}