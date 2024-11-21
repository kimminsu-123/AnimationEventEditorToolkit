using Animation_Event_Editor_Toolkit.Scripts.Handler;
using UnityEngine;

namespace KMS.AnimationToolkit
{   
    // 사용할 Container 가 정해지면 해당 Container 에서 사용할 Id 값을 따로 전달받을 수 있도록 하기
    
    public class AnimationEventStateBehavior : StateMachineBehaviour
    {
        public AnimationEventDataContainer container;
        
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
                data.Reset();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var data in container.AnimationEventDataList)
            {
                if (!data.HasCalled && data.HasReachedTime(stateInfo))
                {
                    _receiver.Execute(data.id);
                }
            }
        }

        private void Initialize(Animator animator)
        {
            _isInitialized = true;
            
            _receiver = animator.GetComponent<AnimationEventReceiver>();
        }
    }
}