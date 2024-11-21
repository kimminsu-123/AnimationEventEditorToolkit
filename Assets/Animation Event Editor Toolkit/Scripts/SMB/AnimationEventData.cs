using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace KMS.AnimationToolkit
{
    public enum TimeType
    {
        Normalized,
        Fixed,
    }

    [Serializable]
    public struct AnimationEventData 
    {
        public int id;

        public TimeType timeType;
        public float time;
        public string title;
        
        public bool HasCalled { get; private set; }

        public void Reset()
        {
            HasCalled = false;
        }

        public bool HasReachedTime(AnimatorStateInfo info)
        {
            switch (timeType)
            {
                case TimeType.Normalized:
                    HasCalled = info.normalizedTime >= time;
                    break;
                case TimeType.Fixed:
                    HasCalled = info.normalizedTime * info.length >= time;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return HasCalled;
        }
    }
}