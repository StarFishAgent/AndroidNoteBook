using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FFImageLoading.Forms;
using FFImageLoading.Helpers.Exif;

using Plugin.Media;
using Plugin.Media.Abstractions;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;


namespace Note
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Notes : ContentPage
    {
        public Notes()
        {
            InitializeComponent();
            files.CollectionChanged += Files_CollectionChanged;
            txtDescription.Completed += EditorCompleted;

        }

        public Notes(long id) : this()
        {
            LoadFormData(id);
        }

        #region 全局变量
        ObservableCollection<MediaFile> files = new ObservableCollection<MediaFile>();
        public string Description = "";
        public ArrayList PathList = new ArrayList { };
        #endregion

        /// <summary>
        /// 拍照按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void takePhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            //files.Clear();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = Title,
                Name = "pic.jpg"
            });

            if (file == null)
                return;

            await DisplayAlert("File Location", file.Path, "OK");

            files.Add(file);
        }

        /// <summary>
        /// 选择图片按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void pickPhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            //files.Clear();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("提示", "图片不支持", "OK");
                return;
            }
            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.Full,
                SaveMetaData = true,
                SaveFolder = Title
            });


            if (file == null)
                return;

            files.Add(file);
        }

        /// <summary>
        /// 批量选择图片按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void pickPhotos_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            //files.Clear();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("提示", "图片不支持", "OK");
                return;
            }
            var picked = await CrossMedia.Current.PickPhotosAsync(new PickMediaOptions
            {
                SaveFolder = Title
            });

            if (picked == null)
                return;
            foreach (var file in picked)
                files.Add(file);
        }

        /// <summary>
        /// 图片更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (files.Count == 0)
            {
                ImageList.Children.Clear();
                return;
            }
            if (e.NewItems.Count == 0)
                return;

            var file = e.NewItems[0] as MediaFile;
            var image = new Image { WidthRequest = 300, HeightRequest = 300, Aspect = Aspect.AspectFit };
            image.Source = ImageSource.FromFile(file.Path);
            PathList.Add(file.Path);
            ImageList.Children.Add(image);
        }

        /// <summary>
        /// 加载控件数据
        /// </summary>
        /// <param name="id"></param>
        private async void LoadFormData(long id)
        {
            try
            {
                var dttitle = SqliteHelper.ExecuteQueryRow($"select name from NoteInfo where id = '{id}'", SqliteHelper.DBTable.NoteInfo).Item1;
                var dtdesc = SqliteHelper.ExecuteQueryRow($"select TextDetil from TextInfo where Noteid = '{id}'", SqliteHelper.DBTable.TextInfo).Item1;
                var dtpicpath = SqliteHelper.ExecuteQuery($"select PicPath from PicInfo where Noteid = '{id}' order by id asc", SqliteHelper.DBTable.PicInfo).Item1;
                Title = dttitle["name"].ToString();
                txtDescription.Text = dtdesc["TextDetil"].ToString();
                Description = dtdesc["TextDetil"].ToString();
                foreach (DataRow item in dtpicpath.Rows)
                {
                    var path = item["PicPath"].ToString();
                    var image = new Image { WidthRequest = 300, HeightRequest = 300, Aspect = Aspect.AspectFit };
                    image.Source = ImageSource.FromFile(path);
                    ImageList.Children.Add(image);
                    PathList.Add(path);
                }
                AutomationId = Convert.ToString(id);
            }
            catch (Exception ex)
            {
                await DisplayAlert("提示", ex.ToString(), "OK");
            }
            
        }

        /// <summary>
        /// 清除所有图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Clicked(object sender, EventArgs e)
        {
            ImageList.Children.Clear();
            PathList.Clear();
        }

        /// <summary>
        /// 重命名标题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnReTitle_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync("重命名", "请输入书页名称");
            if (result != "")
            {
                Title = result;
            }
        }

        /// <summary>
        /// 删除最后一张图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnClearLast_Clicked(object sender, EventArgs e)
        {
            if (ImageList.Children.Count - 1 != -1)
            {
                ImageList.Children.RemoveAt(ImageList.Children.Count - 1);
                PathList.RemoveAt(PathList.Count - 1);

            }
            else
            {
                await DisplayAlert("提示", "列表已经被清空了哦", "OK");
            }
        }

        /// <summary>
        /// 描述文本框输入完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EditorCompleted(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDescription.Text)) 
            {
                Description = txtDescription.Text.Trim();
            }
        }

        
    }
}