using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace KMS.AnimationToolkit
{   
    public class AnimationEventStateBehavior : StateMachineBehaviour
    {
        public AnimationEventDataContainer container;
        public List<uint> selectedEvents = new();
        
        private AnimationEventReceiver _receiver;
        private bool _isInitialized;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_isInitialized)
            {
                Initialize(animator);
            }
            
            foreach (var data in container.AnimationEventDataList)
            {
                data.HasCalled = false;
            }
            
            foreach (var data in container.AnimationEventDataList)
            {/*
                if (data.timeType == TimeType.Entered)
                {
                    _receiver.Execute(data.id);
                    data.HasCalled = true;
                }*/
            }
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            /*foreach (var data in container.AnimationEventDataList)
            {
                                ret = !HasCalled && (info.normalizedTime % 1f >= time);

                if (!data.HasCalled 
                    && data.timeType == TimeType.Normalized
                    && data.HasReachedTime(stateInfo))
                {
                    _receiver.Execute(data.id);
                    data.HasCalled = true;
                }
            }
            
            foreach (var data in container.AnimationEventDataList)
            {
                if (data.timeType == TimeType.Normalized 
                    && data.loop 
                    && data.HasCalled
                    && stateInfo.normalizedTime % 1f < Time.deltaTime / stateInfo.length)
                {
                    data.HasCalled = false;
                }
            }*/
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            /*foreach (var data in container.AnimationEventDataList)
            {
                if (data.timeType == TimeType.Exited)
                {
                    _receiver.Execute(data.id);
                    data.HasCalled = true;
                }
            }*/
        }


        private void Initialize(Animator animator)
        {
            _isInitialized = true;
            
            _receiver = animator.GetComponent<AnimationEventReceiver>();
        }
    }
}