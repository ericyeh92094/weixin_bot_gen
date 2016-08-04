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

namespace CarCaringBot
{
    public class Intent : IComparable<Intent>
    {
        public string intent { get; set; }
        public float score { get; set; }

        int IComparable<Intent>.CompareTo(Intent other)
        {
            if (other.score > score)
                return -1;
            else if (other.score == score)
                return 0;
            else
                return 1;
        }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
    }

    public class CarCaringLUIS
    {
        public const int MAX_INTENT = 20;
        public string query { get; set; }
        public Intent[] intents { get; set; }
        public Entity[] entities { get; set; }
    }

    public class LuisHelp
    {

        public static async Task<CarCaringLUIS> GetEntityFromLUIS(string Query)
        {
            string appId = "b3369986-b55a-4f56-8755-97961e2b69bc";
            string subKey = "f5feb1edbaf3400daff5b89ea13dc85f";

            Query = Uri.EscapeDataString(Query);
            CarCaringLUIS Data = new CarCaringLUIS();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=" + appId + "&subscription-key=" + subKey + "&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<CarCaringLUIS>(JsonDataResponse);
                }
            }
            return Data;
        }

        //private async Task<string> GetStoreLocation(string strAsk)
        public string GetStoreLocation(CarCaringLUIS carLUIS, ref string storeURL)
        {
            string storeData = "";
            int ent_count = carLUIS.entities.Count();

            if (ent_count > 0)
            {
                string ent_str = "";
                for (int i = 0; i < ent_count; i++)
                {
                    ent_str += carLUIS.entities[i].entity;
                }
                if (ent_str.IndexOf("市") < 0)
                    ent_str = ent_str + "市";

                storeData = "您要查找" + ent_str + "附近的轮胎保修店。";
                storeURL = "http://www.giti.com/store-locator/list?province=" + HttpUtility.UrlEncode(ent_str);
            }
            else // likely no location info
            {
                storeData = "可以说说您的位置在哪? 或者到 http://hutai.giti.com/store-locator 指定位置。";
                storeURL = "";
            }

            return storeData;
        }

        public string GetPrice(CarCaringLUIS carLUIS)
        {
            string priceData = "三百元";
            return priceData;
        }
        public string GetItem(CarCaringLUIS carLUIS)
        {
            string itemData = "";
            int ent_count = carLUIS.entities.Count();

            if (ent_count > 0)
            {
                string ent_str = "";
                for (int i = 0; i < ent_count; i++)
                {
                    ent_str += carLUIS.entities[i].entity;
                }
                itemData = "您要查找" + ent_str + "的相关资讯。";
            }
            else // likely no location info
            {
                itemData = "可以说说您想要的轮胎规格? 或者到 http://www.giti.com/tires/daily-driving 查找您要的品项。";
            }

            return itemData;
        }

        public string GetNews(CarCaringLUIS carLUIS)
        {
            string itemData = "";
            int ent_count = carLUIS.entities.Count();

            if (ent_count > 0)
            {
                string ent_str = "";
                for (int i = 0; i < ent_count; i++)
                {
                    ent_str += carLUIS.entities[i].entity;
                }
                itemData = "您要查找" + ent_str + "的相关资讯。请到 http://www.giti.com/promotion 逛逛。";
            }
            else // likely no location info
            {
                itemData = "想知道热门促销? 请到 http://www.giti.com/promotion 逛逛。";
            }

            return itemData;
        }

        public async Task<string> GetBingSearch(string query)
        {
            string sub_key = "8c9c892854ca40efb3c05070f2158704";
            RootObject searchResults = new RootObject();

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", sub_key);

            // Request parameters
            queryString["q"] = query;
            queryString["count"] = "5";
            queryString["offset"] = "0";
            queryString["mkt"] = "zh-CN";
            queryString["safeSearch"] = "Moderate";
            var uri = "https://api.cognitive.microsoft.com/bing/v5.0/search?" + queryString;

            var response = await client.GetStringAsync(uri);
            searchResults = JsonConvert.DeserializeObject<RootObject>(response);

            string itemData = searchResults.webPages.value[0].name;

            return itemData;

        }

        public async Task<CarCaringLUIS> GetModel(string appId, string subKey, string Query)
        {
            Query = Uri.EscapeDataString(Query);
            CarCaringLUIS Model = new CarCaringLUIS();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=" + appId + "&subscription-key=" + subKey + "&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Model = JsonConvert.DeserializeObject<CarCaringLUIS>(JsonDataResponse);
                }
            }
            return Model;
        }
        public async Task<Intent[]> GetTipIntent(string Query)
        {
            string appId_G1 = "6b3f6b3c-f226-493c-8b46-65c234e7fa27";
            string appId_G2 = "387f9840-cbf8-4609-a672-42258b6eebca";
            string appId_G3 = "8f3dbae8-af16-4d06-a03e-44511607583b";
            string subKey = "f5feb1edbaf3400daff5b89ea13dc85f";

            CarCaringLUIS ModelG1 = await GetModel(appId_G1, subKey, Query);
            CarCaringLUIS ModelG2 = await GetModel(appId_G2, subKey, Query);
            CarCaringLUIS ModelG3 = await GetModel(appId_G3, subKey, Query);

            Intent [] score_intent = new Intent[CarCaringLUIS.MAX_INTENT * 4];

            int i = 0;
            foreach (Intent intent in ModelG1.intents)
                if (intent.score > 0.2 && intent.intent != "None")
                    score_intent[i++] = intent;

            foreach (Intent intent in ModelG2.intents)
                if (intent.score > 0.2 && intent.intent != "None")
                    score_intent[i++] = intent;

            foreach (Intent intent in ModelG3.intents)
                if (intent.score > 0.2 && intent.intent != "None")
                    score_intent[i++] = intent;

            //Array.Sort(score_intent);
            return score_intent;
        }

        public class GitiDocKB : TableEntity
        {
            public GitiDocKB() { }

            public string ID { get; set; }
            public string Questions { get; set; }
            public string Topic { get; set; }
            public string PicURL { get; set; }
            public string Model { get; set; }
        }
        public string GetKB(string intent_id)
        {
            string response = "";
            string accountName = "gitio2otable";
            string key = "FLK9B+lVSQNUyNPnOBSALHBBmDBn8afggeocKWxWNBzaK0KACErrswA/iZR7wbMB+T1hrFMsMahNgbJiM4yTHA==";

            intent_id = intent_id.Remove(0, 3);
            StorageCredentials creds = new StorageCredentials(accountName, key);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);
            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("gitidockb");

            //TableOperation retrieveOperation = TableOperation.Retrieve<GitiDocKB>("ID", intent_id);
            TableQuery<GitiDocKB> intentQuery = new TableQuery<GitiDocKB>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "AAA"),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("ID", QueryComparisons.Equal, intent_id)));
            var results = table.ExecuteQuery(intentQuery);

            if (results.Any())
            {
                foreach (GitiDocKB kb in results)
                {
                    string url = "http://www.giti.com" + kb.Topic.Replace("&amp;", "&");
                    response = string.Format(ReplyType.Message_News_Item, kb.Questions, "", kb.PicURL, url);
                }
            }
            return response;
        }
        public async Task<string> GetTips(CarCaringLUIS carLUIS, string fromUser, string toUser)
        {
            Intent [] maxscore_intent = await GetTipIntent(carLUIS.query);

            int count = maxscore_intent.Count();
  
            string infoStr = string.Format(ReplyType.Message_News_Item, "佳通轮胎博士", "佳通轮胎博士",
                                     "http://www.giti.com/Content/cn/Images/doctor-ask.png",
                                     "http://www.giti.com/");
            int cnt = 1;
            for (int i = 0; i < count; i++)
            {
                if (maxscore_intent[i] != null)
                {
                    infoStr += GetKB(maxscore_intent[i].intent);

                        //string.Format(ReplyType.Message_News_Item, carLUIS.query + maxscore_intent[i].intent, "", "", "http://www.giti.com/");
                    cnt++;
                    if (cnt > 5) break;
                }
            }

            string responseContent = string.Format(ReplyType.Message_News_Main, fromUser, toUser,
                DateTime.Now.Ticks,
                cnt.ToString(),
                infoStr);

            return responseContent;

              /*
              int ent_count = carLUIS.entities.Count();

              if (ent_count > 0)
              {
                  string ent_str = "";
                  for (int i = 0; i < ent_count; i++)
                  {
                      ent_str += carLUIS.entities[i].entity;
                  }
                  itemData = "您要查找" + ent_str + "的相关资讯。请到 http://www.giti.com/giti-doctor?b=1&s=1 请教轮胎博士。";
              }
              else // likely no location info
              {
                  itemData = "想了解更多保养知识? 请到 http://www.giti.com/giti-doctor?b=1&s=1 请教轮胎博士。";
              }
              */
        }
    }
}