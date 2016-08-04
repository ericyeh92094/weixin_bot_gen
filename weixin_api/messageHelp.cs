using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace weixin_api
{
    /// <summary>
    /// 接受/发送消息帮助类
    /// </summary>
    public class messageHelp
    {
        //返回消息
        public async Task<string> ReturnMessage(string postStr)
        {
            string responseContent = "";
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(postStr)));
            XmlNode MsgType = xmldoc.SelectSingleNode("/xml/MsgType");
            if (MsgType != null)
            {
                switch (MsgType.InnerText)
                {
                    case "event":
                        responseContent = EventHandle(xmldoc);//事件处理
                        break;
                    case "text":
                        responseContent = await TextHandle(xmldoc);//接受文本消息处理
                        break;
                    default:
                        break;
                }
            }
            return responseContent;
        }
        //事件
        public string EventHandle(XmlDocument xmldoc)
        {
            string responseContent = "";
            XmlNode Event = xmldoc.SelectSingleNode("/xml/Event");
            XmlNode EventKey = xmldoc.SelectSingleNode("/xml/EventKey");
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");

            if (Event!=null)
            {
                //菜单单击事件
                if (Event.InnerText.Equals("CLICK"))
                {
                    /*
                    if (EventKey.InnerText.Equals("click_one"))//click_one
                    {
                        responseContent = string.Format(ReplyType.Message_Text,
                            FromUserName.InnerText,
                            ToUserName.InnerText, 
                            DateTime.Now.Ticks, 
                            "你点击的是click_one");
                    }
                    else if (EventKey.InnerText.Equals("click_two"))//click_two
                    {
  
                    }
                    else if (EventKey.InnerText.Equals("click_three"))//click_three
                    {

                    }
                    */
                }
            }

            return responseContent;
        }
        //接受文本消息
        public async Task<string> TextHandle(XmlDocument xmldoc)
        {

            string responseContent = "";
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            XmlNode Content = xmldoc.SelectSingleNode("/xml/Content");

           // MessageDBLog(xmldoc, false, null);

            if (Content != null)
            {

                LuisHelp cognitive = new weixin_api.LuisHelp();
                string CarCaringString;
                CarCaringLUIS carLUIS = await LuisHelp.GetEntityFromLUIS(Content.InnerText);
                if (carLUIS.intents.Count() > 0)
                {
                    Intent maxscore_intent = carLUIS.intents.Max();

                    switch (maxscore_intent.intent)
                    {
                        case "StoreLocation":
                            string storeURL = "";
                            CarCaringString = cognitive.GetStoreLocation(carLUIS, ref storeURL);
                            if (storeURL != "")
                            {
                                responseContent = string.Format(ReplyType.Message_News_Main,
                                     FromUserName.InnerText,
                                     ToUserName.InnerText,
                                     DateTime.Now.Ticks,
                                     "2",
                                     string.Format(ReplyType.Message_News_Item, "", "查找店家资讯",
                                     "",
                                     "http://www.giti.com/") + 
                                     string.Format(ReplyType.Message_News_Item, CarCaringString, "",
                                    "http://www.giti.com/Content/en/Images/giti-tire-store_icon.png",
                                     storeURL));

                                return responseContent;
                            }
                            break;
                        case "CheckPrice":
                            CarCaringString = cognitive.GetPrice(carLUIS);
                            break;
                        case "CheckItem":
                            CarCaringString = await cognitive.GetItem(carLUIS, FromUserName.InnerText, ToUserName.InnerText);
                            return CarCaringString;
                            break;
                        case "News":
                            CarCaringString = cognitive.GetNews(carLUIS);
                            break;
                        case "Tips":
                            CarCaringString = await cognitive.GetTips(carLUIS, FromUserName.InnerText, ToUserName.InnerText);
                            return CarCaringString;
                            break;
                        default:
                            CarCaringString = await cognitive.GetBingSearch(Content.InnerText, "佳通官网", "您可以询问附近的保修站，轮胎商品或是保养知识；或到网站去查询我门的最新讯息。",FromUserName.InnerText, ToUserName.InnerText);
                            return CarCaringString;
                            break;
                    }
                }
                else
                {
                    CarCaringString = "您可以到网站去查询我门的最新讯息。";
                }
                responseContent = string.Format(ReplyType.Message_Text, 
                    FromUserName.InnerText, 
                    ToUserName.InnerText, 
                    DateTime.Now.Ticks, CarCaringString);
            }

            //MessageDBLog(xmldoc, true, responseContent);
            return responseContent;
        }

        //写入日志
        public void WriteLog(string text)
        {
            StreamWriter sw = new StreamWriter(HttpContext.Current.Server.MapPath(".") + "\\log.txt", true);
            sw.WriteLine(text);
            sw.Close();//写入
        }

        public void MessageDBLog(XmlDocument xmldoc, bool OutMsg, string xmlString)
        {
            var db = new MessageDBDataContext();
            Message wechatmsg = new Message();

            XmlNode MsgId = xmldoc.SelectSingleNode("/xml/MsgId");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            XmlNode Content = xmldoc.SelectSingleNode("/xml/Content");
            XmlNode MsgType = xmldoc.SelectSingleNode("/xml/MsgType");

            wechatmsg.MessageID = MsgId.InnerText;
            wechatmsg.FromUserName = FromUserName.InnerText;
            wechatmsg.MsgType = MsgType.InnerText;
            wechatmsg.CreateTimeWeChat = DateTime.Now.ToString();

            wechatmsg.Out = OutMsg;
            if (OutMsg)
                wechatmsg.ContentWeChat = XElement.Parse(xmlString);
            else
                wechatmsg.ContentWeChat = System.Xml.Linq.XElement.Load(new XmlNodeReader(xmldoc));
            db.Messages.InsertOnSubmit(wechatmsg);
            db.SubmitChanges();
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