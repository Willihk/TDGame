using System;
using UnityEngine;

namespace TDGame.Events.Base
{
    [Serializable]
    public abstract class GameEvent : ScriptableObject
    {
        public event Action EventListeners = delegate { };

        public bool Enabled = true;

        public void Raise()
        {
            if (Enabled)
                EventListeners();
        }
    }

    [Serializable]
    public abstract class GameEvent<T> : ScriptableObject
    {
        public event Action<T> EventListeners = delegate { };

        public bool Enabled = true;

        public void Raise(T item)
        {
            if (Enabled)
                EventListeners(item);
        }
    }
}