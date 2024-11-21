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
        public float normalizedTime;
        public float fixedTime;
        
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
                    HasCalled = info.normalizedTime >= normalizedTime;
                    break;
                case TimeType.Fixed:
                    HasCalled = info.normalizedTime * info.length >= fixedTime;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return HasCalled;
        }
    }
}