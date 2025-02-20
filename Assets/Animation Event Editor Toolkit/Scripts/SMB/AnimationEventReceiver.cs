using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KMS.AnimationToolkit
{
    [Serializable]
    public class MappedEvent
    {
        public uint id;
        public string title;
        public UnityEvent callback;

        public MappedEvent(uint id)
        {
            this.id = id; 
            callback = new UnityEvent();
        }
    }
    
    public class AnimationEventReceiver : MonoBehaviour
    {
        [SerializeField] private AnimationEventDataContainer container;
        [SerializeField] private List<MappedEvent> mappedEvents;

        private readonly Dictionary<uint, UnityEvent> _dictEvents = new();

        private void Awake()
        {
            foreach (MappedEvent mappedEvent in mappedEvents)
            {
                _dictEvents.TryAdd(mappedEvent.id, mappedEvent.callback);
            }
        }

        public void Execute(uint id)
        {
            if (_dictEvents.TryGetValue(id, out UnityEvent unityEvent))
            {
                unityEvent?.Invoke();
            }
        }

        public void AddEvent(uint id, UnityAction callback)
        {
            if (!_dictEvents.ContainsKey(id))
            {
                _dictEvents.Add(id, new UnityEvent());
            }
            _dictEvents[id].AddListener(callback);
        }
        
        public void RemoveEvent(uint id, UnityAction callback)
        {
            if (_dictEvents.TryGetValue(id, out UnityEvent unityEvent))
            {
                unityEvent.RemoveListener(callback);
            }
        }
    }
}