using IzanamiWorkshop.Firebase.Core;
using IzanamiWorkshop.Firebase.Models;
using IzanamiWorkshop.Firebase.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IzanamiWorkshop.Firebase.GameComponents
{
    public class ItemListView : MonoBehaviour
    {
        [SerializeField] Transform itemsContent;
        [SerializeField] ItemView itemPrefab;
        private List<ItemView> items = new List<ItemView>();
        private Dictionary<string,ItemData> itemData = new Dictionary<string,ItemData>();

        //Gets called from UI Button
        public void HandleGetItemsList()
        {
            FirebaseController.Instance.GetItemData(FirebaseUtil.ItemDatabaseName, (items) =>
            {
                Debug.Log("Feteched Items List");
                Create(items);
            });
        }

        private void Create(Dictionary<string,ItemData> itemData)
        {
            Clear();
            this.itemData = itemData;
            foreach (var item in this.itemData)
            {
                ItemView itemView = Instantiate(itemPrefab) as ItemView;
                itemView.SetItemData(item.Key, item.Value);
                itemView.transform.SetParent(itemsContent.transform);
                itemView.gameObject.SetActive(true);
                items.Add(itemView);
            }
        }

        public void Clear()
        {
            if(items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if(items[i]!=null)
                        Destroy(items[i].gameObject);
                }
                items.Clear();
            }
            if(itemData != null)
                itemData.Clear();
        }
    }
}