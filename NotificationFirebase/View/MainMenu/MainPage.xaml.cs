using System;
using System.Collections.Generic;
using NotificationFirebase;
using NotificationFirebase.DependencyService;

using Xamarin.Forms;

namespace NotificationFirebase.View
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
            masterPage.ListView.ItemSelected += OnItemSelected;

            MessagingCenter.Subscribe<string>(this,App.MessageSignOut, SignOut);
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e != null && e.SelectedItem is MasterPageItem item)
            {
                if (item.ID == 0)
                {
                    Xamarin.Forms.DependencyService.Get<IFirebaseManager>().SignOut();
                }
                else
                {
                    Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                    masterPage.ListView.SelectedItem = null;
                    IsPresented = false;
                }
            }
        }

        void SignOut(string message)
        {
            try
            {
                App.IsUserLoggedIn = false;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (Navigation.NavigationStack.Count == 0)
                    {
                        await Navigation.PushModalAsync(new LoginPage(), false);
                    }
                    else
                    {
                        await Navigation.PopModalAsync(false);
                    }



                    if(Device.OS == TargetPlatform.Android)
                    {
                        //Application.Current.MainPage = new LoginPage();
                    }
                });
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }
    }
}
