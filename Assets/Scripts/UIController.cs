using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using IzanamiWorkshop.Firebase.Core;
using IzanamiWorkshop.Firebase.Models;
using Object = System.Object;
using IzanamiWorkshop.Firebase.Utils;
using System;

namespace IzanamiWorkshop.Firebase.GameComponents
{
    public class UIController : MonoBehaviour
    {
        private const string statusDatabase = "Status";
        private const string itemDatabaseName = "Items";
        private const string userItemssDatabaseName = "ItemsByUsers";

        //SignUp Panel
        [Header("SignUp")]
        [SerializeField] private TMP_InputField signUpEmail;
        [SerializeField] private TMP_InputField signUpPassword;
        [SerializeField] private TMP_InputField signUpDisplayName;

        //Login Panel
        [Header("SignIn")]
        [SerializeField] private TMP_InputField signInEmail;
        [SerializeField] private TMP_InputField signInPassword;

        //Items Panel
        [Header("Items")]
        [SerializeField] private TMP_InputField itemName;
        [SerializeField] private TMP_InputField itemData;

        //Info Panel
        [SerializeField] private TMP_Text displayName;

        //Panels
        [SerializeField] GameObject initPanel;
        [SerializeField] GameObject signUpPanel;
        [SerializeField] GameObject signInPanel;
        [SerializeField] GameObject itemPanel;
        [SerializeField] GameObject infoPanel;

        private void OnEnable()
        {
            if (FirebaseController.Instance != null)
                FirebaseController.Instance.Initialized += HandleFirebaseInitialization;
        }

        private void OnDisable()
        {
            if (FirebaseController.Instance != null)
                FirebaseController.Instance.Initialized -= HandleFirebaseInitialization;
        }

        //Gets called from UI Button
        public void HandleSignUp()
        {
            FirebaseController.Instance.CreateAccountWithEmail(signUpEmail.text, signUpPassword.text, signUpDisplayName.text, () => {
                Debug.Log("Created User");
                HandleUserValidated();
            });
        }

        public void HandleSignIn()
        {
            FirebaseController.Instance.SignInAccountWithEmail(signInEmail.text, signInPassword.text, () => {
                Debug.Log("Signed In User");
                HandleUserValidated();
            });
        }

        public void HandleItemSubmit()
        {
            //create new item
            ItemData item = new ItemData();
            item.Name = itemName.text;
            item.Data = itemData.text;
            item.CreatorName = FirebaseController.Instance.currentUser.DisplayName;
            item.CreatorID = FirebaseController.Instance.currentUser.UserId;

            //create dictionary with key pointing to respective database and child.
            Dictionary<string, Object> data = new Dictionary<string, Object>();

            //get unique push key from item database which we will use as reference in userItems and child items db
            string key = FirebaseController.Instance.GetPushKey(itemDatabaseName);
            //converts objects to dictionary
            var maps = FirebaseUtil.ConvertMapDataToDictionary(itemDatabaseName + "/" + key, item);
            var userMaps = FirebaseUtil.ConvertMapDataToDictionary(userItemssDatabaseName + "/" + item.CreatorID + "/" + key, item);
            //below is a extension method that acts as add range
            data.MoveDictionary(maps);
            data.MoveDictionary(userMaps);
            //pass dictionary with all values to update
            FirebaseController.Instance.PushNewData(data, (bool taskCompleted) => {
                Debug.Log("New Item Submitted");
            });
        }

        private void HandleFirebaseInitialization()
        {
            initPanel.SetActive(false);
        }

        private void HandleUserValidated()
        {
            signUpPanel.SetActive(false);
            signInPanel.SetActive(false);
            itemPanel.SetActive(true);
            UpdateDisplayName();
            PushClientStatus();
        }

        private void UpdateDisplayName()
        {
            string name = FirebaseController.Instance.currentUser.DisplayName;
            Debug.Log("UpdateDisplayName " + name);
            displayName.text = "Hi, " + name;
            infoPanel.SetActive(true);
        }

        public void PushClientStatus()
        {
            Status status = new Status();
            status.DisplayName = FirebaseController.Instance.currentUser.DisplayName;
            status.Message = DateTime.Now.ToString();
            string json = JsonUtility.ToJson(status);
            FirebaseController.Instance.SetValueAsJson(statusDatabase, json, (bool taskCompleted) => {
                Debug.Log("UserData Submitted");
            });
        }
    }
}
