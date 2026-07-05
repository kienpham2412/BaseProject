using UnityEngine;

public class EffectParticleSystem : Effect
{
    [SerializeField] private ParticleSystem[] effect;
    public ParticleSystem[] Effect => effect;
    
    private void Reset()
    {
        effect = GetComponentsInChildren<ParticleSystem>();
    }
    
    public override void Play()
    {
        effect[0].Play();
    }

    public ParticleSystem GetSubEffect(int index)
    {
        index = Mathf.Clamp(index, 0, effect.Length);
        return effect[index];
    }
}
