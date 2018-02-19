using System;
using System.Collections.Generic;
using Foundation;
using NotificationFirebase.DependencyService;
using NotificationFirebase.Model;

using Xamarin.Forms;

using Plugin.FirebasePushNotification;
using Firebase.Auth;
using Plugin.Hud;
using Plugin.Hud.Abstractions;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationFirebase.iOS.FirebaseManagerImplementation))]
namespace NotificationFirebase.iOS
{
    public class FirebaseManagerImplementation : IFirebaseManager
    {
        const string SERVER_KEY = "AAAAAkroygo:APA91bE4amfFJHOYWSdflvwYuOx3GLGdXvfoxNLagSUe2QPfLWWSMlVRommTkrTbeXg9fWBgkgkRb7-BuHFHX0zbdUTb7l6K8sJQf9WeiiMawhlIKry4lBeaUjjGA31C6Nnr66rDjxlm";
        const string URL_NOTIFICATIONS = "https://fcm.googleapis.com/fcm/send";

        public FirebaseManagerImplementation()
        {
        }

        public NotificationFirebase.Model.User GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public List<string> GetDeviceTokenUserListFromDistance(double distance, List<NotificationFirebase.Model.User> users)
        {
            throw new NotImplementedException();
        }

        public List<NotificationFirebase.Model.User> GetUsers()
        {
            throw new NotImplementedException();
        }

        public bool IsUserLoggedIn()
        {
            throw new NotImplementedException();
        }

        public void SendChatMessage(string message)
        {
            throw new NotImplementedException();
        }

        public void SendNotificationToChannelAsync(string message, string channel)
        {
            throw new NotImplementedException();
        }

        public void SendNotificationToDeviceAsync(string message, string tokenDevice)
        {
            throw new NotImplementedException();
        }

        public void SendNotificationToNearestDevices(double maxDistance, double myLatitude, double myLongitude)
        {
            throw new NotImplementedException();
        }

        public void SignInAsync(string email, string password)
        {
            var hud = CrossHud.Current;
            hud.Show("Sign In", -1, MaskType.Clear, true);

            Auth.DefaultInstance.SignIn(email, password, async (user, error) => {
                if (error != null)
                {
                    AuthErrorCode errorCode;
                    if (IntPtr.Size == 8) // 64 bits devices
                        errorCode = (AuthErrorCode)((long)error.Code);
                    else // 32 bits devices
                        errorCode = (AuthErrorCode)((int)error.Code);

                    // Posible error codes that SignIn method with email and password could throw
                    // Visit https://firebase.google.com/docs/auth/ios/errors for more information
                    switch (errorCode)
                    {
                        case AuthErrorCode.OperationNotAllowed:
                        case AuthErrorCode.InvalidEmail:
                        case AuthErrorCode.UserDisabled:
                        case AuthErrorCode.WrongPassword:
                        default:
                            // Print error
                            break;
                    }

                    MessagingCenter.Send("Fail to Sign In", NotificationFirebase.App.MessageSignInError);
                    System.Console.WriteLine("Signn Fail");
                }
                else
                {
                    // Do your magic to handle authentication result
                    if (user != null)
                    {
                        var u = new NotificationFirebase.Model.User
                        {
                            Displayname = user.DisplayName,
                            Email = user.Email,
                            Uid = user.Uid,

                            DeviceToken = CrossFirebasePushNotification.Current.Token,
                            Plataforma = Platform.iOS,
                            Latitud = 0.0f,
                            Longitud = 0.0f
                        };

                        await UpdateUser(u);

                        MessagingCenter.Send(u, NotificationFirebase.App.MessageSignIn);
                        System.Console.WriteLine("SignIn sucess");
                    }
                    else
                    {
                        hud.Dismiss();
                        MessagingCenter.Send("Fail to Sign In", NotificationFirebase.App.MessageSignInError);
                        System.Console.WriteLine("Signn Fail");
                    }
                }

                hud.Dismiss();
                hud.Dispose();
                hud = null;
            });
        }

        public void SignOut()
        {
            var hud = CrossHud.Current;
            hud.Show("Sign In", -1, MaskType.Clear, true);

            NSError error;
            if(!Auth.DefaultInstance.SignOut(out error))
            {
                AuthErrorCode errorCode;
                if (IntPtr.Size == 8) // 64 bits devices
                    errorCode = (AuthErrorCode)((long)error.Code);
                else // 32 bits devices
                    errorCode = (AuthErrorCode)((int)error.Code);

                // Posible error codes that SignOut method with credentials could throw
                // Visit https://firebase.google.com/docs/auth/ios/errors for more information
                switch (errorCode)
                {
                    case AuthErrorCode.KeychainError:
                    default:
                        // Print error
                        break;
                }

                MessagingCenter.Send("Fail to Sign Out", NotificationFirebase.App.MessageSignOutError);
                System.Console.Error.WriteLine();
            }
            else
            {
                MessagingCenter.Send("SignOut", NotificationFirebase.App.MessageSignOut);
            }

            hud.Dismiss();
            hud.Dispose();
            hud = null;
        }

        public void SignUpAsync(string email, string password)
        {
            var hud = CrossHud.Current;
            hud.Show("Sign Up", -1, MaskType.Clear, true);

            Auth.DefaultInstance.CreateUser(email, password, async (user, error) => {
                if (error != null)
                {
                    AuthErrorCode errorCode;
                    if (IntPtr.Size == 8) // 64 bits devices
                        errorCode = (AuthErrorCode)((long)error.Code);
                    else // 32 bits devices
                        errorCode = (AuthErrorCode)((int)error.Code);

                    // Posible error codes that CreateUser method could throw
                    switch (errorCode)
                    {
                        case AuthErrorCode.InvalidEmail:
                        case AuthErrorCode.EmailAlreadyInUse:
                        case AuthErrorCode.OperationNotAllowed:
                        case AuthErrorCode.WeakPassword:
                        default:
                            // Print error
                            break;
                    }
                    MessagingCenter.Send("", NotificationFirebase.App.MessageSignInError);
                    System.Console.Error.WriteLine("");
                }
                else
                {
                    // Do your magic to handle authentication result
                    if (user != null)
                    {
                        var u = new NotificationFirebase.Model.User
                        {
                            Displayname = user.DisplayName,
                            Email = user.Email,
                            Uid = user.Uid,

                            DeviceToken = CrossFirebasePushNotification.Current.Token,
                            Plataforma = Platform.iOS,
                            Latitud = 0.0f,
                            Longitud = 0.0f
                        };

                        MessagingCenter.Send(u, NotificationFirebase.App.MessageSignUp);
                        await UpdateUser(u);

                        System.Console.WriteLine("SignUp sucess");
                    }
                    else
                    {
                        MessagingCenter.Send("Fail to Sign Up", NotificationFirebase.App.MessageSignUpError);
                        System.Console.WriteLine("SignUp Fail");
                    }
                }

                hud.Dismiss();
                hud.Dispose();
                hud = null;
            });
        }

        public void UpdateUserDeviceToken()
        {
            throw new NotImplementedException();
        }

        async System.Threading.Tasks.Task UpdateUser(NotificationFirebase.Model.User user)
        {
            try
            {
                /*var mDatabase = FirebaseDatabase.Instance.Reference;
                var _ref = mDatabase.Child(UserTable.TableName).Child(user.Uid);

                await _ref.Child(UserTable.Plataforma).SetValueAsync(new Java.Lang.String(user.Plataforma));
                await _ref.Child(UserTable.DeviceToken).SetValueAsync(new Java.Lang.String(user.DeviceToken));
                await _ref.Child(UserTable.Latitud).SetValueAsync(new Java.Lang.Double(user.Latitud));
                await _ref.Child(UserTable.Longitud).SetValueAsync(new Java.Lang.Double(user.Longitud));
                */
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

        }
    }
}
