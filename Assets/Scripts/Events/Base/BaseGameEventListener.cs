using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TDGame.Events.Base
{
    public abstract class BaseGameEventListener<T, GE, UER> : MonoBehaviour
         where GE : GameEvent<T>
         where UER : UnityEvent<T>
    {
        [AssetSelector]
        [SerializeField]
        protected GE _GameEvent;

        [SerializeField]
        protected UER _UnityEventResponse;

        protected void OnEnable()
        {
            if (_GameEvent != null)
            {
                _GameEvent.EventListeners += TriggerResponses;
            }
        }

        protected void OnDisable()
        {
            if (_GameEvent != null)
            {
                _GameEvent.EventListeners -= TriggerResponses;
            }
        }

        [ContextMenu("Trigger Responses")]
        public void TriggerResponses(T val)
        {
            //No need to nullcheck here, since UnityEvent already does that
            _UnityEventResponse.Invoke(val);
        }
    }

    public abstract class BaseGameEventListener<GE, UER> : MonoBehaviour
        where GE : GameEvent
        where UER : UnityEvent
    {
        [SerializeField]
        protected GE _GameEvent;

        [SerializeField]
        protected UER _UnityEventResponse;

        protected void OnEnable()
        {
            if (_GameEvent != null)
            {
                _GameEvent.EventListeners += TriggerResponses;
            }
        }

        protected void OnDisable()
        {
            if (_GameEvent != null)
            {
                _GameEvent.EventListeners -= TriggerResponses;
            }
        }

        [ContextMenu("Trigger Responses")]
        public void TriggerResponses()
        {
            _UnityEventResponse.Invoke();
        }
    }
}