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
        public string query { get; set; }
        public Intent[] intents { get; set; }
        public Entity[] entities { get; set; }
    }

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private static async Task<CarCaringLUIS> GetEntityFromLUIS(string Query)
        {
            string appId = "b3369986-b55a-4f56-8755-97961e2b69bc";
            string subKey = "f5feb1edbaf3400daff5b89ea13dc85f";

            Query = Uri.EscapeDataString(Query);
            CarCaringLUIS Data = new CarCaringLUIS();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id="+appId+ "&subscription-key=" + subKey + "&q=" + Query;
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
        private string GetStoreLocation(CarCaringLUIS carLUIS)
        {
            string storeData = "";

            if (carLUIS.entities.Count() > 0)
            {
                storeData = "您要查找" + carLUIS.entities[0].entity + "附近的保修店。";
            }

            return storeData;
        }

        private async Task<string> GetPrice(string strAsk)
        {
            string priceData = "三百元";
            return priceData;
        }
        private async Task<string> GetItem(string strAsk)
        {
            string itemData = "轮胎";
            return itemData;
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                // calculate something for us to return
                int length = (message.Text ?? string.Empty).Length;
                string CarCaringString;
                CarCaringLUIS carLUIS = await GetEntityFromLUIS(message.Text);
                if (carLUIS.intents.Count() > 0)
                {
                    Intent maxscore_intent = carLUIS.intents.Max();

                    switch (maxscore_intent.intent)
                    {
                        case "StoreLocation":
                            CarCaringString = GetStoreLocation(carLUIS);
                            break;
                        case "CheckPrice":
                            CarCaringString = await GetPrice(carLUIS.entities[0].entity);
                            break;
                        case "CheckItem":
                            CarCaringString = await GetItem(carLUIS.entities[0].entity);
                            break;
                        default:
                            CarCaringString = "Sorry, I am not getting you...";
                            break;
                    }
                }
                else
                {
                    CarCaringString = "您可以到网站去查询我门的最新讯息。";
                }
                // return our reply to the user
                return message.CreateReplyMessage(CarCaringString);
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}