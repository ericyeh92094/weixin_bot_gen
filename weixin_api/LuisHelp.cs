using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;

namespace weixin_api
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
        public string GetStoreLocation(CarCaringLUIS carLUIS)
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
                storeData = "您要查找" + ent_str + "附近的保修店。";
            }
            else // likely no location info
            {
                storeData = "可以说说您的位置在哪? 或者到 http://hutai.giti.com/store-locator 指定位置。";
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

        public string GetTips(CarCaringLUIS carLUIS)
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
                itemData = "您要查找" + ent_str + "的相关资讯。请到 http://www.giti.com/giti-doctor?b=1&s=1 请教轮胎博士。";
            }
            else // likely no location info
            {
                itemData = "想了解更多保养知识? 请到 http://www.giti.com/giti-doctor?b=1&s=1 请教轮胎博士。";
            }

            return itemData;
        }
    }
}