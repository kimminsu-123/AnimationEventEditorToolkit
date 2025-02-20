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
        private uint _nextCallCnt;
        private uint _prevCallCnt;
        
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

            _nextCallCnt = 0;
            _prevCallCnt = 0;
        }
        
        private void Initialize(Animator animator)
        {
            _isInitialized = true;
            
            _receiver = animator.GetComponent<AnimationEventReceiver>();
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _nextCallCnt = (uint) stateInfo.normalizedTime;
            
            foreach (AnimationEventData data in eventReachedNormalizedTime)
            {
                if (!data.HasCalled && stateInfo.normalizedTime % 1f >= data.NormalizedTime)
                {
                    _receiver.Execute(data.Id);
                    data.HasCalled = true;
                }
            }
            
            foreach (AnimationEventData data in eventReachedNormalizedTime)
            {
                if (data.Loop && _nextCallCnt != _prevCallCnt)
                {
                    if (!data.HasCalled)
                    {
                        _receiver.Execute(data.Id);
                    }
                    data.HasCalled = false;
                }
            }
            _prevCallCnt = _nextCallCnt;
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