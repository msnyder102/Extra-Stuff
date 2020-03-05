using UnityEngine;
using System.Collections;

public class ItemDatabaseManager{

    private static readonly ItemDatabaseManager _instance = new ItemDatabaseManager();
    public static ItemDatabaseManager Instance { get { return _instance; } }

    private ItemDatabase _database;

    private int _uniqueIDCounter = 0;

    public void Init() { }

    public ItemDatabaseManager()
    {
        _database = Resources.Load<ItemDatabase>("ScriptableObjects/Databases/ItemDatabase");
        if (_database == null)
        {
            Debug.Log("Could not find Item Database asset in Resources folder");
        } 
    }

    public Item GetItem(ItemType type, int index)
    {
        return _database.items[(int)type].list[index].DeepCopy();
    }

    /// <summary>
    /// Returns a unique number to represent an item
    /// </summary>
    /// <returns>unique integer</returns>
    public int GenerateUniqueID()
    {
        _uniqueIDCounter += 1;
        return _uniqueIDCounter;
    }
}
