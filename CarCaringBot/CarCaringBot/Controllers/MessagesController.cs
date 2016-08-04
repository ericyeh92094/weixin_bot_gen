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

    [BotAuthentication]
    public class MessagesController : ApiController
    {

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                LuisHelp cognitive = new LuisHelp();
                string CarCaringString;
                CarCaringLUIS carLUIS = await LuisHelp.GetEntityFromLUIS(message.Text);
                if (carLUIS.intents.Count() > 0)
                {
                    Intent maxscore_intent = carLUIS.intents.Max();

                    switch (maxscore_intent.intent)
                    {
                        case "StoreLocation":
                            string storeURL = "";
                            CarCaringString = cognitive.GetStoreLocation(carLUIS, ref storeURL);
                            break;
                        case "CheckPrice":
                            CarCaringString = cognitive.GetPrice(carLUIS);
                            break;
                        case "CheckItem":
                            CarCaringString = cognitive.GetItem(carLUIS);
                            break;
                        case "News":
                            CarCaringString = cognitive.GetNews(carLUIS);
                            break;
                        case "Tips":
                            CarCaringString = await cognitive.GetTips(carLUIS, "FromUser", "ToUser");
                            break;
                        default:
                            CarCaringString = await cognitive.GetBingSearch(message.Text); // "您可以到网站去查询我门的最新讯息。";


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

    //回复类型
    public class ReplyType
    {
        /// <summary>
        /// 普通文本消息
        /// </summary>
        public static string Message_Text
        {
            get { return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[text]]></MsgType>
                            <Content><![CDATA[{3}]]></Content>
                            </xml>"; }
        }
        /// <summary>
        /// 图文消息主体
        /// </summary>
        public static string Message_News_Main
        {
            get
            {
                return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[news]]></MsgType>
                            <ArticleCount>{3}</ArticleCount>
                            <Articles>
                            {4}
                            </Articles>
                            </xml> ";
            }
        }
        /// <summary>
        /// 图文消息项
        /// </summary>
        public static string Message_News_Item
        {
            get
            {
                return @"<item>
                            <Title><![CDATA[{0}]]></Title> 
                            <Description><![CDATA[{1}]]></Description>
                            <PicUrl><![CDATA[{2}]]></PicUrl>
                            <Url><![CDATA[{3}]]></Url>
                            </item>";
            }
        }
    }
}