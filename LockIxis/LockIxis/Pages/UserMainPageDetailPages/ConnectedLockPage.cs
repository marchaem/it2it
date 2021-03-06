﻿using Android.Bluetooth;
using Android.Widget;
using LockIxis.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LockIxis.Pages.UserMainPageDetailPages
{
    public class ConnectedLockPage : AbstractUserMainPageDetailsPage 
    {
        private static BluetoothSocket _btSocket = LockIxisApp.GetBTSocket();
        private ConnectedLock _lock;
        Stream _stream;
        private string _lockStatus = "";

        public ConnectedLockPage(MasterDetailPage root, ConnectedLock locktoconnect) : base(root, "Lock")
        {
            _rootpage = root;
            _lock = locktoconnect;
            _stream = _btSocket.InputStream;
            var getLockIdButton = new Xamarin.Forms.Button()
            {
                Text = "Get lock ID",
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            var getLockStateButton = new Xamarin.Forms.Button()
            {
                Text = "Get lock state",
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            var openLockButton = new Xamarin.Forms.Button()
            {
                Text = "Open lock",
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            var closeLockButton = new Xamarin.Forms.Button()
            {
                Text = "Close lock",
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            BindingContext = new ConnectedLockViewModel(_lock);

            var text = new Label();
            text.BindingContext = this;
            text.SetBinding(Label.TextProperty, "Text");

            //text.SetBinding(Label.TextProperty, "Text");

            getLockIdButton.Clicked += GetLockIdClicked;
            getLockStateButton.Clicked += GetLockStateClicked;
            openLockButton.Clicked += OpenLockClicked;
            closeLockButton.Clicked += CloseLockClicked;

            var stacklayout = new StackLayout();

            var grid = new Grid();
            grid.Children.Add(getLockIdButton, 0, 0);
            grid.Children.Add(getLockStateButton, 0, 1);
            grid.Children.Add(openLockButton, 0, 2);
            grid.Children.Add(closeLockButton, 0, 3);

            stacklayout.Children.Add(grid);
            stacklayout.Children.Add(text);

            _grid.Children.Add(stacklayout, 0, 1);
            Content = _grid;

            var cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => backgroundListener(),
                                  CancellationToken.None, 
                                  TaskCreationOptions.LongRunning, 
                                  TaskScheduler.Default);

            writeData(new Java.Lang.String("state\n"));
        }

        public void backgroundListener()
        {
            byte[] buffer = new byte[1024];
            byte[] bufferCumulate = new byte[1024];
            int cumul = 0;
            int bytes;
            bool eol = false;

            while (true)
            {
                try
                {
                    bytes = _stream.Read(buffer, 0, buffer.Length);
                    for (int i = 0; i < bytes; i++)
                    {
                        if (buffer[i] == '\n')
                        {
                            eol = true;
                        }
                    }
                    Buffer.BlockCopy(buffer, 0,
                                    bufferCumulate, cumul,
                                        bytes);
                    cumul += bytes;
                    bufferCumulate[cumul] = 0;

                    if (eol)
                    {
                        string valor = System.Text.Encoding.ASCII.GetString(bufferCumulate);
                        bufferCumulate = new byte[1024];
                        buffer = new byte[1024];
                        eol = false;
                        cumul = 0;
                        if (valor.ToLower().Contains("open"))
                        {
                            _lock.Status = "Open";
                            Status = _lock.Status;
                        }
                        if (valor.ToLower().Contains("closed"))
                        {
                            _lock.Status = "Closed";
                            Status = _lock.Status;
                        }
                        Device.BeginInvokeOnMainThread(() => {

                            Toast.MakeText(Android.App.Application.Context, valor, ToastLength.Short).Show();

                        });
                    }
                }
                catch (Java.IO.IOException e)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        Toast.MakeText(Android.App.Application.Context, e.Message, ToastLength.Long).Show();
                    });
                    break;
                }
            }
        }

        public string Text
        {
            get
            {
                //return "empty";
                return String.Format("Lock: \n*Public key:{2}\n*Bluetooth device Name:{0}\n*Bluetooth device Address:{1}\n*Status:{3}\n", _lock.BTDeviceName, _lock.BTDeviceAddress, _lock.PublicKey, Status);
            }
        }

        public string Status
        {
            get { return _lockStatus; }
            set
            {
                _lockStatus = value;
                OnPropertyChanged("Text");
            }
        }



        private void writeData(Java.Lang.String data)
        {
            try
            {
                var btSocket = LockIxisApp.GetBTSocket();
                if (btSocket != null)
                {
                    byte[] msgBuffer = data.GetBytes();
                    btSocket.OutputStream.Write(msgBuffer, 0, msgBuffer.Length);
                }
                else
                {
                    throw new Exception("Could not retrieve Socket");
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error send" + e.Message);
            }
        }

        private void GetLockIdClicked(object sender, EventArgs eventargs)
        {
            writeData(new Java.Lang.String("ID\n"));
        }

        private void GetLockStateClicked(object sender, EventArgs eventargs)
        {
            writeData(new Java.Lang.String("state\n"));
        }

        private void OpenLockClicked(object sender, EventArgs eventargs)
        {
            writeData(new Java.Lang.String("open\n"));
        }

        private void CloseLockClicked(object sender, EventArgs eventargs)
        {
            writeData(new Java.Lang.String("close\n"));
        }
    }
}
