using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Skill_Melee : Skill_Base
{		
    [Serializable]
    public class MeleeInfo
    {
        public string animName;
        public int animParam;
        public Collider atkCollider;

        [Serializable]
        public class AtkData
        {
            [Range(0f, 0.9f)] public float atkStartRatio = 0.2f;
            [Range(0f, 0.9f)] public float atkEndRatio = 0.7f;
            public int damage;
        }
        
        public AtkData[] _ratioDatas;
        
        [Range(0f, 0.9f)] public float animEndRatio = 0.9f;
    }
    
    public MeleeInfo _meleeInfo;
    
    public Collision_Manager.ColliderData colliderData = new Collision_Manager.ColliderData();

    //private Collider curCollider;
    private bool isAttacking = false;    

	// Use this for initialization
	void Start () {
        InitVals();
        isAtkSkill = true;
    }
	
	// Update is called once per frame
	void Update () {

        CheckCooltime();
	}

    public override void UseSkill()
    {
        EnterSkillState();

        anim.SetInteger("skill", _meleeInfo.animParam);
        checkAnimEnd = CheckAnimEnd(_meleeInfo.animName, _meleeInfo.animEndRatio);
        StartCoroutine(checkAnimEnd);
        StartCoroutine("CheckAtkRatio");
        StartCoroutine("CheckComboRatio");

    }

    public override void StopSkill()
    {
        StopCoroutine("CheckAtkRatio");
        StopCoroutine("CheckComboRatio");
        Collision_Manager.GetInstance().RemoveCollider(_meleeInfo.atkCollider);
        _meleeInfo.atkCollider.enabled = false;
        isAttacking = false;
        ExitSkillState();
    }

    private IEnumerator CheckAtkRatio()
    {
        int i = 0;

        while(i < _meleeInfo._ratioDatas.Length)
        {
            if (!isAttacking)
            {
                yield return StartCoroutine(WaitForAnimation(_meleeInfo.animName, _meleeInfo._ratioDatas[i].atkStartRatio));
                //curCollider = _meleeInfo.atkCollider;
                _meleeInfo.atkCollider.enabled = true;
                colliderData.damage = _meleeInfo._ratioDatas[i].damage;
                Collision_Manager.GetInstance().AddCollider(_meleeInfo.atkCollider, colliderData);
                isAttacking = true;
            }
            else
            {
                yield return StartCoroutine(WaitForAnimation(_meleeInfo.animName, _meleeInfo._ratioDatas[i].atkEndRatio));
                //_meleeInfo.atkCollider.enabled = true;
                Collision_Manager.GetInstance().RemoveCollider(_meleeInfo.atkCollider);
                _meleeInfo.atkCollider.enabled = false;
                isAttacking = false;
                i++;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private IEnumerator CheckComboRatio()
    {
        _skillManager.isComboEnable = false;

        while(true)
        {
            yield return StartCoroutine(WaitForAnimation(_meleeInfo.animName, 0.7f));
            break;
        }

        _skillManager.isComboEnable = true;
    }
}
