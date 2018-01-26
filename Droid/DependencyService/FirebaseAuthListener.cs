using System;

using Firebase;
using Firebase.Auth;
using Xamarin.Forms;

namespace NotificationFirebase.Droid
{
    public class FirebaseAuthListener : Java.Lang.Object, FirebaseAuth.IAuthStateListener
    {
        public FirebaseAuthListener()
        {
        }

        public void OnAuthStateChanged(FirebaseAuth auth)
        {
            if(auth!= null)
            {
                FirebaseUser user = auth.CurrentUser;
                if (user != null)
                {
                    // an user is already signin
                    if(App.IsUserLoggedIn)
                    {
                        App.IsUserLoggedIn = true;
                        //MessagingCenter.Send(App.user, NotificationFirebase.App.MessageSignIn);
                    }
                    System.Console.WriteLine("onAuthStateChanged:signed_in:" + user.Uid);
                }
                else
                {
                    if(App.IsUserLoggedIn)
                    {
                        App.IsUserLoggedIn = false;
                        //MessagingCenter.Send("SignOut", NotificationFirebase.App.MessageSignOut);
                    }
                }
            }
            else
            {
                App.IsUserLoggedIn = false;
                System.Console.WriteLine("onAuthStateChanged:signed_out");
                //MessagingCenter.Send("SignOut", NotificationFirebase.App.MessageSignOut);
            }
        }
    }
}
