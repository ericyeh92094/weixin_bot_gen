using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

namespace WechatSDK
{
    public partial class barcodeScan : System.Web.UI.Page
    {
        public string appId = "";
        public string timestamp = "";
        public string nonceStr = "";
        public string signature = "";
        public string access_token = "";

        /*
        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Ignore("{resource}.aspx/{*pathInfo}");
        }
        */

        protected void Page_Load(object sender, EventArgs e)
        {
           

            if (!IsPostBack)
            {
                Hashtable ht = JSSDK.getSignPackage();
                appId = ht["appId"].ToString();
                nonceStr = ht["nonceStr"].ToString();
                timestamp = ht["timestamp"].ToString();
                signature = ht["signature"].ToString();
                access_token = ht["access_token"].ToString();
            }
        }

        [WebMethod(true)]
        public static string QRcode_url()
        {
            QRcodeResponse qr = JSSDK.GetQrcode();
            return qr.url;
        }
    }
}