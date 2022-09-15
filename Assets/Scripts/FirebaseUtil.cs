using IzanamiWorkshop.Firebase.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IzanamiWorkshop.Firebase.Utils
{
    public static class FirebaseUtil
    {
        public const string StatusDatabaseName = "Status";
        public const string ItemDatabaseName = "Items";
        public const string UserItemssDatabaseName = "ItemsByUsers";

        public static Dictionary<string, object> ConvertItemDataToDictionary(string path, ItemData itemData)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[path + "/Name"] = itemData.Name;
            data[path + "/Damage"] = itemData.Damage;
            data[path + "/CreatorName"] = itemData.CreatorName;
            data[path + "/CreatorID"] = itemData.CreatorID;
            return data;
        }

        public static void MoveDictionary(this Dictionary<string, object> dest, Dictionary<string, object> source)
        {
            foreach (var item in source)
            {
                dest.Add(item.Key,item.Value);
            }
        }

        public static Dictionary<string, ItemData> ConvertDictionaryToItemData(object source)
        {
            var items = source as Dictionary<string, object>; // to get items list
            Dictionary<string, ItemData> data = new Dictionary<string, ItemData>();

            if(items==null || items.Count==0)
            {
                return data;
            }

            int i = 0;
            foreach (var item in items)
            {
                var rawItemData = item.Value as Dictionary<string, object>;// to get item data from value
                ItemData itemData = new ItemData()
                {
                    Name = rawItemData["Name"] as string,
                    Damage = rawItemData["Damage"] as string,
                    CreatorID = rawItemData["CreatorID"] as string,
                    CreatorName = rawItemData["CreatorName"] as string
                };
                data.Add(item.Key, itemData);
                i++;
            }
            return data;
        }
    }
}

