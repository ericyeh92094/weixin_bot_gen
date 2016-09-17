using Microsoft.Win32;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DataAccess;

namespace ModelGen
{
    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Always test MultiValueConverter inputs for non-null 
            // (to avoid crash bugs for views in the designer) 
            if (values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool)values[0];
                bool hasFocus = (bool)values[1];
                if (hasFocus || hasText)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ModelListViewItem
    {
        public string modelName { get; set; }
        public string appId { get; set; }
    }
    /// <summary>
    /// Interaction logic for ModelGenWindow.xaml
    /// </summary>
    public partial class ModelGenWindow : Window
    {
        public static string azure_storage_account = "";
        public static string azure_storgae_subkey = "";
        public static string LUIS_subkey = "";
        public static string azure_storage_table = "";

        static string subregKey = "SOFTWARE\\BOTMODELGEN";
        static string KeyName_account = "STORAGE_ACCOUNT";
        static string KeyName_subkey = "STORAGE_SUBKEY";
        static string KeyName_domain = "STORAGE_EDNPOINT_DOMAIN";
        static string KeyName_LUIS_subkey = "LUIS_SUBKEY";
        static string KeyName_modelcount = "LUIS_MODELCOUNT";
        static string KeyName_model_key = "LUIS_MODELKEY{0}";
        static string KeyName_model_name = "LUIS_MODELNAME{0}";

        static public int model_num = 0;
        static public string[] model_sub_key;
        static public string data_path;

        private int m_iColumnCount = 0;
        private System.Data.DataTable m_dtCSV;
        private string[] columnName;

        public ModelGenWindow()
        {
            InitializeComponent();
            if (ReadAccountStringsFromKey())
            {
                copy.IsEnabled = (model_num > 0) ? true : false;
                this.listView.Items.Clear();
                for (int i = 0; i < model_num; i++)
                {
                    this.listView.Items.Add(new ModelListViewItem { modelName = LUISGen.appNames[i], appId = LUISGen.appIds[i] });
                }
            }
        }

        public static bool ReadAccountStringsFromKey()
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
                    azure_storage_account = (string)sk1.GetValue(KeyName_account);
                    azure_storgae_subkey = (string)sk1.GetValue(KeyName_subkey);
                    azure_storage_table = (string)sk1.GetValue(KeyName_domain);
                    LUIS_subkey = (string)sk1.GetValue(KeyName_LUIS_subkey);
                    model_num = (int)sk1.GetValue(KeyName_modelcount);

                    if (model_num > 0)
                    {
                        LUISGen.initModelVar(model_num)
                            ;
                        for (int i = 0; i < model_num; i++)
                        {
                            LUISGen.appIds[i] = (string)sk1.GetValue(string.Format(KeyName_model_key, i));
                            LUISGen.appNames[i] = (string)sk1.GetValue(string.Format(KeyName_model_name, i));
                        }
                    }

                }
                catch (Exception e)
                {
                    return false;
                }
                return true;
            }
        }
        public static bool WriteAccountStringsToKey()
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser;
                RegistryKey sk1 = rk.CreateSubKey(subregKey);
                // Save the value
                sk1.SetValue(KeyName_account, azure_storage_account);
                sk1.SetValue(KeyName_subkey, azure_storgae_subkey);
                sk1.SetValue(KeyName_domain, azure_storage_table);
                sk1.SetValue(KeyName_LUIS_subkey, LUIS_subkey);
                sk1.SetValue(KeyName_modelcount, model_num);

                for (int i = 0; i < model_num; i++)
                {
                    sk1.SetValue(string.Format(KeyName_model_key, i), LUISGen.appIds[i]);
                    sk1.SetValue(string.Format(KeyName_model_name, i), LUISGen.appNames[i]);
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private void account_Click(object sender, RoutedEventArgs e)
        {
            AzureAccount wnd = new AzureAccount();
            wnd.ShowDialog();
        }

        private void bOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "CSV Files (.csv)|*.csv|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                data_path = openFileDialog1.FileName;
                datapath.Text = data_path;
                System.IO.Stream fileStream = openFileDialog1.OpenFile();
                PopulateDataTableFromUploadedFile(fileStream);
                fileStream.Close();
            }
        }

        private void PopulateDataTableFromUploadedFile(System.IO.Stream strm)
        {
            System.IO.StreamReader srdr = new System.IO.StreamReader(strm);
            String strLine = String.Empty;
            Int32 iLineCount = 0;
            do
            {
                strLine = srdr.ReadLine();
                if (strLine == null)
                {
                    break;
                }

                if (0 == iLineCount++)
                {
                    m_dtCSV = this.CreateDataTableForCSVData(strLine);
                }
                else
                    this.AddDataRowToTable(strLine, m_dtCSV);

            } while (true);

            dataGrid.ItemsSource = m_dtCSV.DefaultView;

            for (int i = 0; i < m_iColumnCount; i++)
                dataGrid.Columns[i].Header = columnName[i];

            //dataGrid.AutoGenerateColumns = true;
        }

        private System.Data.DataTable CreateDataTableForCSVData(String strLine)
        {
            System.Data.DataTable dt = new System.Data.DataTable("CSVTable");
            String[] strVals = strLine.Split(new char[] { ',' });
            m_iColumnCount = strVals.Length;
            columnName = new string[20];

            int idx = 0;
            foreach (String strVal in strVals)
            {
                String strColumnName = String.Format("Column-{0}", idx);
                dt.Columns.Add(strColumnName, Type.GetType("System.String"));
                columnName[idx] = strVal;
                idx++;
            }
            return dt;
        }

        private DataRow AddDataRowToTable(String strCSVLine, System.Data.DataTable dt)
        {
            String[] strVals = strCSVLine.Split(new char[] { ',' });
            Int32 iTotalNumberOfValues = strVals.Length;
            // If number of values in this line are more than the columns
            // currently in table, then we need to add more columns to table.
            if (iTotalNumberOfValues > m_iColumnCount)
            {
                Int32 iDiff = iTotalNumberOfValues - m_iColumnCount;
                for (Int32 i = 0; i < iDiff; i++)
                {
                    String strColumnName = String.Format("Column-{0}", (m_iColumnCount + i));
                    dt.Columns.Add(strColumnName, Type.GetType("System.String"));
                }
                m_iColumnCount = iTotalNumberOfValues;
            }
            int idx = 0;
            DataRow drow = dt.NewRow();
            foreach (String strVal in strVals)
            {
                //String strColumnName = String.Format("Column-{0}", idx++);
                string str = strVal.Replace("\"", "");
                drow[idx] = str;
                idx++;
            }
            dt.Rows.Add(drow);
            return drow;
        }

        private async Task<bool> TrainModel(bool showinlist)
        {
            for (int i = 0; i < model_num; i++)
            {
                await LUISGen.TrainModelRequest(LUISGen.appIds[i]);
                bool result = false;
                int times = 0;
                while (result)
                {
                    result = await LUISGen.TrainModelStatus(LUISGen.appIds[i]);
                    await Task.Delay(2000);
                    if (times++ > 20)
                        return false;
                }

                await LUISGen.PublishModelRequest(LUISGen.appIds[i]);

                if (showinlist) this.listView.Items.Add(new ModelListViewItem { modelName = LUISGen.appNames[i], appId = LUISGen.appIds[i] });
                this.progressbar.Value++;
            }
            return true;
        }

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            account.IsEnabled = false;
            pickfile.IsEnabled = false;

            bool training_only = false;

            if (model_num > 0)
            {
                string message = "You have models generated. Yes = Re-generate models, No = train/publish models only ";
                string caption = "Models Generated";
                MessageBoxButton buttons = MessageBoxButton.YesNoCancel;
                MessageBoxResult result;

                result = MessageBox.Show(message, caption, buttons);

                if (result == MessageBoxResult.No)
                {
                    this.progressbar.Maximum = model_num;
                    this.progressbar.Value = 0;

                    status.Content = "Training Models ...";

                    bool r = await TrainModel(false);
                    if (r)
                        status.Content = "Model Training Completed";
                    else
                        status.Content = "Model Training Failed";

                    this.progressbar.Value = 0;
                    training_only = true;
                }
                else if (result == MessageBoxResult.Yes)
                    training_only = false;
                else //Cancel the op
                    training_only = true;

            }

            if (!training_only)
            { 
                LUISGen.progBar = this.progressbar;
                LUISGen.progBar.Value = 0;

                status.Content = "Creating Model ...";
                model_num = await LUISGen.GenerateModels(m_dtCSV);

                this.listView.Items.Clear();
                bool result = await TrainModel(true);
                if (result)
                    status.Content = "Model Creation Completed";
                else
                    status.Content = "Model Creation Failed";

                WriteAccountStringsToKey();
            }

            btn.IsEnabled = true;
            copy.IsEnabled = true;
            account.IsEnabled = true;
            pickfile.IsEnabled = true;

           
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            ParaGenWindow wnd = new ParaGenWindow();
            wnd.ShowDialog();
        }

        private void uploadtable_Click(object sender, RoutedEventArgs e)
        {
            CloudStorageAccount cloudStorageAccount;
            string connStr = "DefaultEndpointsProtocol=http;AccountName=" + azure_storage_account + ";AccountKey=" + azure_storgae_subkey;
            cloudStorageAccount = CloudStorageAccount.Parse(connStr);

            var dt = DataAccess.DataTable.New.ReadCsv(data_path);
            dt.SaveToAzureTable(cloudStorageAccount, azure_storage_table);

            MessageBox.Show("Upload CSV completed", "WeChat Bot Gen");
        }
    }
}
