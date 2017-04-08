using LockIxis.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace LockIxis.Pages
{
	public partial class RegisterPage : ContentPage
	{
		public RegisterPage()
		{
            BindingContext = new LoginViewModel(this);
            InitializeComponent ();
		}
    }
}
