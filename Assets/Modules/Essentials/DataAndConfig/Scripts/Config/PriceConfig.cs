using System;
using UnityEngine;

[Serializable]
public class ItemPriceInfo
{
    public int itemId;
    public Collectible price;
}

[Serializable]
public class PriceConfig
{
    public ItemPriceInfo[] prices;

    public Collectible GetPriceData(CollectibleType type)
    {
        foreach (var p in prices)
            if (p.itemId == (int)type)
                return p.price;

        return prices[0].price;
    }
}