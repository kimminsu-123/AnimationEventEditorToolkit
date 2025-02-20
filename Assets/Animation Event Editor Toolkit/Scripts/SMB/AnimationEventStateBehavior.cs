using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace KMS.AnimationToolkit
{
    public class AnimationEventStateBehavior : StateMachineBehaviour
    {
        [SerializeField] private AnimationEventDataContainer container;
        [SerializeField] private List<AnimationEventData> eventStateEnter;
        [SerializeField] private List<AnimationEventData> eventStateExit;
        [SerializeField] private List<AnimationEventData> eventReachedNormalizedTime;
        
        private AnimationEventReceiver _receiver;
        private bool _isInitialized;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_isInitialized)
            {
                Initialize(animator);
            }
            
            foreach (AnimationEventData data in eventStateEnter)
            {
                _receiver.Execute(data.Id);
            }
        }
        
        private void Initialize(Animator animator)
        {
            _isInitialized = true;
            
            _receiver = animator.GetComponent<AnimationEventReceiver>();
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var data in eventReachedNormalizedTime)
            {
                if (!data.HasCalled && stateInfo.normalizedTime % 1f >= data.NormalizedTime)
                {
                    _receiver.Execute(data.Id);
                    data.HasCalled = true;
                }
            }
            
            foreach (var data in eventReachedNormalizedTime)
            {
                if (data.Loop && data.HasCalled && stateInfo.normalizedTime % 1f < Time.deltaTime / stateInfo.length)
                {
                    data.HasCalled = false;
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            foreach (AnimationEventData data in eventStateExit)
            {
                _receiver.Execute(data.Id);
            }
        }
    }
}