using System;


namespace NotificationFirebase.Model
{
    public class User
    {
        public string Displayname { get; set; }
        public string Email { get; set; }
        public string Uid { get; set; }

        public string Plataforma { get; set; }
        public string DeviceToken { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }

    }

    public class Platform
    {
        public const string Android = "Android";
        public const string iOS = "iOS";
        public const string Windows = "Windows";
    }
}
