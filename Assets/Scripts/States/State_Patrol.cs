using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Patrol : State_Base {
    
    [Range(0f, 100f)] public float ifTargetExist = 0f;
    [Range(0f, 90f)] public float minRotateAngle = 30f;
    [Range(0f, 180f)] public float maxRotateAngle = 180f;

    float rotateAngle;
    float rotateTime;
    bool isRotate = false;
    bool isMove = false;

    // 타겟이 없을 경우 가중치 증가
    public override float GetMultPer()
    {
        float multPer = 1f;

        if (curCooltime > 0f)
        {
            multPer = 0f;
        }
        else if (_stateController.target != null)
        {
            multPer = ifTargetExist;
        }

        return multPer;
    }

    public override void OnEnterState()
    {
		StateEnterInit();

		isRotate = false;
        isMove = false;		

        rotateAngle = Random.Range(-minRotateAngle, maxRotateAngle);
        
        if (Random.Range(0, 2) == 0)
        {            
            rotateAngle = -rotateAngle;
        }

        rotateTime = Mathf.Abs(rotateAngle / 180f);
    }

    public override void OnUpdateState()
    {
        CheckTime();

        if (_stateController.target == null)
        {
            if (!isRotate)
            {
                isRotate = true;

                if (rotateAngle > 0f)
                {
                    _stateController._actionManager.Turn(Constants.TurnRight);
                }
                else
                {
                    _stateController._actionManager.Turn(Constants.TurnLeft);
                }
            }
            else
            {
                if (!isMove)
                {
                    if (rotateTime > 0f)
                    {
                        character.transform.Rotate(0f, Mathf.Lerp(0f, rotateAngle, Time.deltaTime), 0f);
                        rotateTime -= Time.deltaTime;
                    }
                    else
                    {
                        isMove = true;
                        _stateController._actionManager.Turn(Constants.TurnStop);
                    }
                }
                else
                {
                    canStateOver = true;
                    _stateController._actionManager.Move(Constants.MoveWalk, Constants.MoveFront, _stateController.moveSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            _stateController.requestUpdate = true;
        }
    }

    public override void OnExitState()
    {
		StateExitInit();

		_stateController._actionManager.Turn(Constants.TurnStop);
        _stateController._actionManager.Move(Constants.MoveStop, 0, 0f);
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update() {
        if (curCooltime > 0f)
        {
            curCooltime -= Time.deltaTime;
        }
    }
}
