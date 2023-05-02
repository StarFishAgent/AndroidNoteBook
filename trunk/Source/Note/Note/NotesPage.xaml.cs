﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plugin.Media;
using Plugin.Media.Abstractions;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Note
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotesPage : FlyoutPage
    {
        public NotesPage()
        {
            InitializeComponent();
            FlyoutPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as NotesPageFlyoutMenuItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            FlyoutPage.ListView.SelectedItem = null;
        }

        private async void btnNew_Clicked(object sender, EventArgs e) {
            SqliteHelper.CreateDb();
            SqliteHelper.ExecuteNonQuery(new SqliteHelper.NoteInfo() { name = "hello" });
           var dt =  SqliteHelper.ExecuteQuery("select * from noteinfo order by id asc");
           var i =  dt.Item1.Rows.Count;
        }
    }
}