using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LockIxis.ViewModels
{
    public class LocksViewModel : BaseViewModel
    {
        private ObservableCollection<string> _listviewitems = new ObservableCollection<string>();
        public ObservableCollection<string> ListViewItems
        {
            get
            {
                return _listviewitems;
            }
            set
            {
                _listviewitems = value;
            }
        }

        public LocksViewModel()
        {
            User u = LockIxisApp.CurrentUser();
            ListViewItems = new ObservableCollection<string>(u.Locks);
        }
    }
}
