using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Slots are used to hold items
/// ex: inventory spaces, equipment 
/// </summary>
public class Slot
{
    /// <summary>
    /// Determines item types able to contained in this slot
    /// </summary>
    public SlotType type { get; private set; }
    /// <summary>
    /// Current item being held by this slot
    /// </summary>
    public Item item { get; private set; }
    /// <summary>
    /// Whether this slot is currently occupied by an item
    /// </summary>
    public bool isOccupied { get; private set; }

    public Slot(SlotType t)
    {
        type = t;
        item = null;
        isOccupied = false;
    }

    /// <summary>
    /// Returns true if this slot is allowed to hold the given item
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public bool ValidateSlot(Item item)
    {
        switch (type)
        {
            case SlotType.Inventory:    //inventory slots can hold anything
                return true;
            case SlotType.Mainhand:
                return (item.type == ItemType.Sword); //TODO extend for other weapons
            case SlotType.Offhand:
                return (item.type == ItemType.Shield) ||  //TODO extend for other off hands
                       (item.type == ItemType.Sword); 
            case SlotType.Helmet:
                return (item.type == ItemType.Helmet);
            case SlotType.Chest:
                return (item.type == ItemType.Chest);
            case SlotType.Boots:
                return (item.type == ItemType.Boots);
            case SlotType.Artifact:
                return (item.type == ItemType.Artifact);
            default:
                return false;
        }
    }

    /// <summary>
    /// Adds item to slot
    /// Will not add the item if it is invalid for the slot or if the slot is already occupied
    /// Caller needs to handle removing the previous item themselves
    /// </summary>
    /// <param name="item"></param>
    /// <returns>
    /// True if item was added to slot
    /// False otherwise
    /// </returns>
    public bool AddItem(Item item)
    {
        if (!isOccupied && ValidateSlot(item))
        {
            isOccupied = true;
            this.item = item;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Removes item currently occupying the slot
    /// </summary>
    /// <returns>The item previously in the slot</returns>
    public Item RemoveItem()
    {
        isOccupied = false;
        Item temp = item;
        item = null;
        return temp;
    }
}
