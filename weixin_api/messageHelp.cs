using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Security;
using System.Xml;

namespace weixin_api
{
    /// <summary>
    /// 接受/发送消息帮助类
    /// </summary>
    public class messageHelp
    {
        //返回消息
        public string ReturnMessage(string postStr)
        {
            string responseContent = "";
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("GB2312").GetBytes(postStr)));
            XmlNode MsgType = xmldoc.SelectSingleNode("/xml/MsgType");
            if (MsgType!=null)
            {
                switch (MsgType.InnerText)
                {
                    case "event":
                        responseContent=EventHandle(xmldoc);//事件处理
                        break;
                    case "text":
                        responseContent=TextHandle(xmldoc);//接受文本消息处理
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
                        responseContent = string.Format(ReplyType.Message_Text,
                            FromUserName.InnerText,
                            ToUserName.InnerText, 
                            DateTime.Now.Ticks, 
                            "你点击的是click_one");
                    }
                    else if (EventKey.InnerText.Equals("click_two"))//click_two
                    {
                        responseContent = string.Format(ReplyType.Message_News_Main, 
                            FromUserName.InnerText, 
                            ToUserName.InnerText, 
                            DateTime.Now.Ticks, 
                            "2",
                             string.Format(ReplyType.Message_News_Item,"我要寄件","",
                             "http://www.soso.com/orderPlace.jpg",
                             "http://www.soso.com/")+
                             string.Format(ReplyType.Message_News_Item, "订单管理", "",
                             "http://www.soso.com/orderManage.jpg",
                             "http://www.soso.com/"));
                    }
                    else if (EventKey.InnerText.Equals("click_three"))//click_three
                    {
                        responseContent = string.Format(ReplyType.Message_News_Main,
                            FromUserName.InnerText,
                            ToUserName.InnerText,
                            DateTime.Now.Ticks,
                            "1",
                             string.Format(ReplyType.Message_News_Item, "标题", "摘要",
                             "http://www.soso.com/jieshao.jpg",
                             "http://www.soso.com/"));
                    }
                }
            }
            return responseContent;
        }
        //接受文本消息
        public string TextHandle(XmlDocument xmldoc)
        {
            string responseContent = "";
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            XmlNode Content = xmldoc.SelectSingleNode("/xml/Content");
            if (Content != null)
            {
                responseContent = string.Format(ReplyType.Message_Text, 
                    FromUserName.InnerText, 
                    ToUserName.InnerText, 
                    DateTime.Now.Ticks, 
                    "欢迎使用微信公共账号，您输入的内容为：" + Content.InnerText+"\r\n<a href=\"http://www.baidu.com\">点击进入</a>");
            }
            return responseContent;
        }

        //写入日志
        public void WriteLog(string text)
        {
            StreamWriter sw = new StreamWriter(HttpContext.Current.Server.MapPath(".") + "\\log.txt", true);
            sw.WriteLine(text);
            sw.Close();//写入
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