using System;
using System.Collections.Generic;
using System.Linq;
using TDGame.Extensions;
using UnityEngine;

namespace TDGame.Systems.Stats
{
    [Serializable]
    public class Stat
    {
        public string Name;
        public float BaseValue;
        
        public readonly IReadOnlyCollection<StatModifier> StatModifiers;

        [Tooltip("Should be '0' if the cap should be ignored")]
        public float Cap;

        public float Value
        {
            get
            {
                if (isDirty)
                {
                    value = CalculateFinalValue();
                    isDirty = false;
                }

                return value;
            }
        }

        protected bool useCap;
        protected float value;
        
        protected bool isDirty = true;
        protected readonly List<StatModifier> modifiers;

        public Stat(float baseValue, float cap) : this(baseValue)
        {
            Cap = cap;
            useCap = true;
        }

        public Stat(float baseValue) : this()
        {
            BaseValue = baseValue;
        }

        public Stat()
        {
            modifiers = new List<StatModifier>();
            StatModifiers = modifiers.AsReadOnly();
        }

        public bool RemoveModifier(StatModifier mod)
        {
            if (modifiers.Remove(mod))
            {
                isDirty = true;
                return true;
            }

            return false;
        }

        public void RemoveAllModifiersFromSource(object source)
        {
            modifiers.RemoveAll(x => x.Source == source);
        }

        public void AddModifier(StatModifier mod)
        {
            modifiers.Add(mod);
            modifiers.OrderBy(x => x.Order);
        }

        protected float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < modifiers.Count; i++)
            {
                StatModifier mod = modifiers[i];

                if (mod.Type == StatModType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == StatModType.PercentAdditive)
                {
                    sumPercentAdd += mod.Value;

                    // If we're at the end of the list OR the next modifer isn't of this type
                    if (i + 1 >= modifiers.Count || modifiers[i + 1].Type != StatModType.PercentAdditive)
                    {
                        finalValue *=
                            1 + sumPercentAdd; // Multiply the sum with the "finalValue", like we do for "PercentMult" modifiers
                        sumPercentAdd = 0;
                    }
                }
                else if (mod.Type == StatModType.PercentMultiplier)
                {
                    finalValue *= 1 + mod.Value;
                }
            }

            if (useCap)
                return (float) Math.Round(finalValue, 4).Clamp(0, Cap);

            return (float) Math.Round(finalValue, 4);
        }

        public void RecalculateValue()
        {
            value = CalculateFinalValue();
        }
    }
}