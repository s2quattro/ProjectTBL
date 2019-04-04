using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스테이트 매니저 스크립트 사용시 Default State로 Idle 반드시 생성
[RequireComponent (typeof (State_Controller))]
public class State_Idle : State_Base
{
    [Range(1f, 100f)] public float ifTargetNull = 2f;
    [Range(0f, 100f)] public float ifTargetExist = 0f;
    public bool canModeChange = true;

    // 타겟이 없을 경우 가중치 증가
    public override float GetMultPer()
    {
        float multPer = 1f;

        if(curCooltime > 0f)
        {
            multPer = 0f;
        }
        else if(_stateController.target == null)
        {
            multPer = ifTargetNull;
        }
        else
        {
            multPer = ifTargetExist;
        }

        return multPer;
    }

    public override void OnEnterState()
    {
		StateEnterInit();

		if (_stateController.isCombatMode)
		{
			_stateController.anim.SetBool("combat", false);
			_stateController.anim.SetTrigger("change");
			StartCoroutine(WaitForAnimation("Disarm", 1f));
		}
		else
		{
			_stateController.anim.SetBool("idle", true);
			canStateOver = true;
		}		
    }

    public override void OnUpdateState()
    {
        CheckTime();

        if (isAnimOver)
        {
            isAnimOver = false;

            _stateController.handWeapon.SetActive(false);
            _stateController.backWeapon.SetActive(true);
            _stateController.anim.SetBool("idle", true);
            canStateOver = true;
            _stateController.isCombatMode = false;
        }
    }

    public override void OnExitState()
    {
		StateExitInit();

		_stateController.anim.SetBool("idle", false);
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update () {
        if (curCooltime > 0f)
        {
            curCooltime -= Time.deltaTime;
        }
    }
}
