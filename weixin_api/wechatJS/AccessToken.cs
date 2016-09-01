using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WechatSDK
{
    public class AccessToken
    {
        public string access_token { get; set; }

        public double expires_in { get; set; }
    }

    public class actionInfo
    {
        public double[] scene_id { get; set; }
        public string[] scense_str { get; set; }
    }
    public class QRcodeRequest
    {
        public double expire_seconds { get; set; }
        public string action_name { get; set; }
        public List<actionInfo> action_info { get; set; }
    }

    public class QRcodeResponse
    {
        public string ticket { get; set; }
        public double expire_seconds { get; set; }
        public string url { get; set; }
    }
}