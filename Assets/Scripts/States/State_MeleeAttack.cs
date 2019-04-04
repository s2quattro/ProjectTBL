using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(State_Combat))]
public class State_MeleeAttack : State_Base
{
    [Range(0f, 100f)] public float ifTargetNull = 0f;
    [Range(0f, 100f)] public float ifTargetClose = 2f;
    [Range(0f, 5f)] public float maxDistance;
    [Range(0f, 5f)] public float multDistance;
    public bool canRoate = false;

    public Skill_Melee.MeleeInfo _meleeInfo;
    private Skill_Base.SkillInfo _skillInfo;
    private Skill_Melee _skill;    

    public override float GetMultPer()
    {
        float multPer = 1f;

        if (curCooltime > 0f)
        {
            multPer = 0f;
        }
        else if ((_stateController.target == null)
            || (_stateController.targetDistance > (maxDistance)))
        {
            multPer = ifTargetNull;
        }
        else if (_stateController.targetDistance < (multDistance))
        {
            multPer = ifTargetClose;
        }

        return multPer;
    }

    public override void OnEnterState()
    {
        StateEnterInit();

        _stateController.IsAttackState = Constants.AtkState;

        _skill.TrySkill();
    }

    public override void OnUpdateState()
    {
        CheckTime();

        if(!_stateController._skillManager.isSkillState)
        {
            canStateOver = true;
        }

        if (canRoate)
        {
            if (_stateController.target)
            {
                dir = (_stateController.target.transform.position - character.transform.position).normalized;
                lookRotate = Quaternion.LookRotation(dir);
                character.transform.rotation = Quaternion.Slerp(character.transform.rotation, lookRotate, _stateController.rotateSpeed * Time.deltaTime);
            }
        }
    }

    public override void OnExitState()
    {
        StateExitInit();
        _skill.StopSkill();
        _stateController.IsAttackState = Constants.NotAtkState;
    }

    void Start () {

        _skillInfo = new Skill_Base.SkillInfo();

        _skill = gameObject.AddComponent<Skill_Melee>();
        //_skill = new Skill_Melee();

        _skillInfo.comboType = Constants.ComboType.Single;
        _skillInfo.cooltime = 0f;
        _skillInfo.effectOnGlobal = false;
        _skillInfo.globalCooltime = 0f;
        
        _skill._skillInfo = _skillInfo;
        _skill._meleeInfo = _meleeInfo;
        _skill.isPlayer = false;
    }
	
	void Update () {

        if (curCooltime > 0f)
        {
            curCooltime -= Time.deltaTime;
        }
    }
}
