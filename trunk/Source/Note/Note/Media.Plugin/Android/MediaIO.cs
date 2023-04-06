using System.IO;


namespace Plugin.Media
{
    public static class MediaIO
    {
        static string dbUrl = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "DB");
        public static void CreateNewDBFile()
        {

            using (var mediaStorageDir = new Java.IO.File(dbUrl))
            {
                mediaStorageDir.Mkdirs();


                // Ensure this media doesn't show up in gallery apps
                using (var nomedia = new Java.IO.File(mediaStorageDir, "Note.db"))
                    nomedia.CreateNewFile();

            }
        }

    }
}
