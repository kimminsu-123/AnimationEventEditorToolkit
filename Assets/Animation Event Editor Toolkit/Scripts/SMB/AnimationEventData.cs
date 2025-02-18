using System;
using UnityEngine;

namespace KMS.AnimationToolkit
{
    public enum TimeType
    {
        Entered,
        Exited,
        Normalized,
        //Fixed, 일단은 사용하지 않는 것으로 설정
    }

    [Serializable]
    public class AnimationEventData 
    {
        [SerializeField] public uint id;
        [SerializeField] public string title;
        [SerializeField] public bool loop;

        public bool HasCalled { get; set; } = false;
        public uint Id => id;
        public string Title => title;
        public bool Loop => loop;
    }
}