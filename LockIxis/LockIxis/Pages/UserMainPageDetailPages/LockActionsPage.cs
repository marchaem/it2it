using Android.Bluetooth;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing;

namespace LockIxis.Pages.UserMainPageDetailPages
{
    public class LockActionsPage : AbstractUserMainPageDetailsPage
    {
        QRCodeScanningPage _qrscanpageOnActionLock;
        QRCodeScanningPage _qrscanpageOnGenerateTransaction;
        //TextView _textview;

        public LockActionsPage(MasterDetailPage root) : base(root, "Actions")
        {
            _qrscanpageOnActionLock = new QRCodeScanningPage(ActionLockClickedScanDelegate);
            _qrscanpageOnGenerateTransaction = new QRCodeScanningPage(GenerateTransactionClickedScanDelegate);

            var stacklayout = new StackLayout();
            var grid = new Grid();

            grid.RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition() { Height = GridLength.Auto},
                new RowDefinition() { Height = GridLength.Auto},
                new RowDefinition() { Height = GridLength.Auto},
            };

            var actionLockButton = new Xamarin.Forms.Button()
            {
                Text = "Action a Lock",
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            var generateTransactionButton = new Xamarin.Forms.Button()
            {
                Text = "Generate a transaction",
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            actionLockButton.Clicked += ActionLockClicked;
            generateTransactionButton.Clicked += GenerateTransactionClicked;

            grid.Children.Add(actionLockButton, 0, 0);
            grid.Children.Add(generateTransactionButton, 0, 1);

            stacklayout.Children.Add(grid);

            _grid.Children.Add(stacklayout, 0, 1);
            //_grid.Children.Add(_textview, 0, 1);

            Content = _grid;
            //Toast.MakeText(Android.App.Application.Context, "ouhlalaa", ToastLength.Long).Show();

        }

        async void ActionLockClicked(object sender, EventArgs eventargs)
        {
            NavigationPage.SetHasNavigationBar(_qrscanpageOnActionLock, false);
            await LockIxisApp.GetInstance().MainPage.Navigation.PushAsync(_qrscanpageOnActionLock);
        }

        async void GenerateTransactionClicked(object sender, EventArgs e)
        {
            NavigationPage.SetHasNavigationBar(_qrscanpageOnGenerateTransaction, false);
            await LockIxisApp.GetInstance().MainPage.Navigation.PushAsync(_qrscanpageOnGenerateTransaction);
        }

        public void ActionLockClickedScanDelegate(Result r)
        {
            try
            {
                _qrscanpageOnActionLock.IsScanning = false;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await LockIxisApp.GetInstance().MainPage.Navigation.PopAsync();
                    string lock_address = r.Text;
                    if (lock_address.Length > 0)
                    {
                        var answer = await DisplayAlert("Do you want to action this lock", r.Text, "Yes", "No");
                        if (answer == true)
                        {
                            await DisplayAlert("", String.Format("Connecting to {0}", lock_address), "OK");
                            var btOk = await CheckBt();
                            if (btOk)
                            {
                                var bt = LockIxisApp.GetBluetoothAdapter();
                                var devicesNames = bt.BondedDevices.Select(d => d.Name).ToArray();
                                var selectedDeviceName = await DisplayActionSheet("Select the Lock you want to connect to:", "Cancel", "", devicesNames);
                                if (selectedDeviceName != null && selectedDeviceName.Length > 0)
                                {
                                    var selectedDevice = bt.BondedDevices.FirstOrDefault(d => d.Name.Equals(selectedDeviceName));
                                    //await DisplayAlert("blabla", selectedDeviceName, "OK");
                                    var selectedLock = new ConnectedLock(lock_address, selectedDevice);
                                    if (selectedLock.IsConnected)
                                    {
                                        var connected = Connect(selectedLock);
                                        if (connected)
                                        {
                                            await LockIxisApp.GetInstance().MainPage.Navigation.PushAsync(new ConnectedLockPage(_rootpage, selectedLock));
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Toast.MakeText(Android.App.Application.Context, e.Message, ToastLength.Long);
            }
        }

        public void GenerateTransactionClickedScanDelegate(Result r)
        {
            try
            {
                _qrscanpageOnGenerateTransaction.IsScanning = false;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await LockIxisApp.GetInstance().MainPage.Navigation.PopAsync();
                    string lock_address = r.Text;
                    if (lock_address.Length > 0)
                    {
                        var answer = await DisplayAlert("Do you want to generate a transaction for this lock", r.Text, "Yes", "No");
                        if (answer == true)
                        {
                            //await DisplayAlert("", String.Format("Connecting to {0}", lock_address), "OK");
                            var generateTransactionPage = new GenerateTransactionPage(lock_address);
                            await LockIxisApp.GetInstance().MainPage.Navigation.PushAsync(generateTransactionPage);

                        }
                    }
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private async Task<bool> CheckBt()
        {
            var bt = LockIxisApp.GetBluetoothAdapter();

            if (bt == null)
            {
                await DisplayAlert("Error", "Bluetooth is not supported or activated", "OK");
                return false;
            }

            return true;
        }

        public bool Connect(ConnectedLock locktoconnect)
        {
            var deviceName = locktoconnect.BTDeviceName;

            if (deviceName != null && deviceName.Equals("HC-05"))
            {
                var btSocket = LockIxisApp.GetBTSocket() ?? LockIxisApp.SetAndGetBTSocket(locktoconnect.BTDevice.CreateRfcommSocketToServiceRecord(LockIxisApp.MY_UUID));
                try
                {
                    if (!btSocket.IsConnected)
                    {
                        btSocket.Connect();
                        Toast.MakeText(Android.App.Application.Context, "Connected to HC-05", ToastLength.Long).Show();
                    }
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                    try
                    {
                        btSocket.Close();
                    }
                    catch (System.Exception)
                    {
                        Toast.MakeText(Android.App.Application.Context, "Socket error on close", ToastLength.Long).Show();
                    }
                    Toast.MakeText(Android.App.Application.Context, "Socket failed to create", ToastLength.Long).Show();
                    return false;
                }
                //beginListenForData(btSocket);
                return true;
            }
            //_textview.Text = "No Lock Found in bluetooth environment (HC-05)";
            return false;
        }

        //    public void beginListenForData(BluetoothSocket btSocket)
        //    {
        //        Stream inStream = null;
        //        try
        //        {
        //            inStream = btSocket.InputStream;
        //        }
        //        catch (System.IO.IOException ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //        //_textview.Text = "Lock: init communication";
        //        Task.Factory.StartNew(() => {
        //            byte[] buffer = new byte[1024];
        //            byte[] bufferCumulate = new byte[1024];
        //            int[] len = new int[10];
        //            int lenIndex = 0;
        //            int cumul = 0;
        //            int bytes;
        //            bool eol = false;
        //            while (true)
        //            {
        //                try
        //                {
        //                    bytes = inStream.Read(buffer, 0, buffer.Length);
        //                    len[lenIndex++] = bytes;
        //                    for (int i = 0; i < bytes; i++)
        //                    {
        //                        if (buffer[i] == '\n')
        //                        {
        //                            eol = true;
        //                        }
        //                    }
        //                    Buffer.BlockCopy(buffer, 0,
        //                                    bufferCumulate, cumul,
        //                                        bytes);
        //                    cumul += bytes;
        //                    bufferCumulate[cumul] = 0;
        //                    if (eol)
        //                    {
        //                        string valor = System.Text.Encoding.ASCII.GetString(bufferCumulate);
        //                        eol = false;
        //                        cumul = 0;
        //                        lenIndex = 0;
        //                        Device.BeginInvokeOnMainThread(() => {
        //                            //Toast.MakeText(Android.App.Application.Context, 
        //                            // Result.Text = Result.Text + "\n" + valor;
        //                            //_textview.Text = valor;
        //                        });
        //                    }
        //                }
        //                catch (Java.IO.IOException)
        //                {
        //                    Device.BeginInvokeOnMainThread(() => {
        //                        //_textview.Text = "Lock: end of communication";
        //                    });
        //                    break;
        //                }
        //            }
        //        });
        //    }

        //    private void writeData(string data)
        //    {
        //        try
        //        {
        //            var btSocket = LockIxisApp.GetBTSocket();
        //            if (btSocket != null)
        //            {
        //                byte[] msgBuffer = new byte[data.Length * sizeof(char)];
        //                System.Buffer.BlockCopy(data.ToCharArray(), 0, msgBuffer, 0, msgBuffer.Length);
        //                var outStream = btSocket.OutputStream;
        //                outStream.Write(msgBuffer, 0, msgBuffer.Length);
        //            }
        //            else
        //            {
        //                throw new Exception("!!! Could not retrieve Socket !!!");
        //            }
        //        }
        //        catch (System.Exception e)
        //        {
        //            Toast.MakeText(Android.App.Application.Context, e.Message, ToastLength.Long);
        //        }
        //    }
        //}
    }
}
