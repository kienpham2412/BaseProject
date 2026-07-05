using System;
using Essential;
using UnityEngine;
using UnityEngine.Pool;

public enum EffectType
{
    BoxClose,
    PinSparkle,
    ScrewOut,
    Hammer,
    UnlockSlot
}

public abstract class Effect : PoolInstance<Effect>
{
    [field: SerializeField] public EffectType EffectType { get; private set; }
    public override ObjectPool<Effect> Pool { protected get; set; }

    public override void Release()
    {
        Pool.Release(this);
    }

    public abstract void Play();

    protected virtual void OnDisable()
    {
        Release();
    }
}
