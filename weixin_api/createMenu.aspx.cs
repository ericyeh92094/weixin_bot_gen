using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;

namespace weixin_api
{
    public class GetAccessToken
    {
        static public string myToekn = "";
        static public string myappid = "wx6e4b63cba932bca8";
        static public string mysecret = "a1370e570a4db84b807c4488b99d964b";
        static public wechat_access_token myaccesstoken;

        public class wechat_access_token
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }

        public static string RetriveAccessToken()
        {
            string requestUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + myappid + "&secret=" + mysecret;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(requestUrl) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(wechat_access_token));
                myaccesstoken = (wechat_access_token)serializer.ReadObject(instream);

                return myaccesstoken.access_token;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return string.Empty;
            }
        }
    }

    public partial class createMenu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FileStream fs1 = new FileStream(Server.MapPath(".")+"\\menu.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs1, Encoding.GetEncoding("GBK"));
            string menu = sr.ReadToEnd();
            sr.Close();
            fs1.Close();
            string token = GetAccessToken.RetriveAccessToken();

            https://api.weixin.qq.com/cgi-bin/menu/create?access_token=
            string posturl = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + token;
            GetPage(posturl, menu);
        }
        public string GetPage(string posturl, string postData)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            byte[] data = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(posturl) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                Response.Write(content);
                return content;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return string.Empty;
            }
        }
    }
}

