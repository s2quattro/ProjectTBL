using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class State_Controller : Controller_Base
{
    private Dictionary<string, State_Base> stateFindDic = new Dictionary<string, State_Base>();

    //private String preState;
    private string curState;
    private string nextState = null;
    private string hitState = null;
    private string blockState = null;
    private string deathState = null;
    private string defaultState;   

    private State_Base _curStateEntity;

    [Range(0.5f, 5f)] public float moveSpeed = 1.5f;
    [Range(0.1f, 1f)] public float moveBackMult = 0.7f;
    [Range(0.1f, 1f)] public float moveSideMult = 0.8f;
    [Range(0.5f, 5f)] public float runSpeed = 2.5f;
    [Range(1f, 5f)] public float rotateSpeed = 4f;

    [Range(1f, 100f)] public float distanceLimit = 20f;

    public bool findTargetAfterKill = true;

    // 필요할 경우 프로퍼티로 바꿀것
    [HideInInspector] public GameObject target = null;
    [HideInInspector] public Controller_Base targetController;
    [HideInInspector] public float targetDistance;
    [HideInInspector] public bool isCombatMode = false;
    [HideInInspector] public bool requestUpdate = false;
    [HideInInspector] public bool isHitState = false;

    //private bool isDeathState = false;
    //public bool IsDeathState { get; set; }

    public override void InitState()
    {
        hp = 100;

        IsAttackState = false;
        IsActiveState = true;

        isHitState = false;
        isCombatMode = false;
        requestUpdate = false;
        target = null;

        handWeapon.SetActive(false);
        backWeapon.SetActive(true);

        _skillManager.cantUseSkill = false;

        SetTagLayer();

        stateFindDic.Clear();

        State_Base[] state_Entities = GetComponents<State_Base>();
              
        // 디폴트 스테이트로 Idle 지정
        defaultState = GetComponent<State_Idle>()._stateInfo.stateName;
        curState = GetComponent<State_Idle>()._stateInfo.stateName;

        if (GetComponent<State_Hit>())
        {
            hitState = GetComponent<State_Hit>()._stateInfo.stateName;
        }

        if (GetComponent<State_Death>())
        {
            deathState = GetComponent<State_Death>()._stateInfo.stateName;
        }

        if (GetComponent<State_Blocking>())
        {
            blockState = GetComponent<State_Blocking>()._stateInfo.stateName;
        }

        for (int count = 0; count < state_Entities.Length; count++)
        {
            if(state_Entities[count].isActiveAndEnabled)
            {
                state_Entities[count].InitStateVars();
                state_Entities[count].character = character;
                state_Entities[count]._stateController = this;
                stateFindDic.Add(state_Entities[count]._stateInfo.stateName, state_Entities[count]);
            }
        }

        _curStateEntity = stateFindDic[curState];
        _curStateEntity.OnEnterState();
    }

    public void DeleteCharacter()
    {
        if (curState != deathState)
        {
            nextState = deathState;
            UpdateState();
        }
    }

    public void UpdateState()
    {
        if (nextState != null)
        {
            _curStateEntity.OnExitState();

            //preState = curState;
            curState = nextState;
            nextState = null;

            _curStateEntity = stateFindDic[curState];

            _curStateEntity.OnEnterState();
        }
		else
		{
            // 만약 Next State가 없을때 현재 State가 Death State가 아니라면 자기자신(State)을 한번더 시작한다.
            // Death State라면 현재 State를 종료하고 끝난다.
			_curStateEntity.OnExitState();

            if (curState != deathState)
            {
                //curState = defaultState;
                //_curStateEntity = stateFindDic[curState];
                _curStateEntity.OnEnterState();
            }
		}
	}

    void CalcNextState()
    {
        // 가중치를 이용하여 다음 스테이트를 계산.
        int nextStateLength = stateFindDic[curState]._stateInfo.nextState.Length;
        float[] statePer = new float[nextStateLength];
        float prePer = 0f;
        float multPer;

        for (int i = 0; i < nextStateLength; i++)
        {
            multPer = 0f;

            if (stateFindDic.ContainsKey(_curStateEntity._stateInfo.nextState[i].stateName))
            {
                multPer = stateFindDic[_curStateEntity._stateInfo.nextState[i].stateName].GetMultPer();
            }

            // 만약 가중치가 원래의 가중치보다 심하게 높게 마스킹 돼있을 경우 강제 상태 업데이트(피격 혹은 특정조건 만족)
            if (multPer == 100f)
            {
                nextState = stateFindDic[_curStateEntity._stateInfo.nextState[i].stateName]._stateInfo.stateName;
                prePer = -1f;
                UpdateState();
                break;
            }
            else if(multPer == 0f)
            {
                statePer[i] = 0f;
            }
            else
            {
                statePer[i] = prePer + _curStateEntity._stateInfo.nextState[i].statePer * multPer;
				prePer = statePer[i];
			}            
        }

        if (prePer > 0f)
        {
            float random = UnityEngine.Random.Range(0f, prePer);

            for(int i=0; i<nextStateLength; i++)
            {
                if((statePer[i] > 0f) && (random <= statePer[i]))
                {                    
                    nextState = _curStateEntity._stateInfo.nextState[i].stateName;
                    //Debug.Log("Next State : " + nextState);
                    break;
                }
            }
        }
        else if((prePer == 0f) && (curState != deathState))   // 만약 가중치가 모두 0이라면 다음에 올 State가 없는것이므로 기본 State를 Next State로 지정
        {
            nextState = defaultState;
        }
    }

    public override void ReceiveDamage(int damage)
    {
        if (curState != deathState)
        {
            if ((blockState != null) && (curState == blockState))
            {
                isHitState = true;
                //_actionManager.Hit(Constants.HitAnim1);
                //StartCoroutine(CheckHitEnd("Blocking Hit"));

                _curStateEntity.receiveDamage = damage;
                _curStateEntity.OnUpdateState(Constants.BlockingHit);

            }
            else if (hitState != null)
            {
                hp -= damage;

                _actionManager.BloodEffect(0.4f);

                if ((hp <= 0) && (deathState != null))
                {
                    nextState = deathState;
                    UpdateState();
                }
                else
                {
                    for (int i = 0; i < stateFindDic[curState]._stateInfo.nextState.Length; i++)
                    {
                        if (stateFindDic.ContainsKey(_curStateEntity._stateInfo.nextState[i].stateName) &&
                            stateFindDic[_curStateEntity._stateInfo.nextState[i].stateName]._stateInfo.stateName == hitState)
                        {
                            isHitState = true;
                            nextState = hitState;
                            stateFindDic[hitState].receiveDamage = damage;
                            UpdateState();
                        }
                    }
                }
            }
        }
    }

    void Start () {

        InitState();
    }

    void Update () {

        // blocking 중 hit 상태 초기화

        //Debug.Log(curState);

        //if (force == Force.Force1)
        //{
        //    Debug.Log("target : " + target);
        //    Debug.Log("curState : " + curState);
        //    Debug.Log("nextState : " + nextState);
        //}

        if (target)
        {
            targetDistance = Vector3.Distance(character.transform.position, target.transform.position);

            if ((targetController && !targetController.IsActiveState)
                || (targetDistance > distanceLimit))
            {
                target = null;
                targetController = null;

                if (findTargetAfterKill && GetComponent<State_Find>().isActiveAndEnabled)
                {
                    GetComponent<State_Find>().FindTarget(360f);
                }

                //if (GetComponent<State_Find>())
                //{
                //    GetComponent<State_Find>().FindTarget();
                //}
            }
        }

        if (!isHitState && (curState != deathState))
        {
            CalcNextState();

            if (requestUpdate)
            {
                requestUpdate = false;
                UpdateState();
            }
            else
            {
                _curStateEntity.OnUpdateState();
            }
        }        



    }
    
    //private IEnumerator CheckHitEnd(string name)
    //{
    //    yield return StartCoroutine(WaitForAnimation(name, 0.9f));
    //    isHitState = false;
    //    anim.SetInteger("hit", Constants.HitStop);
    //}

    // Use this for initialization
}
