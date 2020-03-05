using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base for every item in game
/// Attributes/functionality that is common for all items in game regardless of type should go here
/// i.e. name/description/2D texture etc.
/// </summary>
[System.Serializable]
[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public ItemQualityType quality;
    public ItemType type;
    public string iconPath;
    public ItemSize size;
    private Sprite _icon;
    public Sprite icon
    { get
        {
            if (_icon == null)
            {
                _icon = Resources.Load<Sprite>(iconPath);
            }
            return _icon;
        }
    }
    public int uniqueID;
    private List<StatModifier> _statModifiers;
    public List<StatModifier> statModifiers
    {
        get
        {
            if(_statModifiers == null)
            {
                _statModifiers = new List<StatModifier>();
            }
            return _statModifiers;
        }
    }

    /// <summary>
    /// Deep clone of item from item database.
    /// </summary>
    /// <returns>Deep Copy of item</returns>
    public Item DeepCopy()
    {
        return Instantiate<Item>(this);
    }

}
