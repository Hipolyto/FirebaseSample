using Java;
using Java.Lang;

namespace NotificationFirebase.Droid.Model
{
    public class User : Java.Lang.Object
    {
        public String Displayname;
        public String Email;
        public String Uid;
        public String DeviceToken;
        public Double Latitud;
        public Double Longitud;
        public String Plataforma;

        public User()
        {
        }

        public User(String username, String email)
        {
            this.Displayname = username;
            this.Email = email;
        }
    }
}
