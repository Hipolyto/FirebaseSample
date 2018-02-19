using System;
namespace NotificationFirebase.Model
{
    public class ChatMessage
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string ImageUrl { get; set; }

        public ChatMessage()
        {
            
        }

        public ChatMessage(string text, string name, string photourl, string imageurl)
        {
            this.Text = text;
            this.Name = name;
            this.PhotoUrl = photourl;
            this.ImageUrl = imageurl;
        }
    }

    public static class ChatMessageTable
    {
        public const string TableName = "message";

        public const string Id = "id";
        public const string Text = "text";
        public const string Name = "name";
        public const string PhotoUrl = "photoUrl";
        public const string ImageUrl = "imageUrl";
    }
}
