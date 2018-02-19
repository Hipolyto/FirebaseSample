using System;
using Java.Util;
using NotificationFirebase.Model;

namespace NotificationFirebase.Droid.Model
{
    public static class HelperMap
    {
        public static HashMap UserModelToMap(User user)
        {
            HashMap map = new HashMap();
            map.Put(UserTable.Uid, user.Uid);
            map.Put(UserTable.Email, user.Email);
            map.Put(UserTable.DisplayName, user.Displayname);
            map.Put(UserTable.Plataforma, user.Plataforma);
            map.Put(UserTable.DeviceToken, user.DeviceToken);
            map.Put(UserTable.Latitud, user.Latitud);
            map.Put(UserTable.Longitud, user.Longitud);

            return map;
        }

        public static NotificationFirebase.Model.User MapToUserModel(Firebase.Database.DataSnapshot dataSnapshot)
        {
            if(dataSnapshot != null)
            {
                if (dataSnapshot.GetValue(true) == null) return null;

                var user = new NotificationFirebase.Model.User
                {
                    Uid = dataSnapshot.Key,
                    Displayname = dataSnapshot.Child(UserTable.DisplayName)?.GetValue(true)?.ToString(),
                    Email = dataSnapshot.Child(UserTable.Email)?.GetValue(true)?.ToString(),
                    Plataforma = dataSnapshot.Child(UserTable.Plataforma)?.GetValue(true)?.ToString(),
                    DeviceToken = dataSnapshot.Child(UserTable.DeviceToken)?.GetValue(true)?.ToString()
                };
                double.TryParse(dataSnapshot.Child(UserTable.Latitud)?.GetValue(true)?.ToString(), out double lat);
                user.Latitud = lat;
                double.TryParse(dataSnapshot.Child(UserTable.Longitud)?.GetValue(true)?.ToString(), out double lon);
                user.Longitud = lon;

                return user;
            }
            return null;
        }
    

        public static HashMap ChatMessageModelToMap(NotificationFirebase.Model.ChatMessage m)
        {
            if (m != null)
            {
                HashMap map = new HashMap();

                if(!string.IsNullOrWhiteSpace(m.Id))
                    map.Put(new Java.Lang.String(ChatMessageTable.Id), new Java.Lang.String(m.Id));
                if (!string.IsNullOrWhiteSpace(m.Name))
                    map.Put(new Java.Lang.String(ChatMessageTable.Name), new Java.Lang.String(m.Name));
                if (!string.IsNullOrWhiteSpace(m.Text))
                    map.Put(new Java.Lang.String(ChatMessageTable.Text), new Java.Lang.String(m.Text));
                if (!string.IsNullOrWhiteSpace(m.PhotoUrl))
                    map.Put(new Java.Lang.String(ChatMessageTable.PhotoUrl), new Java.Lang.String(m.PhotoUrl));
                if (!string.IsNullOrWhiteSpace(m.ImageUrl))
                    map.Put(new Java.Lang.String(ChatMessageTable.ImageUrl), new Java.Lang.String(m.ImageUrl));

                return map;
            }
            return null;
        }

        public static ChatMessage MapToChatMessageModel(Firebase.Database.DataSnapshot dataSnapshot)
        {
            if(dataSnapshot != null)
            {
                if (dataSnapshot.GetValue(true) == null) return null;

                var m = new ChatMessage
                {
                    Id = dataSnapshot.Key,
                    Name = dataSnapshot.Child(ChatMessageTable.Name)?.GetValue(true)?.ToString(),
                    Text = dataSnapshot.Child(ChatMessageTable.Text)?.GetValue(true)?.ToString(),
                    PhotoUrl = dataSnapshot.Child(ChatMessageTable.PhotoUrl)?.GetValue(true)?.ToString(),
                    ImageUrl = dataSnapshot.Child(ChatMessageTable.ImageUrl)?.GetValue(true)?.ToString()
                };

                return m;
            }
            return null;
        }
    
    }
}
