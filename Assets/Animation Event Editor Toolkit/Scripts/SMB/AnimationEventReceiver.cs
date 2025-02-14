using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace KMS.AnimationToolkit
{
    [Serializable]
    public class MappedEvent
    {
        public uint id;
        public UnityEvent callback;

        public MappedEvent(uint id)
        {
            this.id = id;
        }
    }
    
    public class AnimationEventReceiver : MonoBehaviour
    {
        public AnimationEventDataContainer container;
        public List<MappedEvent> mappedEvents = new();
        
        public void Execute(uint id)
        {
            MappedEvent evt = mappedEvents.First(x => x.id.Equals(id));

            evt.callback?.Invoke();
        }
    }
}