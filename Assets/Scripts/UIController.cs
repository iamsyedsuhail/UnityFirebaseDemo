using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using IzanamiWorkshop.Firebase.Core;
using IzanamiWorkshop.Firebase.Models;
using Object = System.Object;
using IzanamiWorkshop.Firebase.Utils;
using System;
using UnityEngine.UI;

namespace IzanamiWorkshop.Firebase.GameComponents
{
    public class UIController : MonoBehaviour
    {
        //SignUp Panel
        [Header("SignUp")]
        [SerializeField] private TMP_InputField signUpEmail;
        [SerializeField] private TMP_InputField signUpPassword;
        [SerializeField] private TMP_InputField signUpDisplayName;

        //Login Panel
        [Header("SignIn")]
        [SerializeField] private TMP_InputField signInEmail;
        [SerializeField] private TMP_InputField signInPassword;

        //New Item Panel
        [Header("Items")]
        [SerializeField] private TMP_InputField itemName;
        [SerializeField] private TMP_InputField itemDamage;

        //List Items Panel
        [SerializeField] ItemListView itemListPanel;

        //Info Panel
        [SerializeField] private TMP_Text displayName;

        //Panels
        [SerializeField] private GameObject initPanel;
        [SerializeField] private GameObject signUpPanel;
        [SerializeField] private GameObject signInPanel;
        [SerializeField] private GameObject newItemPanel;
        [SerializeField] private GameObject infoPanel;

        //buttons
        [SerializeField] private Button[] authBtns;

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
            EnableAuthButtons(false);
            FirebaseController.Instance.CreateAccountWithEmail(signUpEmail.text, signUpPassword.text, signUpDisplayName.text, (success) => {
                if (success)
                {
                    Debug.Log("Created User");
                    HandleUserValidated();
                }
                else
                {
                    EnableAuthButtons(true);
                }

            });
        }

        //Gets called from UI Button
        public void HandleSignIn()
        {
            EnableAuthButtons(false);
            FirebaseController.Instance.SignInAccountWithEmail(signInEmail.text, signInPassword.text, (success) => {
                if (success)
                {
                    Debug.Log("Signed In User");
                    HandleUserValidated();
                }
                else
                {
                    EnableAuthButtons(true);
                }
            });
        }

        //Gets called from UI Button
        public void HandleNewItemSubmit()
        {
            //create new item
            ItemData item = new ItemData();
            item.Name = itemName.text;
            item.Damage = itemDamage.text;
            item.CreatorName = FirebaseController.Instance.currentUser.DisplayName;
            item.CreatorID = FirebaseController.Instance.currentUser.UserId;

            //create dictionary with key pointing to respective database and child.
            Dictionary<string, object> data = new Dictionary<string, object>();

            //get unique push key from item database which we will use as reference in userItems and child items db
            string key = FirebaseController.Instance.GetPushKey(FirebaseUtil.ItemDatabaseName);
            //converts objects to dictionary
            var maps = FirebaseUtil.ConvertItemDataToDictionary(FirebaseUtil.ItemDatabaseName + "/" + key, item);
            var userMaps = FirebaseUtil.ConvertItemDataToDictionary(FirebaseUtil.UserItemssDatabaseName + "/" + item.CreatorID + "/" + key, item);
            //below is a extension method that acts as add range
            data.MoveDictionary(maps);
            data.MoveDictionary(userMaps);
            //pass dictionary with all values to update
            FirebaseController.Instance.PushItemData(data, (bool taskCompleted) => {
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
            newItemPanel.SetActive(true);
            itemListPanel.gameObject.SetActive(true);
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
            FirebaseController.Instance.SetValueAsJson(FirebaseUtil.StatusDatabaseName, json, (bool taskCompleted) => {
                Debug.Log("UserData Submitted");
            });
        }

        private void EnableAuthButtons(bool enable)
        {
            for (int i = 0; i < authBtns.Length; i++)
            {
                authBtns[i].interactable = enable;
            }
        }
    }
}
