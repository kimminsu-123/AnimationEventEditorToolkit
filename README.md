# Animation Event Toolkit

애니메이션 이벤트를 Clip 의 Keyframe 수정 없이 받을 수 있도록 향상된 기능을 제공합니다.

## 런타임 사용 예제

``` cs
public AnimationEventReceiver receiver;

private void Start()
{
    receiver.AddEvent(1, DoSomething);
    receiver.AddEvent(2, DoSomething);
}

private void DoSomething()
{
    // Do something...
}

private void OnDestroy()
{
    receiver.RemoveEvent(1, DoSomething);
    receiver.RemoveEvent(2, DoSomething);
}
```

## 에디터 사용 예제

1. AnimationEventDataContainer 스크립터블 오브젝트를 생성합니다.
2. 스크립터블 오브젝트 안에 이벤트를 생성합니다.
3. Animator -> 상태 머신을 선택하여 AnimationEventStateMachineBehavior 를 추가합니다.
4. Container 필드에 (1) 에서 생성한 스크립터블 오브젝트를 등록합니다.
5. StateMachineBehavior 에서 호출할 이벤트를 등록합니다.
6. 이벤트를 수신받을 오브젝트에 AnimationEventReceiver 를 추가합니다.
7. Container 필드에 (1) 에서 생성한 스크립터블 오브젝트를 등록합니다.
8. 수신받을 이벤트 정보를 등록합니다.

## 설치 방법
- 패키지 매니저에서 아래와 같은 Git URL 을 입력하여 설치할 수 있습니다.
``` 
https://github.com/kimminsu-123/AnimationEventEditorToolkit.git
```
- 추가적으로 프로젝트의 manifest.json 파일안에 아래 라인을 입력하여 설치할 수 있습니다.
```
"com.kms.animationeventtoolkit": "https://github.com/kimminsu-123/AnimationEventEditorToolkit.git"
```