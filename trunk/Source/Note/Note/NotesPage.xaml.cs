using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            FlyoutPage.ListViewPage.ItemSelected += ListView_ItemSelected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as NotesPageFlyoutMenuItem;
            if (item == null)
                return;


            if (item.Title == "添加空白书页")
            {
                MyNoteBook = new Notes();
                MyNoteBook.Title = "空白书页";
                Detail = new NavigationPage(MyNoteBook);

            }
            else
            {
                MyNoteBook = new Notes(item.Id);
                MyNoteBook.Title = item.Title;
                Detail = new NavigationPage(MyNoteBook);
            }

            IsPresented = false;

            FlyoutPage.ListViewPage.SelectedItem = null;

        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            var Pathlist = MyNoteBook.PathList;
            var description = MyNoteBook.Description;
            if (Pathlist == null && description == "")
            {
                await DisplayAlert("Tips", "没有需要保存的内容", "确定");
            }
            string title = MyNoteBook.Title;
            int id = 0;
            if (!string.IsNullOrEmpty(MyNoteBook.AutomationId))
            {
                id = Convert.ToInt32(MyNoteBook.AutomationId);//使用AutomationId作为id
            }
            if (id != 0)//修改
            {
                var Result = SqliteHelper.ExecuteQueryEqualsTitle(title, id);
                var IsUpdate = Result.Item1;
                var NewPathList = new ArrayList { };
                if (IsUpdate)
                {//如果修改了标题，判断之前标题的文件夹是否存在，如果存在就修改该文件夹的名称为新标题。若不存在则新建一个文件夹，命名为新标题，把文件拷贝进去
                    var PicFPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    var TargetFolderPath = Path.Combine(PicFPath, title);//目标路径
                    foreach (string FilePath in Pathlist)
                    {
                        if (!Directory.Exists(TargetFolderPath))
                        {
                            Directory.CreateDirectory(TargetFolderPath);
                        }
                        var TargetFilePath = Path.Combine(TargetFolderPath, Path.GetFileName(FilePath));
                        File.Move(FilePath, TargetFilePath);
                        NewPathList.Add(TargetFilePath);
                    }
                    if(NewPathList!= null && NewPathList.Count > 0)
                    {
                        Pathlist.Clear();
                        Pathlist = NewPathList;
                    }
                }
                SqliteHelper.ExecuteQueryEqualsDetil(description, id);
                SqliteHelper.ExecuteQueryEqualsPicList(Pathlist, id);
            }
            else//不存在
            {
                SqliteHelper.ExecuteNonQuery(new SqliteHelper.NoteInfo() { name = title });
                var mid = SqliteHelper.ExecuteQueryGetRowID("select id from NoteInfo order by id desc", SqliteHelper.DBTable.NoteInfo).Item1;
                SqliteHelper.ExecuteNonQuery(new SqliteHelper.TextInfo() { TextDetil = description, Noteid = mid });
                foreach (string path in Pathlist)
                {
                    SqliteHelper.ExecuteNonQuery(new SqliteHelper.PicInfo() { PicPath = path, Noteid = mid });
                }
                MyNoteBook.AutomationId = Convert.ToString(mid);
            }
            FlyoutPage.reload();
            await DisplayAlert("Tips", "保存成功", "确定");
        }
    }
}