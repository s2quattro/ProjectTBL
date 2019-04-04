using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

    public AI_State[] stateList;
    private AI_State preState;
    private AI_State curState;
    private AI_State nextState;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        calcNextState();
        if (curState != nextState && nextState != null)
        {
            updateState();
        }
        else
        {
            curState.onUpdateState();
        }
	}

    public void updateState()
    {
        if(curState)
        {
            curState.onExitState();
        }

        preState = curState;
        curState = nextState;
        nextState = null;

        curState.onEnterState();
    }

    public void calcNextState()
    {
        // 가중치 등을 이용해서 다음 스테이트를 정한다.

        curState.nextState;
        curState.nextStatePer;
    }
}
