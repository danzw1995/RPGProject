﻿

using System.Collections.Generic;

namespace RPG.Stats
{
  public interface IModifierProvider
  {
     IEnumerable<float> GetAdditiveModifiers(Stat stat);
     IEnumerable<float> GetPercentModifiers(Stat stat);
  }
}
