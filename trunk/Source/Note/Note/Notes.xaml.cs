using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FFImageLoading.Forms;

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
        }
        ObservableCollection<MediaFile> files = new ObservableCollection<MediaFile>();
        /// <summary>
        /// 拍照按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void takePhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            files.Clear();
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
            files.Clear();
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
            files.Clear();
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
            ImageList.Children.Add(image);
        }
        /// <summary>
        /// 清除所有图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Clicked(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 重命名标题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReTitle_Clicked(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 删除最后一张图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearLast_Clicked(object sender, EventArgs e)
        {

        }
    }
}