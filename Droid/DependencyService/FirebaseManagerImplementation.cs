using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using Android.Content;
using Android.Gms.Tasks;

using Xamarin.Forms;


using Plugin.CurrentActivity;
using Plugin.FirebasePushNotification;
using Firebase;
using Firebase.Auth;
using Firebase.Database;


using NotificationFirebase;
using NotificationFirebase.Model;
using NotificationFirebase.DependencyService;
using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationFirebase.Droid.FirebaseManagerImplementation))]
namespace NotificationFirebase.Droid
{
    public class FirebaseManagerImplementation : IFirebaseManager
    {
        public FirebaseManagerImplementation()
        {
        }
        const string SERVER_KEY = "AAAAAkroygo:APA91bE4amfFJHOYWSdflvwYuOx3GLGdXvfoxNLagSUe2QPfLWWSMlVRommTkrTbeXg9fWBgkgkRb7-BuHFHX0zbdUTb7l6K8sJQf9WeiiMawhlIKry4lBeaUjjGA31C6Nnr66rDjxlm";
        const string URL = "https://fcm.googleapis.com/fcm/send";

        public async void SendNotificationToDeviceAsync(string message, string tokenDevice)
        {
            var jData = new JObject
            {
                { "body", message }
            };
            var jGcmData = new JObject
            {
                { "to", CrossFirebasePushNotification.Current.Token }, // general
                { "notification", jData }
            };

            var url = new Uri(URL);
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + SERVER_KEY);
                    //System.Threading.Tasks.Task.WaitAll(

                    var response = await client.PostAsync(
                            url,
                            new StringContent(jGcmData.ToString(), Encoding.Default, "application/json"));
                        
                    //.ContinueWith(HandleAction));

                    if(response != null)
                    {
                        if(response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Message sent: check client device notification tray");
                        }
                        else
                        {
                            Console.WriteLine("Message sent response: StatusCode = " + response.StatusCode.ToString() + " message = " + response.ToString());
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine("Error sending message, return task=null");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to send GCM message");
                Console.Error.WriteLine(ex.StackTrace);
            }
        }
        public async void SendNotificationToChannelAsync(string message, string channel)
        {
            var jData = new JObject
            {
                { "message", message }
            };
            var jGcmData = new JObject
            {
                { "to", "/topics/"+channel },
                { "data", jData }
            };

            var url = new Uri(URL);
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + SERVER_KEY);
                    //System.Threading.Tasks.Task.WaitAll(
                    var response = await client.PostAsync(
                            url,
                        new StringContent(jGcmData.ToString(), Encoding.Default, "application/json"));
                    //    .ContinueWith(HandleAction));

                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Message sent: check client device notification tray");
                        }
                        else
                        {
                            Console.WriteLine("Message sent response: StatusCode = " + response.StatusCode.ToString() + " message = " + response.ToString());
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine("Error sending message, return task=null");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to send GCM message");
                Console.Error.WriteLine(ex.StackTrace);
            }
        }

        /*void HandleAction(Task<HttpResponseMessage> response)
        {
            if(response != null)
            {
                HttpResponseMessage res = response.Result;
                if(res.IsSuccessStatusCode)
                {
                    Console.WriteLine("Message sent: check client device notification tray");
                }
                else
                {
                    Console.WriteLine("Message sent response: StatusCode = " + res.StatusCode.ToString() + " message = " + res.ToString());
                }
            }
            else
            {
                Console.Error.WriteLine("Error sending message, return task=null");
            }
        }*/

        public async void SignInAsync(string email, string password)
        {
            try
            {
                FirebaseAuth mAuth = FirebaseAuth.Instance;
                var res = await mAuth.SignInWithEmailAndPasswordAsync(email, password);
                if (res != null && res.User != null)
                {
                    var user = new User
                    {
                        Displayname = res.User.DisplayName,
                        Email = res.User.Email,
                        Uid = res.User.Uid,

                        DeviceToken = CrossFirebasePushNotification.Current.Token,
                        Plataforma = Platform.Android,
                        Latitud = 0.0f,
                        Longitud = 0.0f
                    };

                    MessagingCenter.Send(user, NotificationFirebase.App.MessageSignIn);

                    await UpdateUser(user);
                }
                else
                {
                    MessagingCenter.Send("Fail to Sign In", NotificationFirebase.App.MessageSignInError);
                }
            }
            catch(Exception ex)
            {
                MessagingCenter.Send(ex.Message, NotificationFirebase.App.MessageSignInError);
                System.Console.Error.WriteLine(ex.StackTrace);
            }
        }
        public async void SignUpAsync(string email, string password)
        {
            try
            {
                FirebaseAuth mAuth = FirebaseAuth.Instance;
                var res = await mAuth.CreateUserWithEmailAndPasswordAsync(email, password);
                if (res != null && res.User != null)
                {
                    var user = new User
                    {
                        Displayname = res.User.DisplayName,
                        Email = res.User.Email,
                        Uid = res.User.Uid,

                        DeviceToken = CrossFirebasePushNotification.Current.Token,
                        Plataforma = Platform.Android,
                        Latitud = 0.0f,
                        Longitud = 0.0f
                    };
                    MessagingCenter.Send(user, NotificationFirebase.App.MessageSignUp);

                    await UpdateUser(user);
                }
                else
                {
                    MessagingCenter.Send("Fail to Sign Up", NotificationFirebase.App.MessageSignUpError);
                }
            }
            catch(Exception ex)
            {
                MessagingCenter.Send(ex.Message, NotificationFirebase.App.MessageSignUpError);
                System.Console.Error.WriteLine(ex.StackTrace);
            }
        }
        public void SignOut()
        {
            try
            {
                FirebaseAuth mAuth = FirebaseAuth.Instance;
                mAuth.SignOut();
                MessagingCenter.Send("SignOut", NotificationFirebase.App.MessageSignOut);
            }
            catch(Exception ex)
            {
                MessagingCenter.Send("Fail to Sign Out", NotificationFirebase.App.MessageSignOutError);
                System.Console.Error.WriteLine(ex.StackTrace);
            }
        }

        public async void UpdateUserDeviceToken(string userid, string deviceToken)
        {
            var mDatabase = FirebaseDatabase.Instance.Reference;
            await mDatabase.Child("users").Child(userid).Child("DeviceToken").SetValueAsync(new Java.Lang.String(deviceToken));
            await mDatabase.Child("users").Child(userid).Child("Plataforma").SetValueAsync(new Java.Lang.String(Platform.Android));
        }

        async System.Threading.Tasks.Task UpdateUser(User user)
        {
            var mDatabase = FirebaseDatabase.Instance.Reference;
            //var u = ConvertCsharpUserToJavaUser(user);
            await mDatabase.Child("users").Child(user.Uid).Child("Plataforma").SetValueAsync(new Java.Lang.String(user.Plataforma));
            await mDatabase.Child("users").Child(user.Uid).Child("DeviceToken").SetValueAsync(new Java.Lang.String(user.DeviceToken));
            await mDatabase.Child("users").Child(user.Uid).Child("Latitud").SetValueAsync(new Java.Lang.Double(user.Latitud));
            await mDatabase.Child("users").Child(user.Uid).Child("Longitud").SetValueAsync(new Java.Lang.Double(user.Longitud));
        }

        NotificationFirebase.Droid.Model.User ConvertCsharpUserToJavaUser(NotificationFirebase.Model.User userCS)
        {
            NotificationFirebase.Droid.Model.User userJava = new NotificationFirebase.Droid.Model.User
            {
                Displayname = new Java.Lang.String(userCS.Displayname),
                Email = new Java.Lang.String(userCS.Email),
                Uid = new Java.Lang.String(userCS.Uid),

                Plataforma = new Java.Lang.String(userCS.Plataforma),
                DeviceToken = new Java.Lang.String(userCS.DeviceToken),
                Latitud = new Java.Lang.Double(userCS.Latitud),
                Longitud = new Java.Lang.Double(userCS.Longitud)
            };
            return userJava;
        }


        public List<User> GetUsers()
        {
            List<User> users = null;
            var mDatabase = FirebaseDatabase.Instance.Reference;
            var query = mDatabase.Child("users").OrderByKey().AddChildEventListener(new ChildEventListener());

            if(query != null)
            {
                users = new List<User>();
            }
            return users;
        }

        public List<string> GetDeviceTokenUserListFromDistance(double distanceKm, List<User> users)
        {
            List<string> tokens = null;
            double myLatitud = 0.0f;
            double myLongitud = 0.0f;

            if(users != null && users.Count > 0)
            {
                foreach(User u in users)
                {
                    if(GetDistanceInKm(myLatitud, myLongitud, u.Latitud, u.Longitud) <= distanceKm)
                    {
                        if(tokens == null)
                        {
                            tokens = new List<string>();
                        }
                        else
                        {
                            tokens.Add(u.DeviceToken);
                        }
                    }
                }
            }
            return tokens;
        }

        private double GetDistanceInKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6378.0f; // Km
            double dlat = EnRadianes(lat2 - lat1);
            double dlon = EnRadianes(lon2 - lon1);
            double a = Math.Pow(Math.Sin(dlat / 2.0f), 2.0f) + Math.Cos(EnRadianes(lat1)) + Math.Cos(EnRadianes(lat2)) + Math.Pow(Math.Sin(dlon / 2.0f), 2.0f);
            double c = 2.0f * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0f - a));
            return R - c;
        }

        private double EnRadianes(double valor)
        {
            return ((Math.PI / 180.0f) * valor);
        }
        public class ChildEventListener : Java.Lang.Object, IChildEventListener
        {
            public void OnCancelled(DatabaseError error)
            {
                
            }

            public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
            {
                
            }

            public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
            {
                NotificationFirebase.Droid.Model.User user = (NotificationFirebase.Droid.Model.User)snapshot.Children.Iterator().Next();
                System.Console.WriteLine();                                                     
            }

            public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
            {
                
            }

            public void OnChildRemoved(DataSnapshot snapshot)
            {
                
            }
        }
    }
}
