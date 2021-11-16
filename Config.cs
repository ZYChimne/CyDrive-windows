using System;
using System.Collections.Generic;
using System.Text;

namespace CyDrive
{
    public static class Config
    {
        public static readonly bool DEBUG = true;
        public static readonly string SERVER_ADDRESS = "123.57.39.79:6454";
        public static readonly string DEFAULT_EMAIL = "test@cydrive.io";
        public static readonly string DEFAULT_PASSWORD = "hello_world";
        public static CyDriveClient client = new CyDriveClient(SERVER_ADDRESS);
        public static string getFileTypeIcon(string suffix)
        {
            suffix=suffix.ToLower();
            switch (suffix)
            {
                case "jpg":return "image";
                case "jpeg":return "image";
                case "png":return "image";
                case "mp4":return "video";
                case "mp3":return "music";
                case "xls":return "excel";
                case "xlsx":return "excel";
                case "doc":return "word";
                case "docx":return "word";
                case "ppt":return "powerpoint";
                case "pptx":return "powerpoint";
                case "zip":return "zip";
                case "txt":return "doc";
                case "pdf":return "pdf";
                default:return "unknown";
            }
        }
    }
}
