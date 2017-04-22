using LockIxis.Pages.UserMainPageDetailPages;
using LockIxis.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LockIxis.Pages
{
	public partial class UserMainPage : MasterDetailPage
	{
        private User _user;
        public UserMainPage(User u)
		{
            _user = u;

            BindingContext = new UserMainViewModel(_user);
            this.Detail = new LockActionsPage(this);
			InitializeComponent ();
		}

        void ViewCurrentTransactionsClicked(object sender, EventArgs e)
        {
        }

        private void ShowMasterButtonClick(object sender, EventArgs e)
        {
            IsPresented = !IsPresented;
        }

        void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            var label = sender as Label;
            if (label != null)
            {
                this.Detail.Navigation.PushAsync(new LocksPage(this));

                IsPresented = !IsPresented;
            }
        }
    }
}
