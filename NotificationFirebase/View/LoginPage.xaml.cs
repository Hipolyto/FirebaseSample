using System;
using System.Collections.Generic;

using Xamarin.Forms;
using NotificationFirebase.Model;
using NotificationFirebase.DependencyService;
using System.Threading.Tasks;

namespace NotificationFirebase.View
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, true);

            MessagingCenter.Subscribe<User>(this, App.MessageSignIn, SignIn);
            MessagingCenter.Subscribe<string>(this, App.MessageSignInError, SignInError);
        }

        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SigUpPage());
        }

        void OnLoginButtonClicked(object sender, System.EventArgs e)
        {
            Xamarin.Forms.DependencyService.Get<IFirebaseManager>().SignInAsync(emailEntry.Text, passwordEntry.Text);
        }

        void SignIn(User user)
        {
            try
            {
                App.IsUserLoggedIn = true;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if(Navigation.NavigationStack.Count == 0)
                    {
                        await Navigation.PushModalAsync(new MainPage(), false);
                    }
                    else
                    {
                        await Navigation.PopModalAsync(false);
                    }

                    if(Device.OS == TargetPlatform.Android)
                    {
                        //Application.Current.MainPage = new MainPage();
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        void SignInError(string erorMessage)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                messageLabel.Text = erorMessage;
            });
        }
    }
}
