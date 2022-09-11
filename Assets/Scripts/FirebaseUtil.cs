using IzanamiWorkshop.Firebase.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IzanamiWorkshop.Firebase.Utils
{
    public static class FirebaseUtil
    {
        public static Dictionary<string, object> ConvertMapDataToDictionary(string path, ItemData itemData)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[path + "/Name"] = itemData.Name;
            data[path + "/Data"] = itemData.Data;
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
    }
}

