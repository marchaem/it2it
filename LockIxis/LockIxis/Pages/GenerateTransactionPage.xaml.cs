using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing;

namespace LockIxis.Pages
{
	public partial class GenerateTransactionPage : ContentPage
	{
        public QRCodeScanningPage _qrCodeScanningPage;

        public ObservableCollection<string> ListViewItems { get; set; }

        public GenerateTransactionPage ()
		{
            BindingContext = this;
            ListViewItems = new ObservableCollection<string> { "Add a party" };
            _qrCodeScanningPage = new QRCodeScanningPage(AddNewAddress);
            InitializeComponent();
		}

        private void OnAddButtonClicked(object sender, EventArgs e)
        {
            ListViewItems.Add(DateTime.Now.ToString());
            // OnPropertyChanged("ListViewItems");
        }

        private async void HandleItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if ((e.SelectedItem == null) || !e.SelectedItem.Equals("Add a party"))
            {
                return;
            }
            NavigationPage.SetHasNavigationBar(_qrCodeScanningPage, false);
            await LockIxisApp.GetInstance().MainPage.Navigation.PushAsync(_qrCodeScanningPage);
        }

        public void AddNewAddress(Result r)
        {
            try
            {
                _qrCodeScanningPage.IsScanning = false;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await LockIxisApp.GetInstance().MainPage.Navigation.PopAsync();
                    string lock_address = r.Text;
                    if (lock_address.Length > 0)
                    {
                        ListViewItems.Insert(0, lock_address);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
