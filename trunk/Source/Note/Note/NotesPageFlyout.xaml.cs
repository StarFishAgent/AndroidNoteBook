using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Note
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotesPageFlyout : ContentPage
    {
        public ListView ListViewPage;
        public Button button;
        private ToolbarItem _saveAddToolBarItem;

        public NotesPageFlyout()
        {
            InitializeComponent();

            BindingContext = new NotesPageFlyoutViewModel();
            ListViewPage = MenuItemsListView;
        }

        class NotesPageFlyoutViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<NotesPageFlyoutMenuItem> MenuItems { get; set; }

            public NotesPageFlyoutViewModel()
            {
                MenuItems = new ObservableCollection<NotesPageFlyoutMenuItem>();
                var dt = SqliteHelper.ExecuteQuery("select id,name from NoteInfo where IsShow='1' order by id asc", SqliteHelper.DBTable.NoteInfo).Item1;
                foreach (DataRow item in dt.Rows)
                {
                    MenuItems.Add(new NotesPageFlyoutMenuItem { Id = Convert.ToInt64(item["id"]), Title = Convert.ToString(item["name"]) });
                }
                MenuItems.Add(new NotesPageFlyoutMenuItem { Title = "添加空白书页" });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}