using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Move : State_Base {

    public string targetName;
    private GameObject target;

    [Range(1f, 10f)] public float farDistance = 5f;
    [Range(0f, 1f)] public float ifDistanceClose = 0f;
    [Range(1f, 100f)] public float ifDistanceFar = 99f;

    private float cross;
    private bool isTurnState = true;

    public override void InitStateVars()
    {
        target = GameObject.Find(targetName);
    }

    public override float GetMultPer()
    {
        float multPer = 1f;
        
        if (target == null)
        {
            multPer = 0f;
        }
        else
        {
            if (Vector3.Distance(transform.position, target.transform.position) < farDistance)
            {
                multPer = ifDistanceClose;
            }
            else
            {
                multPer = ifDistanceFar;
            }
        }        

        return multPer;
    }

    public override void OnEnterState()
    {
        StateEnterInit();

        canStateOver = true;

        isTurnState = true;
    }

    public override void OnUpdateState()
    {
        CheckTime();

        if(Vector3.Distance(transform.position, target.transform.position) < farDistance)
        {
            _stateController.requestUpdate = true;
        }
        else
        {
            if (isTurnState)
            {
                LookRotation(target.transform.position);

                cross = character.transform.forward.z * dir.x - character.transform.forward.x * dir.z;

                if (cross > 0.1f)
                {
                    _stateController._actionManager.Turn(Constants.TurnRight);
                }
                else if (cross < -0.1f)
                {
                    _stateController._actionManager.Turn(Constants.TurnLeft);
                }
                else
                {
                    isTurnState = false;
                }
            }
            else
            {
                _stateController._actionManager.Turn(Constants.TurnStop);
                _stateController._actionManager.Move(Constants.MoveWalk, Constants.MoveFront, _stateController.moveSpeed * Time.deltaTime);
            }
        }
    }

    public override void OnExitState()
    {
        StateExitInit();

        _stateController._actionManager.Turn(Constants.TurnStop);
        _stateController._actionManager.Move(Constants.MoveStop, Constants.MoveStop, 0f);
    }

    // Use this for initialization
    private void Start()
    {

    }
	// Update is called once per frame
	void Update () {
		
	}
}
