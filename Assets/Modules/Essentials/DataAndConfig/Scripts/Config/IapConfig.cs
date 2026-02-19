using System;
using UnityEngine;

[Serializable]
public class IapInfo
{
    public string id;
    public string name;
    public Collectible[] rewards;
}

[Serializable]
public class IapConfig
{
    public IapInfo[] products;

    public IapInfo GetInfo(string id)
    {
        foreach (var p in products)
        {
            if (p.id.CompareTo(id) != 0) continue;

            return p;
        }

        return null;
    }

    public Collectible[] GetRewards(string id)
    {
        foreach (var p in products)
        {
            if (p.id.CompareTo(id) != 0) continue;

            return p.rewards;
        }

        return new Collectible[] { new(CollectibleType.Coin, 10) };
    }
}