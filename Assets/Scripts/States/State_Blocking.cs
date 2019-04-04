using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Blocking 스킬 State
public class State_Blocking : State_Base {
    
    [Range(0f, 1f)] public float ifTargetNull = 0f; // 타겟이 없을때 가중치 곱
    [Range(1f, 100f)] public float ifTargetAttack = 5f; // 타겟이 공격상태일때 가중치 곱
    [Range(0f, 1f)] public float ifTargetFar = 0.5f;    // 타겟이 멀 때 가중치 곱
    [Range(1f, 10f)] public float FarDistance = 3f; // 타겟의 먼 거리 기준치

    public Skill_Blocking.BlockingInfo _blockingInfo;
    private Skill_Base.SkillInfo _skillInfo;
    private Skill_Blocking _skill;
    private bool isTargetAttacking = false;

    public override float GetMultPer()
    {
        float multPer = 1f;

        if (curCooltime > 0f)
        {
            multPer = 0f;
        }
        else if (_stateController.target == null)
        {
            multPer = ifTargetNull;
        }
        else if((_stateController.target != null)
            && _stateController.targetController
           && _stateController.targetController.IsAttackState)
        {
            multPer = ifTargetAttack;
        }
        else if(_stateController.targetDistance > FarDistance)
        {
            multPer = FarDistance;
        }

        return multPer;
    }

    public override void OnEnterState()
    {
        StateEnterInit();

        _skill.TrySkill();

        if (_stateController.targetController
           && _stateController.targetController.IsAttackState)
        {
            isTargetAttacking = true;
        }
        else
        {
            canStateOver = true;
        }


    }

    public override void OnUpdateState()
    {
        CheckTime();

        LookRotation();
		
        if(isTargetAttacking
            && _stateController.targetController
            && !_stateController.targetController.IsAttackState)
        {
            isTargetAttacking = false;
            canStateOver = true;
        }

		// 가드시에 상대가 공격중이면 공격이 끝날 때까지 가드를 유지하다가 공격상태가 null이 되면 해제 가능하도록 추가할 것
    }

    public override void OnUpdateState(int message)
    {
        if (message == Constants.BlockingHit)
        {
            //_stateController._actionManager.Move(Constants.MoveStop, 0, 0);
            //_stateController._actionManager.anim.SetInteger("skill", Constants.SkillEnd);

            // 히트 리커버리와 받은 데미지에 따라 피격 애니메이션 속도가 달라짐
            _stateController.anim.speed = 0.6f + ((_stateController.hitRecovery / (float)receiveDamage) * 0.4f);
            
            _stateController._actionManager.Hit(Constants.BlockingHit);

            StartCoroutine(CheckHitEnd("Blocking Hit"));


            //StartCoroutine(CheckHitEnd("Blocking Hit"));

        }
    }

    public override void OnExitState()
    {
        StateExitInit();
        _skill.TrySkill();
        isTargetAttacking = false;
    }

    // Use this for initialization
    void Start () {
        _skillInfo = new Skill_Base.SkillInfo();

        _skill = gameObject.AddComponent<Skill_Blocking>();

        _skillInfo.comboType = Constants.ComboType.Blocking;
        _skillInfo.cooltime = 0f;
        _skillInfo.effectOnGlobal = false;
        _skillInfo.globalCooltime = 0f;

        _skill._skillInfo = _skillInfo;
        _skill._blockingInfo = _blockingInfo;
        _skill.isPlayer = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (curCooltime > 0f)
        {
            curCooltime -= Time.deltaTime;
        }
    }

    private IEnumerator CheckHitEnd(string name)
    {       
        yield return StartCoroutine(WaitForAnimation(name, 0.9f));

        _stateController.isHitState = false;
        _stateController.anim.speed = Constants.NormalSpeed;
        _stateController.anim.SetInteger("hit", Constants.HitStop);
    }
}
