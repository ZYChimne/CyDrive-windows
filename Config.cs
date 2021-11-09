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
        public static readonly int DEVICE_ID = 123456;
        public static readonly CyDriveClient client = new CyDriveClient(SERVER_ADDRESS, DEVICE_ID);
    }
}
