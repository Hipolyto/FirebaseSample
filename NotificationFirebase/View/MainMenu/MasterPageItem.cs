using System;

namespace NotificationFirebase.View
{
    public class MasterPageItem
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string IconSource { get; set; }

        public Type TargetType { get; set; }
    }
}
