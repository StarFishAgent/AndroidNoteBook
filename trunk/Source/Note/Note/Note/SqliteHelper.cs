using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SQLite;
using Xamarin.Essentials;
using System.IO;
using static Android.Provider.Telephony.Mms;

namespace Note
{
    public static class SqliteHelper
    {
        static string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mydb.db3");

        static string dbFolder = FileSystem.AppDataDirectory + "/DB";
        static string dbUrl = System.IO.Path.Combine(dbFolder, "Note.db");
        static string StrConn = "Data Source="+ dbUrl;
        public static DataTable ExecuteQuery(this string StrSql) {
            var dt = new DataTable();
            try {
                new SQLiteDataAdapter(StrSql, StrConn).Fill(dt);

            }
            catch {
                dt = new DataTable();
            }
            return dt;
        }
        public static void ExecuteNonQuery(this string StrSql) {
            try {
                var conn = new SQLiteConnection(StrConn);
                var cmd = new SQLiteCommand(StrSql, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch {
            }
        }
        public static async void CreateSqliteDatabase() {
            if (File.Exists(dbUrl))//如果文件存在就返回True，则返回False
            {//存在
                return;
            }
            else {//不存在
                var conn = new SQLiteConnection(path);
                //var conn = new SQLite.SQLiteConnection(dbUrl);

            }

        }
    }
}
