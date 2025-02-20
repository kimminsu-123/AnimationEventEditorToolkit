using System;
using UnityEngine;

namespace KMS.AnimationToolkit
{
    [Serializable]
    public class AnimationEventData 
    {
        [SerializeField] private uint id;
        [SerializeField] private string title;
        
        [SerializeField] private bool loop;             // 이 값은 각 데이터별로 다를 예정 (따라서 SO 에는 적용 X)
        [SerializeField] private float normalizedTime;  // 이 값은 각 데이터별로 다를 예정 (따라서 SO 에는 적용 X)

        public uint Id => id;
        public string Title => title;
        public bool Loop => loop;
        public float NormalizedTime => normalizedTime;
        
        public bool HasCalled { get; set; } = false;
    }
}