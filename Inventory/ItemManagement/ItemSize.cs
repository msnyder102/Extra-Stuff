using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSize {

    public ItemSizeType SizeType;

    public ItemSize(ItemSizeType type)
    {
        SizeType = type;

    }

    /// <summary>
    /// Number of rows this item will occupy
    /// </summary>
    /// <returns>rows</returns>
    public int Rows()
    {
        switch (SizeType)
        {
            case ItemSizeType.OneByOne:
            case ItemSizeType.OneByTwo:
                return 1;
            case ItemSizeType.TwoByOne:
            case ItemSizeType.TwoByTwo:
                return 2;
            case ItemSizeType.ThreeByOne:
            case ItemSizeType.ThreeByTwo:
                return 3;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Number of columns this item will occupy
    /// </summary>
    /// <returns>columns</returns>
    public int Cols()
    {
        switch (SizeType)
        {
            case ItemSizeType.OneByOne:
            case ItemSizeType.TwoByOne:
            case ItemSizeType.ThreeByOne:
                return 1;
            case ItemSizeType.OneByTwo:
            case ItemSizeType.TwoByTwo:
            case ItemSizeType.ThreeByTwo:
                return 2;
            default:
                return 0;
        }
    }

}
