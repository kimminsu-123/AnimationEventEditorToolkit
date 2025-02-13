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
        //Fixed, 일단은 사용하지 않는 것으로 설정
    }

    [Serializable]
    public class AnimationEventData 
    {
        public uint id;

        public TimeType timeType;
        public float time;
        public string title;
        public bool loop;
        
        public bool HasCalled { get; private set; }

        public void Reset()
        {
            HasCalled = false;
        }

        public void Call()
        {
            HasCalled = true;
        }

        public void Repeat(AnimatorStateInfo stateInfo)
        {
            if (stateInfo.loop)
            {
                
            }
        }

        public bool HasReachedTime(AnimatorStateInfo info)
        {
            switch (timeType)
            {
                case TimeType.Normalized:
                    HasCalled = !HasCalled && (info.normalizedTime % 1f >= time);
                    break;
                /*case TimeType.Fixed:
                    HasCalled = info.normalizedTime * info.length >= time;
                    break;*/
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return HasCalled;
        }
    }
}