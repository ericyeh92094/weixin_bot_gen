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
using System.Web;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace weixin_bot
{
    /// <summary>
    /// interfaceHandler 的摘要说明
    /// </summary>
    public class interfaceHandler : IHttpHandler
    {
        HttpResponse currentResponse;
        public void ProcessRequest(HttpContext param_context)
        {

            string postString = string.Empty;

            if (HttpContext.Current.Request.HttpMethod.ToUpper() == "POST")
            {
                using (Stream stream = HttpContext.Current.Request.InputStream)
                {
                    Byte[] postBytes = new Byte[stream.Length];
                    stream.Read(postBytes, 0, (Int32)stream.Length);
                    postString = Encoding.UTF8.GetString(postBytes);

                    currentResponse = HttpContext.Current.Response;
                    Handle(postString);
                }
            }
            else
            {
                Trace.TraceError("InterfaceTest");
                InterfaceTest();
            }

        }

        /// <summary>
        /// 处理信息并应答
        /// </summary>
        private async void Handle(string postStr)
        {
            MessageHelp help = new MessageHelp();
            string responseContent = await help.ReturnMessage(postStr);

            currentResponse.ContentEncoding = Encoding.UTF8;
            currentResponse.Write(responseContent);

        }

        //成为开发者url测试，返回echoStr
        public void InterfaceTest()
        {
            string token = BotParameters.WEIXIN_token;
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            string echoString = HttpContext.Current.Request.QueryString["echoStr"];
            string signature = HttpContext.Current.Request.QueryString["signature"];
            string timestamp = HttpContext.Current.Request.QueryString["timestamp"];
            string nonce = HttpContext.Current.Request.QueryString["nonce"];

            if (!string.IsNullOrEmpty(echoString))
            {
                HttpContext.Current.Response.Write(echoString);
                HttpContext.Current.Response.End();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}