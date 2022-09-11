using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine;
using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Auth;

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

        public void CreateAccountWithEmail(string email, string password, string displayName, Action callback = null)
        {
            firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                // Firebase user has been created.

                UpdateDisplayName(displayName, callback);
            });
        }

        private void UpdateDisplayName(string displayName, Action callback = null)
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
                    callback?.Invoke();
                });
            }
        }

        public void SignInAccountWithEmail(string email, string password, Action callback = null)
        {
            firebaseAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                callback?.Invoke();
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
                //LoadAsset.Instance.DownloadAssetsFromFirebase(task.Result.AbsoluteUri);
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

        //Sets or updates a Value.
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
        public void PushNewData(IDictionary<string, object> toUpdateData, Action<bool> callback = null)
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

        #endregion

    }
}
