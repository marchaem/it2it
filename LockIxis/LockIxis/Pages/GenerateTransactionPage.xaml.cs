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

        public ObservableCollection<User> ListViewItems { get; set; }
        static string blankuseraddress = "Add a Party";
        private string _lockaddress;

        static User blankUser = new User() { PublicKey = blankuseraddress };

        Function _multiplyFunction2;

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
                var senderAddress = LockIxisApp.CurrentUser().PublicKey;
                var password = "jll";
                var abi = @"[{""constant"":false,""inputs"":[{""name"":""val"",""type"":""int256""}],""name"":""multiply"",""outputs"":[{""name"":""d"",""type"":""int256""}],""type"":""function""},{""inputs"":[{""name"":""multiplier"",""type"":""int256""}],""type"":""constructor""}]";
                var abi2 = @"[{""constant"":false,""inputs"":[{""name"":""val"",""type"":""int256""}],""name"":""multiply"",""outputs"":[{""name"":""d"",""type"":""int256""}],""type"":""function""},{""inputs"":[{""name"":""multiplier"",""type"":""int256""}],""type"":""constructor""}]";
                var byteCode =
                    "0x60606040526040516020806052833950608060405251600081905550602b8060276000396000f3606060405260e060020a60003504631df4f1448114601a575b005b600054600435026060908152602090f3";

                var web3 = new Web3(@"http://192.168.1.17:8545/");
                //var unlockAccountResult = await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, password, 120);

                var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(abi, byteCode, senderAddress, 3);

                var mineResult = await web3.Miner.Start.SendRequestAsync(6);

                var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

                while (receipt == null)
                {
                    Thread.Sleep(5000);
                    receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                }

                mineResult = await web3.Miner.Stop.SendRequestAsync();

                var contractAddress = receipt.ContractAddress;

                var contract = web3.Eth.GetContract(abi, receipt.ContractAddress);

                var f = contract.GetFunction("multiply");

                var result = await f.CallAsync<int>(7);

                if(result == 21)
                {
                    LockIxisApp.CurrentUser().AddLock(_lockaddress);
                    LockIxisApp.CurrentUser().AddTransaction(transactionHash);
                    await DisplayAlert("Transaction has been generated", "Transaction has been generated", "OK");
                    await LockIxisApp.GetInstance().MainPage.Navigation.PopAsync();
                }

                #endregion
            }
            else
            {
                await DisplayAlert("List is empty", "List is empty", "OK");
            }
        }
    }
}
