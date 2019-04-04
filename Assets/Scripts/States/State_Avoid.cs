using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 타겟과의 거리를 일정수준으로 유지시키는 State
public class State_Avoid : State_Base
{
    [Range(1f, 100f)] public float ifTargetClose = 2f;  // 타겟이 가까워졌을때 가중치에 곱
    [Range(0f, 100f)] public float ifTargetNull = 0f;   // 타겟이 없을때 가중치에 곱
    [Range(1f, 100f)] public float maintainDistance = 3f;   // 타겟과 유지해야하는 거리
    [Range(0.5f, 100f)] public float maxGapDistance = 2f;   // 유지거리와의 최대 거리 차이

    private bool isDirRight;
    private bool isSideMove = false;

    public override float GetMultPer()
    {
        float multPer = 1f;

        if (curCooltime > 0f)
        {
            multPer = 0f;
        }
        else if ((_stateController.target == null)  // 타겟이 없거나 타겟과의 거리가 유지거리에서 특정차이만큼 벌어지면 가중치를 0으로 만든다.
            || (_stateController.targetDistance > (maintainDistance + maxGapDistance))  // 즉 너무 가깝거나 너무 멀면 State가 진행되지 않도록 한다.
            || (_stateController.targetDistance < (maintainDistance - maxGapDistance)))
        {
            multPer = ifTargetNull;
        }
        else
        {
            if (_stateController.targetDistance < maintainDistance) // 타겟과의 거리가 유지거리보다 가까울때 가중치를 증가시킨다.
            {
                multPer = ifTargetClose;
            }
        }

        return multPer;
    }

    public override void OnEnterState()
    {
        StateEnterInit();
        isSideMove = false; // 양옆으로 움직이는 상태인지를 결정하는 변수
        canStateOver = true;
    }

    public override void OnUpdateState()
    {
        CheckTime();

        LookRotation(); // target을 바라보도록 한다.

        if ((_stateController.targetDistance > (maintainDistance + maxGapDistance))
            || (_stateController.targetDistance < (maintainDistance - maxGapDistance)))
        {
            _stateController.requestUpdate = true;  // 너무 가깝거나 너무 멀면 다음 State를 갱신하도록 한다.
        }
        else if (!isSideMove && (Mathf.Abs(_stateController.targetDistance - maintainDistance) < 0.2f))
        {
            // 양옆으로 이동중이 아닐때 타겟과의 거리와 유지거리 차이의 절대값이 낮다면 옆으로 이동하는 상태가 된다.
            // 즉 가깝거나 멀어지게 거리를 조정하는 도중에 유지거리와 큰 차이가 없는 상태에서는 옆으로 이동하게 한다.
            isDirRight = true;

            if (Random.Range(0, 2) == 0)
            {
                isDirRight = false;
            }

            isSideMove = true;
        }
        else if(isSideMove && (Mathf.Abs(_stateController.targetDistance - maintainDistance) > 0.8f))
        {   
            // 반면 거리차이가 벌어지면 다시 target과의 거리유지를 위해 움직이기 시작한다.
            isSideMove = false;
        }
        else if(isSideMove)
        {
            if (isDirRight)
            {
                _stateController._actionManager.Move(Constants.MoveWalk, Constants.MoveRight, _stateController.moveSpeed * _stateController.moveSideMult * Time.deltaTime);
            }
            else
            {
                _stateController._actionManager.Move(Constants.MoveWalk, Constants.MoveLeft, -_stateController.moveSpeed * _stateController.moveSideMult * Time.deltaTime);
            }
        }
        else if(!isSideMove)
        {
            if (_stateController.targetDistance < maintainDistance)
            {
                _stateController._actionManager.Move(Constants.MoveWalk, Constants.MoveBack, -_stateController.moveSpeed * _stateController.moveBackMult * Time.deltaTime);
            }
            else
            {
                _stateController._actionManager.Move(Constants.MoveWalk, Constants.MoveFront, _stateController.moveSpeed * Time.deltaTime);
            }
        }
    }

    public override void OnExitState()
    {
        StateExitInit();
        
        _stateController._actionManager.Move(Constants.MoveStop, 0, 0f);
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
