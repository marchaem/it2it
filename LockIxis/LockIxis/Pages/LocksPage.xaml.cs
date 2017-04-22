using LockIxis.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LockIxis.Pages
{
	public partial class LocksPagexaml : ContentPage
	{
		public LocksPagexaml ()
		{
            BindingContext = new LocksViewModel();
			InitializeComponent ();
		}

        private void HandleItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedUser = e.SelectedItem as User;
            if (e.SelectedItem != null && selectedUser != null)
            {
            }
        }
    }
}
