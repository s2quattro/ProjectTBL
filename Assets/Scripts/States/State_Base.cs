using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// AI State 스크립트들의 기본 부모클래스
public class State_Base : MonoBehaviour {
    
    [Serializable]
    public class StateInfo  // 이름과 동작시간, 쿨타임과 다음에 올 State등을 Inspecter에서 지정할 수 있는 클래스
    {
        public string stateName;
        public float minRunningTime;
        public float maxRunningTime;
        public float cooltime = 0f;
        public NextState[] nextState;
    }

    [Serializable]
    public class NextState  // 해당 State 다음에 올 State들과 그 State들의 기본 가중치(확률상)를 지정하는 클래스
    {
        public string stateName;
        [Range(0f,100f)] public float statePer = 1f;
    }

    public StateInfo _stateInfo;

    protected IEnumerator coroutine1;
    protected IEnumerator coroutine2;

    [HideInInspector] public State_Controller _stateController;
    [HideInInspector] public GameObject character;
    [HideInInspector] public float remainTime = 0f;
    [HideInInspector] public float curCooltime = 0f;
    [HideInInspector] public bool canStateOver = false;
    [HideInInspector] public bool isStateEnter = false;
	[HideInInspector] public bool isAnimOver = false;
    [HideInInspector] public int receiveDamage;

    [HideInInspector] public AnimatorStateInfo animInfo;

    [HideInInspector] public Vector3 dir;
    [HideInInspector] public Quaternion lookRotate;

    // 기본 상태 변수들을 초기화하는 함수
    public virtual void InitStateVars() { }

    // 조건 등을 비교하여 스테이트의 최종 가중치를 결정하는 함수
    public virtual float GetMultPer()
    {
        float multiPer = 1;
        return multiPer;
    }

    // State 진입시에 호출되는 함수
    public virtual void OnEnterState() { }

    // State 갱신시에 호출되는 함수
    public virtual void OnUpdateState() { }

    // State 갱신시 메세지를 같이 보내는 함수
    public virtual void OnUpdateState(int message) { }

    // State가 종료될 때 호출되는 함수. 강제로 다음 스테이트로 넘어가야 하는 상황일때도 호출되어 State를 빠르게 마무리
    public virtual void OnExitState() { }
    
    // 남은 시간을 체크하여 다음 스테이트로 업데이트
    public void CheckTime()
    {
        if (remainTime > 0f)
        {
            remainTime -= Time.deltaTime;
        }
        else
        {
            if (canStateOver)
            {
                _stateController.requestUpdate = true;
            }
        }
    }
	
    // 해당 이름을 가진 애니메이션이 매개변수로 전달받은 비율(0f~1f)에 도달했을때 종료되는 코루틴
	protected IEnumerator WaitForAnimation(string name, float ratio)
	{
		isAnimOver = false;

        AnimatorStateInfo animInfo = _stateController._actionManager.anim.GetCurrentAnimatorStateInfo(0);

        while ((animInfo.normalizedTime < ratio) || !animInfo.IsName(name))
		{
			animInfo = _stateController.anim.GetCurrentAnimatorStateInfo(0);
            			
			yield return new WaitForEndOfFrame();
		}

		isAnimOver = true;
	}

    // State 진입시 호출되어 초기화하는 함수
    public void StateEnterInit()
	{  
		remainTime = UnityEngine.Random.Range(_stateInfo.minRunningTime, _stateInfo.maxRunningTime);
	}

    // State 종료시 호출되어 초기화하는 함수
	public void StateExitInit()
	{
        curCooltime = _stateInfo.cooltime;
		canStateOver = false;
	}

    // AI 캐릭터가 Target을 바라보도록하는 함수
    public void LookRotation()
    {
        if (_stateController.target)    // 컨트롤러의 타겟 정보가 존재하면
        {   
            // 타겟과의 노말벡터를 구한 후 y방향으로 보간회전시킨다.
            dir = (_stateController.target.transform.position - character.transform.position).normalized;
            lookRotate = Quaternion.LookRotation(dir);
            lookRotate.x = 0f;
            lookRotate.z = 0f;
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, lookRotate, _stateController.rotateSpeed * Time.deltaTime);
        }
    }

    // AI 캐릭터가 매개변수로 전달받은 위치로 회전하도록 하는 함수
    public void LookRotation(Vector3 location)
    {
        // 목표 위치와의 노말벡터를 구한 후 마찮가지로 y방향으로 보간회전시킨다.
        dir = (location - character.transform.position).normalized;
        lookRotate = Quaternion.LookRotation(dir);
        lookRotate.x = 0f;
        lookRotate.z = 0f;
        character.transform.rotation = Quaternion.Slerp(character.transform.rotation, lookRotate, _stateController.rotateSpeed * Time.deltaTime);
    }

	//public IEnumerator onEnterState(State_Manager.CharacterInfo characterInfo)
	//{
	//    while (true)
	//    {
	//        break;
	//        yield return null;
	//        yield return new WaitForSeconds(1f);
	//    }
	//    Debug.Log(stateInfo.stateName + "End");
	//}
}
