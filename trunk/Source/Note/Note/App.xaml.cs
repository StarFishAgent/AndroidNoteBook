﻿using System;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Note
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            SqliteHelper.CreateDb();
            MainPage = new NotesPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
