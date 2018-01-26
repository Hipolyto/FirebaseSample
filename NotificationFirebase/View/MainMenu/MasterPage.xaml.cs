using System;
using System.Collections.Generic;

using Xamarin.Forms;
using NotificationFirebase;

namespace NotificationFirebase.View
{
    public partial class MasterPage : ContentPage
    {
        public ListView ListView { get { return listView; }}

        public MasterPage()
        {
            InitializeComponent();
        }
    }
}
