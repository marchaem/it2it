using LockIxis.Ethereum;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing;

namespace LockIxis.Pages
{
	public partial class GenerateTransactionPage : ContentPage
	{
        public QRCodeScanningPage _qrCodeScanningPage;
        static public string transactionId;
        public ObservableCollection<User> ListViewItems { get; set; }
        static string blankuseraddress = "Add a Party";
        private string _lockaddress;
        bool isGenerated = false;

        static User blankUser = new User() { PublicKey = blankuseraddress };

        private bool IsListEmpty
        {
            get
            {
                if(ListViewItems.Count > 1)
                {
                    return false;
                }
                else if(ListViewItems.Count == 1)
                {
                    if(ListViewItems.First().PublicKey.Equals(blankuseraddress))
                        return true;
                }
                throw new Exception("Should not be empty.");
            }
        }

        public GenerateTransactionPage (string lock_address)
		{
            _lockaddress = lock_address;
            BindingContext = this;
            ListViewItems = new ObservableCollection<User> { blankUser };
            _qrCodeScanningPage = new QRCodeScanningPage(AddNewAddress);
            isGenerated = false;
            InitializeComponent();
		}

        private async void HandleItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedUser = e.SelectedItem as User;
            if (e.SelectedItem != null && selectedUser != null)
            {
                if (!selectedUser.PublicKey.Equals(blankuseraddress))
                {
                    return;
                }
                NavigationPage.SetHasNavigationBar(_qrCodeScanningPage, false);
                await LockIxisApp.GetInstance().MainPage.Navigation.PushAsync(_qrCodeScanningPage);
            }
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
                        var newUser = new User();
                        newUser.PublicKey = lock_address;
                        ListViewItems.Insert(0, newUser);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void OnEdit(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            DisplayAlert("More Context Action", mi.CommandParameter + " more context action", "OK");
        }

        public void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var u = mi.CommandParameter as User;
            if(u != null)
            {
                if(!u.PublicKey.Equals(blankuseraddress))
                    ListViewItems.Remove(u);
            }
        }

        

        public async void OnGenerateTransaction(object sender, EventArgs e)
        {
            if (!IsListEmpty)
            {
                #region smart contract deployment

                //we unlock and deploy the contract with our geth account and we add the lock
                await LockIxisApp.CurrentUser().Locker.unlock();
                await LockIxisApp.CurrentUser().Locker.getContract();
                await LockIxisApp.CurrentUser().AddLock(transactionId,_lockaddress);
                LockIxisApp.CurrentUser().Locker.updateTransactionId(transactionId);
                await DisplayAlert("Transaction has been generated", "Transaction has been generated", "OK");
                await LockIxisApp.GetInstance().MainPage.Navigation.PopAsync();
                
                #endregion
            }
            else
            {
                await DisplayAlert("List is empty", "List is empty", "OK");
            }
        }
    }
}
