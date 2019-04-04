using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(State_Death))]
public class State_Hit : State_Base {

    //[Range(0, 100)] public int hitRecovery = 10;
    public bool findTargetAfterHit = true;

    //private GameObject blood;

    public override void OnEnterState()
    {
        StateEnterInit();

        _stateController._actionManager.Move(Constants.MoveStop, 0, 0);
        _stateController._actionManager.anim.SetInteger("skill", Constants.SkillEnd);

        if (Random.Range(0, 2) == 0)
        {
            _stateController._actionManager.Hit(Constants.HitAnim1);
            //_stateManager.anim.SetInteger("hit", Constants.HitAnim1);
            StartCoroutine(CheckHitEnd("Hit Left"));
        }
        else
        {
            _stateController._actionManager.Hit(Constants.HitAnim2);
            //_stateManager.anim.SetInteger("hit", Constants.HitAnim2);
            StartCoroutine(CheckHitEnd("Hit Right"));
        }

        // 히트 리커버리와 받은 데미지에 따라 피격 애니메이션 속도가 달라짐
        _stateController.anim.speed = 0.6f + ((_stateController.hitRecovery / (float)receiveDamage) *0.4f);
        
        if (findTargetAfterHit && _stateController.GetComponent<State_Find>().isActiveAndEnabled)
        {
            _stateController.GetComponent<State_Find>().FindTarget(360f);
        }

        //_stateController._actionManager.BloodEffect(0.4f);

        //blood = ObjectPool_Manager.GetInstance().GetItem("Effect/BloodSplat");
        //blood.transform.position = character.transform.position + Vector3.up;
        //StartCoroutine(CheckEffectEnd(0.4f));

    }

    public override void OnUpdateState()
    {

    }

    public override void OnExitState()
    {
        StateExitInit();

        _stateController.anim.speed = Constants.NormalSpeed;
        _stateController._actionManager.Hit(Constants.HitStop);
        //_stateManager.anim.SetInteger("hit", Constants.HitStop);

        //StopCoroutine("CheckEffectEnd");

        //if (blood != null)
        //{
        //    blood.SetActive(true);
        //    ObjectPool_Manager.GetInstance().ReleaseItem("Effect/BloodSplat", blood);
        //}
    }

    protected IEnumerator CheckHitEnd(string name)
    {
        yield return StartCoroutine(WaitForAnimation(name, 0.9f));
        isAnimOver = false;
        _stateController.isHitState = false;
        _stateController.requestUpdate = true;
        _stateController.anim.speed = Constants.NormalSpeed;
        _stateController._actionManager.Hit(Constants.HitStop);
    }

    //private IEnumerator CheckEffectEnd(float time)
    //{
    //    yield return new WaitForSeconds(time);

    //    if (blood != null)
    //    {
    //        blood.SetActive(false);
    //    }
    //}

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update () {

    }
}
