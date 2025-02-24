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
        private int _currentCallCnt;
        private int _prevCallCnt;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_isInitialized)
            {
                Initialize(animator);
            }

            for (var index = 0; index < eventStateEnter.Count; index++)
            {
                AnimationEventData data = eventStateEnter[index];
                _receiver.Execute(data.Id);
            }

            _currentCallCnt = 0;
            _prevCallCnt = 0;
        }
        
        private void Initialize(Animator animator)
        {
            _isInitialized = true;
            
            _receiver = animator.GetComponent<AnimationEventReceiver>();
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _currentCallCnt = (int) stateInfo.normalizedTime;

            for (var index = 0; index < eventReachedNormalizedTime.Count; index++)
            {
                AnimationEventData data = eventReachedNormalizedTime[index];
                if (!eventReachedNormalizedTime[index].HasCalled && stateInfo.normalizedTime % 1f >= eventReachedNormalizedTime[index].NormalizedTime)
                {
                    _receiver.Execute(data.Id);
                    data.HasCalled = true;
                }
                
                if (data.Loop && _currentCallCnt != _prevCallCnt)
                {
                    if (!data.HasCalled)
                    {
                        _receiver.Execute(data.Id);
                    }

                    data.HasCalled = false;
                }
            }
            
            _prevCallCnt = _currentCallCnt;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            for (var index = 0; index < eventStateExit.Count; index++)
            {
                AnimationEventData data = eventStateExit[index];
                _receiver.Execute(data.Id);
            }
        }
    }
}