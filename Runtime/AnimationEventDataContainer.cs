using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KMS.AnimationToolkit
{
    [CreateAssetMenu(fileName = "AnimationEventDataContainer", menuName = "AnimationToolkit/AnimationEventDataContainer", order = 0)]
    public class AnimationEventDataContainer : ScriptableObject
    {
        [SerializeField] private List<AnimationEventData> animationEventDataList = new();
        
        public List<AnimationEventData> AnimationEventDataList => animationEventDataList;
    }
}