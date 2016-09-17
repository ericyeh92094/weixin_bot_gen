namespace weixin_bot
{
public class BotParameters {
        public static string WEIXIN_TITLE = "佳通轮胎博士";
        public static string WEIXIN_MAINURL = "http://www.giti.com/";
        public static string WEIXIN_PICURL = "http://www.giti.com/Content/cn/Images/doctor-ask.png";

    public static string WEIXIN_DEFAULTMSG = "您可以询问附近的保修站，轮胎商品或是保养知识；或到网站去查询我门的最新讯息";
        public static string WEIXIN_token = "myironmanaccess";
        public static string LUIS_app_Id = "47422b3b-f47e-4bf0-9cc3-5f18271fcff0";
        public static string LUIS_sub_key = "f5feb1edbaf3400daff5b89ea13dc85f";
        public static string Azure_storage_account = "gitio2otable";
        public static string Azure_storage_table = "myweixintable";
        public static string Azure_storage_key = "FLK9B+lVSQNUyNPnOBSALHBBmDBn8afggeocKWxWNBzaK0KACErrswA/iZR7wbMB+T1hrFMsMahNgbJiM4yTHA==";
        public static int LUIS_model_count = 5;
        public static string[] LUIS_model_Ids = {
        "daeec8af-48aa-47e0-8166-8ea6ac1b0755",
		"f9944276-83d1-438f-befc-eb0fbc24dd12",
		"1a630843-950a-4bb9-b47c-359d4ae7a760",
		"0d9b9d33-b174-4499-b4ad-04c2cb47f5a4",
	};
	public static double model_threshold = (0.1);
	public static int max_intent_display = 5;
	public static bool use_binsearch = false;
    public static string Tunling123_key = "8b4fe884f37946e2b1981c2ea2dc7c66";
    public static string Bing_key = "FLK9B+lVSQNUyNPnOBSALHBBmDBn8afggeocKWxWNBzaK0KACErrswA/iZR7wbMB+T1hrFMsMahNgbJiM4yTHA==";
    }
}
