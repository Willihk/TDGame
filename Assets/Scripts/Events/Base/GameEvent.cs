using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDGame.Events.Base
{
    [Serializable]
    public abstract class GameEvent : ScriptableObject
    {
        public event Action EventListeners = delegate { };

        public bool Enabled = true;

        [Button]
        public void Raise()
        {
            Debug.Log("Event Triggered: " + name);
            if (Enabled)
                EventListeners();
        }
    }

    [Serializable]
    public abstract class GameEvent<T> : ScriptableObject
    {
        public event Action<T> EventListeners = delegate { };

        public bool Enabled = true;

        [Button]
        public void Raise(T item)
        {
            Debug.Log("Event Triggered: " + name);
            if (Enabled)
                EventListeners(item);
        }
    }
}