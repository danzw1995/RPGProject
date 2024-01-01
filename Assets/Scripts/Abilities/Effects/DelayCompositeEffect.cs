using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Filters
{
  [CreateAssetMenu(fileName = "Delay Composite Effect", menuName = "Abilities/Effects/Delay Composite", order = 0)]
  public class DelayCompositeEffect : EffectStrategy
  {
    [SerializeField] private float delay = 0;
    [SerializeField] private EffectStrategy[] delayEffects;

    public override void StartEffect(AbilityData data, Action finished)
    {

      data.StartCoroutine(DelayEffects(data, finished));
    }

    private IEnumerator DelayEffects(AbilityData data, Action finished)
    {
      yield return new WaitForSeconds(delay);

      foreach (EffectStrategy effectStrategy in delayEffects)
      {
        effectStrategy.StartEffect(data, finished);
      }
      finished();
    }
  }
}