using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine;
using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Auth;
using IzanamiWorkshop.Firebase.Utils;
using IzanamiWorkshop.Firebase.Models;
using System.Text.RegularExpressions;

namespace IzanamiWorkshop.Firebase.Core
{
    public class FirebaseController : MonoBehaviour
    {
        public static FirebaseController Instance;

        private FirebaseAuth firebaseAuth;
        public FirebaseUser currentUser { get { return firebaseAuth.CurrentUser; }}

        private FirebaseStorage firebaseStorage;
        private StorageReference storageRef;
        private StorageReference bundleRef;
        private StorageReference prefabsRef;

        private FirebaseDatabase firebaseDatabase;

        public Action Initialized;
        private void Awake()
        {
            Instance = this;
        }

        private async void Start()
        {
            //initializing firebase
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    if (task.Result == DependencyStatus.Available)
                    {
                        Initialized?.Invoke();
                        InitAuth();
                        InitStorage();
                        InitDatabase();
                        Debug.Log("Firebase Initialized!!");
                    }
                }
            });
        }

        #region Auth

        private void InitAuth()
        {
            firebaseAuth = FirebaseAuth.DefaultInstance;
        }

        public void CreateAccountWithEmail(string email, string password, string displayName, Action<bool> callback = null)
        {
            firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    callback?.Invoke(false);
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    callback?.Invoke(false);
                    return;
                }

                // Firebase user has been created.

                UpdateDisplayName(displayName, callback);
            });
        }

        private void UpdateDisplayName(string displayName, Action<bool> callback = null)
        {
            FirebaseUser user = firebaseAuth.CurrentUser;
            if (user != null)
            {
                UserProfile profile = new UserProfile
                {
                    DisplayName = displayName,
                };
                user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task => {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("UpdateUserProfileAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                        return;
                    }
                    
                    Debug.Log("User profile updated successfully.");
                    callback?.Invoke(true);
                });
            }
        }

        public void SignInAccountWithEmail(string email, string password, Action<bool> callback = null)
        {
            firebaseAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    callback?.Invoke(false);
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    callback?.Invoke(false);
                    return;
                }

                callback?.Invoke(true);
            });
        }

        #endregion

        #region Storage

        private void InitStorage()
        {
            // Get a reference to the storage service, using the default Firebase App
            firebaseStorage = FirebaseStorage.DefaultInstance;

            // Create a storage reference from our storage service
            storageRef = firebaseStorage.GetReferenceFromUrl("gs://tryout-5a1c5.appspot.com");
            bundleRef = storageRef.Child("bundles");
        }

        //provides downloadable link that can be used by unity to download bundle from firebase
        public void GetDownloadURLForBundle(string bundleName, Action<string> callback)
        {
            //prefabsRef = bundleRef.Child("prefabs");
            prefabsRef = bundleRef.Child(bundleName);
            prefabsRef.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                // ... now download the file via WWW or UnityWebRequest.
                Debug.Log("Download URL: " + task.Result.AbsoluteUri);
                    callback(task.Result.AbsoluteUri);
            }
            });
        }

        #endregion

        #region Database

        private void InitDatabase()
        {
            // Get a reference to the storage service, using the default Firebase App
            firebaseDatabase = FirebaseDatabase.DefaultInstance;
        }

        //Sets or updates a Value, we just use it to update last logged in user data in database, just an example.
        public void SetValueAsJson(string dbName, string json, Action<bool> callback = null)
        {
            firebaseDatabase.GetReference(dbName).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    Debug.Log("Json Submitted to Database, Status Completed: " + task.IsCompleted);
                    callback(task.IsCompleted);
                }
            });
        }

        //push object to firebase database, this method will add new object to respective database
        public void PushToDatabaseAsJson(string dbName, string json, Action<bool> callback = null)
        {
            firebaseDatabase.GetReference(dbName).Push().SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    Debug.Log("Json Submitted to Database, Status Completed: " + task.IsCompleted);
                    callback(task.IsCompleted);
                }
            });
        }

        public string GetPushKey(string dbName)
        {
            return firebaseDatabase.GetReference(dbName).Push().Key;
        }

        //used to update or set values, we pass dictionary, with keys being path specific (parent/child) and value the data to be set
        public void PushItemData(IDictionary<string, object> toUpdateData, Action<bool> callback = null)
        {
            firebaseDatabase.RootReference.UpdateChildrenAsync(toUpdateData).ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    Debug.Log("Json Submitted to Database, Status Completed: " + task.IsCompleted);
                    callback(task.IsCompleted);
                }
            });
        }

        //Get list of item datas that user has created
        public void GetItemData(string dbName, Action<Dictionary<string,ItemData>> callback = null)
        {
            FirebaseDatabase.DefaultInstance.GetReference(dbName).GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted)
                {
                    // Handle the error...
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    object rawData = snapshot.GetValue(true);
                    Dictionary<string, ItemData> itemDatas = FirebaseUtil.ConvertDictionaryToItemData(rawData);
                    callback?.Invoke(itemDatas);
                }
            });
        }

        public void DeleteItemData(string dbName, Action callback = null)
        {
            FirebaseDatabase.DefaultInstance.GetReference(dbName).RemoveValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted)
                {
                    // Handle the error...
                }
                else if (task.IsCompleted)
                {
                    callback?.Invoke();
                }
            });
        }

        #endregion

    }
}
