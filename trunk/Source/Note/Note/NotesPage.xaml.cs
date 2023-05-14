using System;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            string title = MyNoteBook.Title;
            var id = Convert.ToInt32(MyNoteBook.AutomationId);
            var Pathlist = MyNoteBook.PathList;
            var description = MyNoteBook.Description;
            if (Pathlist==null && description == "")
            {
                await DisplayAlert("Tips", "没有需要保存的内容", "确定");
            }
            if (id != 0)//修改
            {
                //isExist = SqliteHelper.IsExist("id", id, SqliteHelper.DBTable.NoteInfo).Item1;
                SqliteHelper.ExecuteQueryEquals(description, id);
            }
            else//不存在
            {
                SqliteHelper.ExecuteNonQuery(new SqliteHelper.NoteInfo() { name = title });
                var mid = SqliteHelper.ExecuteQueryGetRowID("select id from NoteInfo order by id desc", SqliteHelper.DBTable.NoteInfo).Item1;
                SqliteHelper.ExecuteNonQuery(new SqliteHelper.TextInfo() { TextDetil = description ,Noteid=mid});
                foreach (string path in Pathlist)
                {
                    SqliteHelper.ExecuteNonQuery(new SqliteHelper.PicInfo() { PicPath = path, Noteid = mid });
                }
            }
            await DisplayAlert("Tips", "保存成功", "确定");
        }
    }
}