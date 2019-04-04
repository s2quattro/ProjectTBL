using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(State_Find))]
public class State_Combat : State_Base {

	[Range(0f, 1f)] public float ifTargetNull = 0f;
	[Range(1f, 100f)] public float ifTargetExist = 100;
    public bool canModeChange = true;
    public bool findNewTarget = true;

    private bool isIdleMode;
    private float cross;


    public override float GetMultPer()
	{
		float multPer = 1f;

        if (curCooltime > 0f)
        {
            multPer = 0f;
        }
        else if ((_stateController.target != null) && !_stateController.isCombatMode)
		{
			multPer = ifTargetExist;
		}
		else if(_stateController.target == null)
		{
			multPer = ifTargetNull;
		}

		return multPer;
	}
    
    public override void OnEnterState()
    {
		StateEnterInit();

		if((_stateController.target != null) && (!_stateController.isCombatMode))
		{
			_stateController.anim.SetBool("combat", true);
			_stateController.anim.SetTrigger("change");
			StartCoroutine(WaitForAnimation("Equip", 1f));
			_stateController.isCombatMode = true;

			_stateController.handWeapon.SetActive(true);
			_stateController.backWeapon.SetActive(false);

            isIdleMode = false;
		}
		else
		{
            isIdleMode = true;
            _stateController.anim.SetBool("idle", true);
			canStateOver = true;
		}

        if (findNewTarget && _stateController.GetComponent<State_Find>())
        {
            _stateController.GetComponent<State_Find>().FindTarget();
            //if(newTarget)
            //{
            //    _stateController.target = newTarget;
            //}
        }
    }

    public override void OnUpdateState()
    {
        CheckTime();

        if (isIdleMode)
        {
            LookRotation();

            cross = character.transform.forward.z * dir.x - character.transform.forward.x * dir.z;
            
            if (cross > 0.2f)
            {
                _stateController._actionManager.Idle(Constants.IdleStop);
                _stateController._actionManager.Turn(Constants.TurnRight);
            }
            else if (cross < -0.2f)
            {
                _stateController._actionManager.Idle(Constants.IdleStop);
                _stateController._actionManager.Turn(Constants.TurnLeft);
            }
            else if ((cross > -0.1f) && (cross < 0.1f))
            {
                _stateController._actionManager.Idle(Constants.IdleStop);
                _stateController._actionManager.Turn(Constants.TurnStop);
            }
        }

        if (isAnimOver)
        {
            isAnimOver = false;
            _stateController._actionManager.Idle(Constants.Idle);
            canStateOver = true;
            isIdleMode = true;
        }
    }

    public override void OnExitState()
    {
		StateExitInit();

        _stateController._actionManager.Idle(Constants.IdleStop);
        _stateController._actionManager.Turn(Constants.TurnStop);
    }	

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        
        if (curCooltime > 0f)
        {
            curCooltime -= Time.deltaTime;
        }
    }
}
