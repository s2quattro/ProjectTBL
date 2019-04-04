using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(State_Combat))]
public class State_Chase : State_Base
{
	[Range(1f, 100f)] public float ifTargetFar = 3f;
    [Range(0f, 100f)] public float ifTargetNull = 0f;
    [Range(0f, 100f)] public float farDistance = 2f;
	[Range(0f, 100f)] public float runDistance = 4f;
    
	public override float GetMultPer()
	{
		float multPer = 1f;

        if (_stateInfo.cooltime > 0f)
        {
            multPer = 0f;
        }
        else if (_stateController.target == null)
		{
			multPer = ifTargetNull;
		}
		else if(_stateController.targetDistance > farDistance)
		{
			multPer = ifTargetFar;
		}

		return multPer;
	}

	public override void OnEnterState()
	{
		StateEnterInit();

		canStateOver = true;
	}

    public override void OnUpdateState()
    {
        CheckTime();

        if ((_stateController.target == null) || (_stateController.targetDistance < farDistance))
        {
            _stateController.requestUpdate = true;
        }
        else if (_stateController.targetDistance > farDistance)
        {
            LookRotation();

            if (_stateController.targetDistance > runDistance)
            {
                _stateController._actionManager.Move(Constants.MoveRun, Constants.MoveFront, _stateController.runSpeed * Time.deltaTime);
            }
            else
            {
                _stateController._actionManager.Move(Constants.MoveWalk, Constants.MoveFront, _stateController.runSpeed * Time.deltaTime);
            }
        }
    }

    public override void OnExitState()
	{
		StateExitInit();

		_stateController._actionManager.Move(Constants.MoveStop, 0, 0f);
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
        if (curCooltime > 0f)
        {
            curCooltime -= Time.deltaTime;
        }
    }
}
