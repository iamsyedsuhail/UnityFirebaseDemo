using IzanamiWorkshop.Firebase.Core;
using IzanamiWorkshop.Firebase.Models;
using IzanamiWorkshop.Firebase.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IzanamiWorkshop.Firebase.GameComponents
{
    public class ItemView : MonoBehaviour
    {
        private ItemData itemData;
        private string itemID;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text creatorName;

        public void SetItemData(string itemID, ItemData itemData)
        {
            this.itemID = itemID;
            this.itemData = itemData;
            itemName.text = "Item Name: "+itemData.Name;
            creatorName.text = "Creator Name: " + itemData.CreatorName;
        }

        //Gets called from UI Button
        public void HandleDeleteItem()
        {
            //delete from item database
            FirebaseController.Instance.DeleteItemData(FirebaseUtil.ItemDatabaseName+"/"+itemID, HandleDeletedItem);
            //delete item reference from items by user database
            FirebaseController.Instance.DeleteItemData(FirebaseUtil.UserItemssDatabaseName +"/"+FirebaseController.Instance.currentUser.UserId +"/" + itemID);
        }

        private void HandleDeletedItem()
        {
            Debug.Log("Deleted ItemID: " + itemID);
            //if item is deleted in database, destroy this gameobject on success
            Destroy(gameObject);
        }

    }
}
