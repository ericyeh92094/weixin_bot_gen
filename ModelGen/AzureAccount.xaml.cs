using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModelGen
{
    /// <summary>
    /// Interaction logic for AzureAccount.xaml
    /// </summary>
    public partial class AzureAccount : Window
    {
        public AzureAccount()
        {
            InitializeComponent();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            storageaccount.Text = ModelGenWindow.azure_storage_account;
            storagesubkey.Text = ModelGenWindow.azure_storgae_subkey;
            endpointdomain.Text = ModelGenWindow.azure_storage_table;
            luissubkey.Text = ModelGenWindow.LUIS_subkey;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            ModelGenWindow.azure_storage_account = storageaccount.Text;
            ModelGenWindow.azure_storgae_subkey = storagesubkey.Text;
            ModelGenWindow.azure_storage_table = endpointdomain.Text;
            ModelGenWindow.LUIS_subkey = luissubkey.Text;

            ModelGenWindow.WriteAccountStringsToKey(); // write strings to reg key
            this.Close();
        }

    }
}
