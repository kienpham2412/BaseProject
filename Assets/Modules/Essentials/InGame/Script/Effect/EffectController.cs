using System;
using System.Collections.Generic;
using Essential;
using Pixelplacement;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Effect[] effectPrefabs;
    private Dictionary<EffectType, Pool<Effect>> effectPools;

    private void Awake()
    {
        InitEffectPools();
    }

    private void InitEffectPools()
    {
        effectPools = new Dictionary<EffectType, Pool<Effect>>(effectPrefabs.Length);
        
        foreach (var e in effectPrefabs)
        {
            var pool = new Pool<Effect>(e, container, Vector3.up * 100);
            
            effectPools.Add(e.EffectType, pool);
        }
    }

    public Effect GetEffect(EffectType type)
    {
        if (!effectPools.ContainsKey(type)) return null;
        
        return effectPools[type].Get();
    }

    public void SpawnEffect(EffectType type, Vector3 position)
    {
        var e = GetEffect(type);
        if (e == null) return;
        
        e.transform.position = position;
        e.gameObject.SetActive(true);
        e.Play();
    }
}