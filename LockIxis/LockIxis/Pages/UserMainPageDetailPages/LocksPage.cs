using LockIxis.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LockIxis.Pages.UserMainPageDetailPages
{
    public class LocksPage : AbstractUserMainPageDetailsPage
    {
        public LocksPage(MasterDetailPage root) : base(root, "Owned Locks")
        {
            BindingContext = new LocksViewModel();
            //InitializeComponent();
            var stacklayout = new StackLayout();
            var listview = new ListView();

            var b = new Binding("ListViewItems")
            {
                Source = BindingContext
            };
            listview.SetBinding(ListView.ItemsSourceProperty, b);

            var grid = new Grid();

            grid.RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition() { Height = GridLength.Auto},
                new RowDefinition() { Height = GridLength.Auto},
                new RowDefinition() { Height = GridLength.Auto},
            };

            stacklayout.Children.Add(listview);

            _grid.Children.Add(stacklayout, 0, 1);

            Content = _grid;

        }

        
    }

    //   <StackLayout Grid.Row="1"
    //             VerticalOptions= "StartAndExpand" HorizontalOptions= "Fill" >
    //  < !--< Button Text= "Add" Clicked= "OnAddButtonClicked" /> -->
    //  < ListView ItemsSource= "{Binding ListViewItems}"
    //            ItemSelected= "HandleItemSelected"
    //            VerticalOptions= "FillAndExpand"
    //            HorizontalOptions= "FillAndExpand" >
    //    < ListView.ItemTemplate >
    //      < DataTemplate >
    //        < ViewCell >
    //          < !--< ViewCell.ContextActions >
    //          < MenuItem Clicked= "OnDelete" CommandParameter= "{Binding .}"
    //             Text= "Delete" IsDestructive= "True" />
    //        </ ViewCell.ContextActions > -->
    //          < StackLayout Padding= "15,0" >
    //            < Label Text= "{Binding .}" />
    //          </ StackLayout >
    //        </ ViewCell >
    //      </ DataTemplate >
    //    </ ListView.ItemTemplate >
    //  </ ListView >
    //</ StackLayout >
}
