using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

using NotificationFirebase;
using NotificationFirebase.Model;
using NotificationFirebase.DependencyService;

namespace NotificationFirebase.View
{
    public partial class SigUpPage : ContentPage
    {
        public SigUpPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<User>(this, App.MessageSignUp, SignUp);
            MessagingCenter.Subscribe<string>(this, App.MessageSignUpError, SignUpError);
        }

        void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            Xamarin.Forms.DependencyService.Get<IFirebaseManager>().SignUpAsync(emailEntry.Text, passwordEntry.Text);
        }

        void SignUp(User user)
        {
            try
            {
                App.IsUserLoggedIn = true;
                Device.BeginInvokeOnMainThread(() =>
                {
                    //await Navigation.PushModalAsync(new MainPage());
                    Application.Current.MainPage = new MainPage();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        void SignUpError(string erorMessage)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                messageLabel.Text = erorMessage;
            });
        }
    }
}
