using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(State_Combat))]
public class Skill_Blocking : Skill_Base
{
    [Serializable]
    public class BlockingInfo
    {
        public int animParam;
    }

    public BlockingInfo _blockingInfo;

    private bool isBlockingState = false;

	// Use this for initialization
	void Start () {
        InitVals();
        isAtkSkill = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!isBlockingState)
        {
            CheckCooltime();
        }
    }
        
    public override void UseSkill()
    {
        if ((_skillManager.curSkill != null) && (_skillManager.curSkill != this))
        {
            _skillManager.curSkill.StopSkill();
        }

        if (isBlockingState)
        {
            StopSkill();
        }
        else
        {
            isBlockingState = true;
            _skillManager.isSkillState = true;
            _skillManager.curSkill = this;
            _skillManager.curComboState = _skillInfo.comboType;
            _skillManager.isBlockingState = true;
            anim.SetInteger("skill", _blockingInfo.animParam);
        }
    }

    public override void StopSkill()
    {
        isBlockingState = false;
        isNoCooltime = false;
        curCooltime = _skillInfo.cooltime;
        _skillManager.curGlobalCool += _skillInfo.globalCooltime;
        _skillManager.curSkill = null;
        _skillManager.curComboState = Constants.ComboType.Single;
        _skillManager.isSkillState = false;
        _skillManager.isComboEnable = false;
        _skillManager.isBlockingState = false;
        anim.SetInteger("skill", Constants.SkillEnd);
    }
}

