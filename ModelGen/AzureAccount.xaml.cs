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
using System.Windows;

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
