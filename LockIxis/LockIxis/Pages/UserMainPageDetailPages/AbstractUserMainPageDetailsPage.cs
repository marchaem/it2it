using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LockIxis.Pages.UserMainPageDetailPages
{
    public abstract class AbstractUserMainPageDetailsPage : ContentPage
    {
        protected MasterDetailPage _rootpage;
        protected Grid _grid = new Grid();

        protected AbstractUserMainPageDetailsPage(MasterDetailPage rootpage, string header)
        {
            _rootpage = rootpage;
            _grid.RowSpacing = 10;
            _grid.RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition() { Height = GridLength.Auto},
                new RowDefinition() { Height = GridLength.Auto},
                new RowDefinition() { Height = GridLength.Auto},
            };

            _grid.ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition() { Width = GridLength.Star }
            };

            var boxView = new BoxView() { Color = Color.Silver };
            var label = new Label() { Text = header, VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand };
            var button = new Button()
            {
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Start,
                Text = "☰"

            };
            //button.Font = new Font(button.Font.FontFamily, 24);
            button.Clicked += ShowMasterButtonClick;

            _grid.Children.Add(boxView);
            _grid.Children.Add(label);
            _grid.Children.Add(button);
        }

        public void ShowMasterButtonClick(object sender, EventArgs eventargs)
        {
            _rootpage.IsPresented = !_rootpage.IsPresented;
        }
    }
}
