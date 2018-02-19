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
using Plugin.Hud;
using Firebase;
using Firebase.Auth;
using Firebase.Database;


using NotificationFirebase;
using NotificationFirebase.Model;
using NotificationFirebase.DependencyService;
using System.Collections.Generic;
using Plugin.Hud.Abstractions;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationFirebase.Droid.FirebaseManagerImplementation))]
namespace NotificationFirebase.Droid
{
    public class FirebaseManagerImplementation : IFirebaseManager
    {
        const string SERVER_KEY = "AAAAAkroygo:APA91bE4amfFJHOYWSdflvwYuOx3GLGdXvfoxNLagSUe2QPfLWWSMlVRommTkrTbeXg9fWBgkgkRb7-BuHFHX0zbdUTb7l6K8sJQf9WeiiMawhlIKry4lBeaUjjGA31C6Nnr66rDjxlm";
        const string URL_NOTIFICATIONS = "https://fcm.googleapis.com/fcm/send";

        public FirebaseManagerImplementation()
        {
        }

        public async void SendNotificationToDeviceAsync(string message, string tokenDevice)
        {
            CrossHud.Current.Show("Sending Message", -1, MaskType.Clear, true);

            try
            {
                var jGcmData = new JObject
                {
                    { "to", tokenDevice },
                    { "notification", new JObject{ { "body", message }} }
                };
                string json = jGcmData.ToString();

                using (var handler = new HttpClientHandler())
                {
                    using (var client = new HttpClient(handler, true))
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Post, URL_NOTIFICATIONS))
                        {
                            request.Headers.Add("Accept", "application/json"); //Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.TryAddWithoutValidation("Authorization", "key=" + SERVER_KEY);

                            request.Content = new StringContent(jGcmData.ToString(), Encoding.UTF8, "application/json");

                            using (var response = await client.SendAsync(request))
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to send GCM message");
                Console.Error.WriteLine(ex.StackTrace);
            }

            CrossHud.Current.Dismiss();
        }
        public async void SendNotificationToChannelAsync(string message, string channel)
        {
            CrossHud.Current.Show("Sending Message", -1, MaskType.Clear, true);

            try
            {
                var jGcmData = new JObject
                {
                    { "to", "/topics/"+channel },
                    { "data", new JObject { { "message", message } } }
                };

                using (var handler = new HttpClientHandler())
                {
                    using (var client = new HttpClient(handler, true))
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Post, URL_NOTIFICATIONS))
                        {
                            request.Headers.Add("Accept", "application/json"); //Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.TryAddWithoutValidation("Authorization", "key=" + SERVER_KEY);

                            request.Content = new StringContent(jGcmData.ToString(), Encoding.UTF8, "application/json");

                            using (var response = await client.SendAsync(request))
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to send GCM message");
                Console.Error.WriteLine(ex.StackTrace);
            }

            CrossHud.Current.Dismiss();
        }

        public async void SignInAsync(string email, string password)
        {
            var hud = CrossHud.Current;
            hud.Show("Sign In", -1, MaskType.Clear, true);

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
                    await UpdateUser(user);

                    hud.Dismiss();

                    MessagingCenter.Send(user, NotificationFirebase.App.MessageSignIn);
                    System.Console.WriteLine("SignIn sucess");
                }
                else
                {
                    hud.Dismiss();
                    MessagingCenter.Send("Fail to Sign In", NotificationFirebase.App.MessageSignInError);
                    System.Console.WriteLine("Signn Fail");
                }
            }
            catch(Exception ex)
            {
                hud.Dismiss();
                MessagingCenter.Send(ex.Message, NotificationFirebase.App.MessageSignInError);
                System.Console.Error.WriteLine(ex.StackTrace);
            }
            hud.Dismiss();
            hud.Dispose();
            hud = null;
        }

        public async void SignUpAsync(string email, string password)
        {
            var hud = CrossHud.Current;
            hud.Show("Sign Up", -1, MaskType.Clear, true);

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
                    System.Console.WriteLine("SignUp sucess");
                }
                else
                {
                    MessagingCenter.Send("Fail to Sign Up", NotificationFirebase.App.MessageSignUpError);
                    System.Console.WriteLine("SignUp Fail");
                }
            }
            catch(Exception ex)
            {
                MessagingCenter.Send(ex.Message, NotificationFirebase.App.MessageSignUpError);
                System.Console.Error.WriteLine(ex.StackTrace);
            }
            hud.Dismiss();
            hud.Dispose();
            hud = null;
        }
        public void SignOut()
        {
            var hud = CrossHud.Current;
            hud.Show("Sign Up", -1, MaskType.Gradient, true);
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
            hud.Dismiss();
            hud.Dispose();
            hud = null;
        }

        public User GetCurrentUser()
        {
            FirebaseAuth mAuth = FirebaseAuth.Instance;
            if(mAuth.CurrentUser != null)
            {
                User user = new User
                {
                    Email = mAuth.CurrentUser.Email,
                    Displayname = mAuth.CurrentUser.DisplayName,
                    Uid = mAuth.CurrentUser.Uid,
                    Plataforma = NotificationFirebase.Model.Platform.Android
                };
                return user;
            }
            return null;

        }
        /*bool IsUserLoggedIn()
        {
            return GetCurrentUser() != null;
        }*/

        bool IFirebaseManager.IsUserLoggedIn()
        {
            return GetCurrentUser() != null;
        }

        public async void UpdateUserDeviceToken()
        {
            try
            {
                var userid = FirebaseAuth.Instance.CurrentUser.Uid;
                var token = CrossFirebasePushNotification.Current.Token;

                var mDatabase = FirebaseDatabase.Instance.Reference;
                var _ref = mDatabase.Child(UserTable.TableName).Child(userid);

                await _ref.Child(UserTable.DeviceToken).SetValueAsync(new Java.Lang.String(token));
                await _ref.Child(UserTable.Plataforma).SetValueAsync(new Java.Lang.String(Platform.Android));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        async System.Threading.Tasks.Task UpdateUser(User user)
        {
            try
            {
                var mDatabase = FirebaseDatabase.Instance.Reference;
                var _ref = mDatabase.Child(UserTable.TableName).Child(user.Uid);

                await _ref.Child(UserTable.Plataforma).SetValueAsync(new Java.Lang.String(user.Plataforma));
                await _ref.Child(UserTable.DeviceToken).SetValueAsync(new Java.Lang.String(user.DeviceToken));
                await _ref.Child(UserTable.Latitud).SetValueAsync(new Java.Lang.Double(user.Latitud));
                await _ref.Child(UserTable.Longitud).SetValueAsync(new Java.Lang.Double(user.Longitud));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

        }

        public async void SendChatMessage(string message)
        {
            try
            {
                var m = new ChatMessage(message, FirebaseAuth.Instance.CurrentUser.Email, "no photo", null);

                using (var mFirebaseDatabse = FirebaseDatabase.Instance.Reference)
                {
                    await mFirebaseDatabse.Child(ChatMessageTable.TableName).Push().SetValueAsync(Model.HelperMap.ChatMessageModelToMap(m));
                    /*
                    using (var n = mFirebaseDatabse.Child(ChatMessageTable.TableName).Push())
                    {
                        System.Diagnostics.Debug.WriteLine(n.Key);

                        await n.Child(ChatMessageTable.Text).SetValueAsync(new Java.Lang.String(m.Text));
                        await n.Child(ChatMessageTable.Name).SetValueAsync(new Java.Lang.String(m.Name));
                        await n.Child(ChatMessageTable.PhotoUrl).SetValueAsync(new Java.Lang.String(m.PhotoUrl));
                    }
                    */
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        ValueEventListener valueEventListener = null;
        public List<User> GetUsers()
        {
            List<User> users = null;
            var mDatabase = FirebaseDatabase.Instance.Reference;
            valueEventListener = mDatabase.Child("users").OrderByKey().AddValueEventListener(new ValueEventListener()) as ValueEventListener;

            /*if(valueEventListener != null)
            {
                if(valueEventListener.UsersGet)
                {
                    users = valueEventListener.Users;
                }
            }*/
            return users;
        }

        void GetAllUsers(double distance, double myLatitude, double myLongitude)
        {
            var mDatabase = FirebaseDatabase.Instance.Reference;
            valueEventListener = mDatabase
                .Child("users").OrderByKey()
                .AddValueEventListener(
                    new ValueEventListener(distance, myLatitude, myLongitude)) as ValueEventListener;

            //var _ref = mDatabase.Child("messages").OrderByKey();
            //_ref.AddValueEventListener(()=>{});
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

        private static double GetDistanceInKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6378.0f; // Km
            double dlat = EnRadianes(lat2 - lat1);
            double dlon = EnRadianes(lon2 - lon1);
            double a = Math.Pow(Math.Sin(dlat / 2.0f), 2.0f) + Math.Cos(EnRadianes(lat1)) + Math.Cos(EnRadianes(lat2)) + Math.Pow(Math.Sin(dlon / 2.0f), 2.0f);
            double c = 2.0f * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0f - a));
            return R - c;
        }

        private static  double EnRadianes(double valor)
        {
            return ((Math.PI / 180.0f) * valor);
        }

        public void SendNotificationToNearestDevices(double maxDistance, double myLatitude, double myLongitude)
        {
            GetAllUsers(maxDistance, myLatitude, myLongitude);
        }

        public class ValueEventListener : Java.Lang.Object, IValueEventListener
        {
            List<User> _users = null;
            //public List<User> Users { get { return _users; } }
            bool _usersgets = false;
            //public bool UsersGet { get { return _usersgets; }}
            double _distance;
            List<string> _tokens = null;
            double _myLatitud = 0.0f;
            double _myLongitud = 0.0f;

            public ValueEventListener()
            {
            }

            public ValueEventListener(double distance, double myLatitude, double myLongitude)
            {
                this._distance = distance;
                this._myLatitud = myLatitude;
                this._myLongitud = myLongitude;
            }

            public void OnCancelled(DatabaseError error)
            {

            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                GetAllUsers(snapshot);
                GetAllNearestTokens();
                SendNotifications();
            }

            void GetAllUsers(DataSnapshot snapshot)
            {
                if (snapshot != null && snapshot.Exists())
                {
                    var obj = snapshot.Children;

                    if (snapshot != null && snapshot.ChildrenCount > 0)
                    {
                        _users = new List<User>();
                    }

                    while (obj.Iterator().HasNext)
                    {
                        DataSnapshot snapshotChild = obj.Iterator().Next() as DataSnapshot;

                        if (snapshotChild.GetValue(true) == null) continue;

                        User user = new User();
                        user.Uid = snapshotChild.Key;
                        user.DeviceToken = snapshotChild.Child("DeviceToken")?.GetValue(true)?.ToString();
                        user.Plataforma = snapshotChild.Child("Plataforma")?.GetValue(true)?.ToString();

                        var lat1 = snapshotChild.Child("Latitud")?.GetValue(true)?.ToString();
                        var lon1 = snapshotChild.Child("Longitud")?.GetValue(true)?.ToString();

                        if (double.TryParse(lat1, out double lat))
                            user.Latitud = lat;

                        if (double.TryParse(lon1, out double lon))
                            user.Latitud = lon;

                        _users.Add(user);
                    }
                }
                // return _users;
            }

            void GetAllNearestTokens()
            {
                if (_users != null && _users.Count > 0)
                {
                    foreach (User u in _users)
                    {
                        if (FirebaseManagerImplementation.GetDistanceInKm(_myLatitud, _myLongitud, u.Latitud, u.Longitud) <= _distance)
                        {
                            if (_tokens == null)
                            {
                                _tokens = new List<string>();

                            }
                            _tokens.Add(u.DeviceToken);
                        }
                    }
                }
                // return _tokens;
            }

            void SendNotifications()
            {
                if(_tokens != null && _tokens.Count > 0)
                {
                    FirebaseManagerImplementation fi = new FirebaseManagerImplementation();
                    foreach(var token in _tokens)
                    {
                        fi.SendNotificationToDeviceAsync("Messsage", token);
                    }
                }
            }
        }
    }
}
