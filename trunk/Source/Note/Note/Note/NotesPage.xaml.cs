using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android;
using Android.Content.PM;

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
            //string pagetitle = await DisplayPromptAsync("新建标签", "请输入标签的名称");
            //if (pagetitle == "") {
            //    await DisplayAlert("提示", "标签的名称不能为空", "OK");
            //}
            //MyNoteBook.Save();
            //MyNoteBook.Title = pagetitle;
            
            SqliteHelper.CreateSqliteDatabase();
        }
    }
}