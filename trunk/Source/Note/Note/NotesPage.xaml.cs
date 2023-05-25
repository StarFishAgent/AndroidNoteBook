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
            bool IsSuccess = false;
            string ErrMsg = "";
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
                var ResultsTitle = SqliteHelper.ExecuteQueryEqualsTitle(title, id);//判断标题是否修改
                if (ResultsTitle.Item1 == false)
                {
                    ErrMsg += ResultsTitle.Item3;
                    IsSuccess = false;
                }
                var IsUpdate = ResultsTitle.Item2;
                var NewPathList = new ArrayList { };
                if (IsUpdate)
                {//如果修改了标题，判断之前标题的文件夹是否存在，如果存在就修改该文件夹的名称为新标题。若不存在则新建一个文件夹，命名为新标题，把文件拷贝进去
                    var PicFPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    var TargetFolderPath = Path.Combine(PicFPath, title);//目标路径
                    foreach (string FilePath in Pathlist)
                    {//修改了标题移动文件
                        if (!Directory.Exists(TargetFolderPath))
                        {
                            Directory.CreateDirectory(TargetFolderPath);
                        }
                        var TargetFilePath = Path.Combine(TargetFolderPath, Path.GetFileName(FilePath));//目标路径
                        File.Move(FilePath, TargetFilePath);//移动文件
                        NewPathList.Add(TargetFilePath);
                    }
                    if (NewPathList != null && NewPathList.Count > 0)//如果移动了文件那么就修改数据库中的路径
                    {
                        Pathlist.Clear();//清空
                        Pathlist = NewPathList;
                    }
                    if (Pathlist != null && Pathlist.Count > 0)//如果移动文件后
                    {
                        if (Directory.Exists(TargetFolderPath))//检查文件夹是否被手动删除
                        {
                            bool hasFilesOrFolders = CheckFilesAndFolders(TargetFolderPath);//判断文件夹内是否有文件或子文件夹

                            if (!hasFilesOrFolders)//文件夹内不存在任何文件或子文件夹
                            {
                                Directory.Delete(Path.Combine(PicFPath, ResultsTitle.Item4));//删除该空文件夹
                            }
                        }
                    }
                }
                var ResultsDetil = SqliteHelper.ExecuteQueryEqualsDetil(description, id);//判断描述文本是否修改
                if (ResultsDetil.Item1 == false)
                {
                    ErrMsg += ResultsDetil.Item3;
                    IsSuccess = false;
                }
                var ResultsPicList = SqliteHelper.ExecuteQueryEqualsPicList(Pathlist, id);//判断图片文件路径是否有变化
                if (ResultsPicList.Item1 == false)
                {
                    ErrMsg += ResultsPicList.Item3;
                    IsSuccess = false;
                }
            }
            else//不存在
            {//添加信息至数据库
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
            if (IsSuccess == true)
            {
                await DisplayAlert("Tips", "保存成功", "确定");
            }
            else
            {
                await DisplayAlert("保存失败", ErrMsg, "确定");
            }
        }

        /// <summary>
        /// 检查文件夹内是否包含文件或子文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        static bool CheckFilesAndFolders(string folderPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            FileSystemInfo[] filesAndFolders = directoryInfo.GetFileSystemInfos();

            if (filesAndFolders.Length > 0)
            {
                return true;
            }

            return false;
        }
    }
}