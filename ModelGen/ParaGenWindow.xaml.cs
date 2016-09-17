using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace ModelGen
{
    /// <summary>
    /// Interaction logic for ParaGenWindow.xaml
    /// </summary>
    public partial class ParaGenWindow : Window
    {
        StringBuilder parameters;

        string weixin_title = "";
        string weixin_url = "";
        string weixin_iconurl = "";
        string weixin_defaultmsg = "";
        string weixin_token = "";
        string api_key = "";

        static string subregKey = "SOFTWARE\\BOTMODELGEN";
        static string KeyName_title = "WEIXIN_TITLE";
        static string KeyName_url = "WEIXIN_URL";
        static string KeyName_iconurl = "WEIXIN_ICONURL";
        static string KeyName_defaultmsg = "WEIXIN_DEFAULTMSG";
        static string KeyName_token = "WEIXIN_TTOKEN";
        static string KeyName_apikey = "API_KEY";

        public bool ReadAccountStringsFromKey()
        {
            RegistryKey rk = Registry.CurrentUser;
            RegistryKey sk1 = rk.OpenSubKey(subregKey);

            if (sk1 == null)
            {
                return false;
            }
            else
            {
                try
                {
                    weixin_title = (string)sk1.GetValue(KeyName_title);
                    weixin_url = (string)sk1.GetValue(KeyName_url);
                    weixin_iconurl = (string)sk1.GetValue(KeyName_iconurl);
                    weixin_token = (string)sk1.GetValue(KeyName_token);
                    weixin_defaultmsg = (string)sk1.GetValue(KeyName_defaultmsg);
                    api_key = (string)sk1.GetValue(KeyName_apikey);
                }
                catch (Exception e)
                {
                    return false;
                }
                return true;
            }
        }
        public bool WriteAccountStringsToKey()
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser;
                RegistryKey sk1 = rk.CreateSubKey(subregKey);
                // Save the value
                sk1.SetValue(KeyName_title, weixin_title);
                sk1.SetValue(KeyName_url, weixin_url);
                sk1.SetValue(KeyName_iconurl, weixin_iconurl);
                sk1.SetValue(KeyName_defaultmsg, weixin_defaultmsg);
                sk1.SetValue(KeyName_token, weixin_token);
                sk1.SetValue(KeyName_apikey, api_key);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public ParaGenWindow()
        {
            InitializeComponent();
        }

        public void GenParameters()
        {
            parameters.Append("namespace weixin_bot\r\n");
            parameters.Append("{\n");
            parameters.Append("public class BotParameters {\r\n");
            parameters.Append(string.Format("\tpublic static string WEIXIN_TITLE = \"{0}\";\r\n", weixin_title));
            parameters.Append(string.Format("\tpublic static string WEIXIN_MAINURL =\"{0}\";\r\n", weixin_url));
            parameters.Append(string.Format("\tpublic static string WEIXIN_PICURL =\"{0}\";\r\n", weixin_iconurl));
            parameters.Append(string.Format("\tpublic static string WEIXIN_DEFAULTMSG =\"{0}\";\r\n", weixin_defaultmsg));
            parameters.Append(string.Format("\tpublic static string WEIXIN_token =\"{0}\";\r\n", weixin_token));
            parameters.Append(string.Format("\tpublic static string LUIS_app_Id =\"{0}\";\r\n", LUISGen.appIds[0])); // Main model
            parameters.Append(string.Format("\tpublic static string LUIS_sub_key =\"{0}\";\r\n", ModelGenWindow.LUIS_subkey));
            parameters.Append(string.Format("\tpublic static string Azure_storage_account =\"{0}\";\r\n", ModelGenWindow.azure_storage_account));
            parameters.Append(string.Format("\tpublic static string Azure_storage_table =\"{0}\";\r\n", ModelGenWindow.azure_storage_table));
            parameters.Append(string.Format("\tpublic static string Azure_storage_key =\"{0}\";\r\n", ModelGenWindow.azure_storgae_subkey));
            parameters.Append(string.Format("\tpublic static  int LUIS_model_count ={0};\r\n", ModelGenWindow.model_num));
            parameters.Append("\tpublic static string[] LUIS_model_Ids = {\r\n");

            for (int i = 1; i < ModelGenWindow.model_num - 1; i++ )
            {
                parameters.Append(string.Format("\t\t\"{0}\",\r\n", LUISGen.appIds[i]));
            }
            parameters.Append(string.Format("\t\t\"{0}\"\r\n", LUISGen.appIds[ModelGenWindow.model_num - 1]));
            parameters.Append("\t};\r\n");

            parameters.Append("\tpublic static double model_threshold = (0.2);\r\n");
            parameters.Append("\tpublic static int max_intent_display = 5;\r\n");

            if (bingradioButton.IsChecked == true)
            {
                parameters.Append("\tpublic static bool use_binsearch = true;\r\n");
                parameters.Append(string.Format("\tpublic static string Bing_key = \"{0}\"\r\n", api_key));
                parameters.Append(string.Format("\tpublic static string Tunling123_key = \"\"\r\n"));
            }
            else
            {
                parameters.Append("\tpublic static bool use_binsearch = false;\r\n");
                parameters.Append(string.Format("\tpublic static string Tunling123_key = \"{0}\"\r\n", api_key));
                parameters.Append(string.Format("\tpublic static string Bing_key = \"\"\r\n"));
            }

            parameters.Append("\t}\r\n}\r\n");
        }

        protected void text_TextChanged(object sender, TextChangedEventArgs e)
        {
            weixin_title = titletext.Text;
            weixin_url = mainurl.Text;
            weixin_iconurl = iconurl.Text;
            weixin_defaultmsg = defaultmsg.Text;
            weixin_token = weixintoken.Text;
            api_key = apikey.Text;

            parameters.Clear();
            GenParameters();
            ParametersBox.Text = parameters.ToString();
        }

        protected void save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "Parameters.cs";
            save.Filter = "Class File | *.cs";
            bool? userClickedOK = save.ShowDialog();
            if (userClickedOK == true)
            {
                string path = save.FileName;
                File.WriteAllText(path, parameters.ToString());

                WriteAccountStringsToKey();
                MessageBox.Show("Saved Parameters.cs completed", "WeChat Bot Gen");

                this.Close();
            }
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReadAccountStringsFromKey();

            parameters = new StringBuilder("");

            titletext.Text = weixin_title;
            mainurl.Text = weixin_url;
            iconurl.Text = weixin_iconurl;
            defaultmsg.Text = weixin_defaultmsg;
            weixintoken.Text = weixin_token;
            apikey.Text = api_key;

            GenParameters();

            ParametersBox.Text = parameters.ToString();

            titletext.TextChanged += new TextChangedEventHandler(text_TextChanged);
            mainurl.TextChanged += new TextChangedEventHandler(text_TextChanged);
            iconurl.TextChanged += new TextChangedEventHandler(text_TextChanged);
            defaultmsg.TextChanged += new TextChangedEventHandler(text_TextChanged);
            weixintoken.TextChanged += new TextChangedEventHandler(text_TextChanged);
            apikey.TextChanged += new TextChangedEventHandler(text_TextChanged);
        }
    }
}
