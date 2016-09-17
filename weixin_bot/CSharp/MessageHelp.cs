//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Xml;
using System.Linq;
using System.Threading.Tasks;

namespace weixin_bot
{
    /// <summary>
    /// 接受/发送消息帮助类
    /// </summary>
    public class MessageHelp
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
                    if (EventKey.InnerText.Equals("click_one"))//click_one
                    {

                    }
                    else if (EventKey.InnerText.Equals("click_two"))//click_two
                    {
  
                    }
                    else if (EventKey.InnerText.Equals("click_three"))//click_three
                    {

                    }
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
                string returnString;
 
                LuisHelp cognitive = new weixin_bot.LuisHelp();
                
                LUISModel model = await LuisHelp.GetEntityFromLUIS(Content.InnerText);
                if (model.intents.Count() > 0)
                {
                    Intent maxscore_intent = model.intents.Max();

                    switch (maxscore_intent.intent)
                    {
                        case "Tips":
                            returnString = await cognitive.GetTips(model, FromUserName.InnerText, ToUserName.InnerText);
                            return returnString;

                        default:
                            if (BotParameters.use_binsearch)
                                returnString = await cognitive.GetBingSearch(Content.InnerText, BotParameters.WEIXIN_TITLE, BotParameters.WEIXIN_DEFAULTMSG, 
                                    FromUserName.InnerText, ToUserName.InnerText);
                            else    
                                returnString = cognitive.GetTunling123(Content.InnerText, BotParameters.WEIXIN_TITLE, BotParameters.WEIXIN_DEFAULTMSG, 
                                    FromUserName.InnerText, ToUserName.InnerText);
                            return returnString;

                    }
                }
                else
                {
                    returnString = BotParameters.WEIXIN_DEFAULTMSG;
                }
                responseContent = string.Format(ReplyType.Message_Text, 
                    FromUserName.InnerText, 
                    ToUserName.InnerText, 
                    DateTime.Now.Ticks, returnString);
            }

            return responseContent;
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