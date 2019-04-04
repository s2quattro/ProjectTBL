using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Death : State_Base
{

    [Range(0f, 60f)] public float keepTime = 5f;

    private float deltaTime;

    public override float GetMultPer()
    {
        float multPer = 0f;

        return multPer;
    }

    public override void OnEnterState()
    {
        StateEnterInit();

        if (Random.Range(1, 3) == 1)
        {
            _stateController._actionManager.Death(Constants.DeathFront);
            StartCoroutine(CheckDeathEnd("Death Front"));
        }
        else
        {
            _stateController._actionManager.Death(Constants.DeathBack);
            StartCoroutine(CheckDeathEnd("Death Back"));
        }

        _stateController.IsActiveState = false;
        //_stateController._actionManager.BloodEffect(0.4f);
    }

    public override void OnUpdateState()
    {
        //if (deltaTime > 0f)
        //{
        //    deltaTime -= Time.deltaTime;
        //}
    }

    public override void OnExitState()
    {
        StateExitInit();
        _stateController._actionManager.Death(Constants.DeathEnd);
        _stateController._actionManager.StopBloodEffect();

        //_stateController._actionManager.Idle();


        ObjectPool_Manager.GetInstance().ReleaseItem(GetComponent<Controller_Base>().resourcePath, gameObject);

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator CheckDeathEnd(string name)
    {
        yield return StartCoroutine(WaitForAnimation(name, 0.9f));
        //_stateController._actionManager.StopBloodEffect();
               
        deltaTime = keepTime;

        while (true)
        {
            if(deltaTime > 0f)
            {
                deltaTime -= Time.deltaTime;
                yield return null;
            }
            else
            {
                deltaTime = 0f;
                //_stateController._actionManager.StopBloodEffect();
                //OnExitState();
                break;
            }

            //yield return new WaitForSeconds(keepTime);
            //canStateOver = true;

            //_stateController.requestUpdate = true;
        }

        //canStateOver = true;
        //_stateController.requestUpdate = true;
        _stateController.UpdateState();

        //Debug.Log("!!");
        //OnExitState();
    }
}
