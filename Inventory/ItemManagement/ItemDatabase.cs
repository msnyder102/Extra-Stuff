using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Database contains the base(normal) quality versions of all items in game
/// Game will copy an item out of the database when new items are spawned
/// </summary>
[CreateAssetMenu]
public class ItemDatabase : ScriptableObject{

    [System.Serializable]
    public class ItemList
    {
        public string sectionName = null;
        public List<Item> list = new List<Item>();
    }
    public ItemList[] items = new ItemList[ItemType.GetNames(typeof(ItemType)).Length];
}
