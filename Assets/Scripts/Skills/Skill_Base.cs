using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 스킬 스크립트들의 부모클래스
public class Skill_Base : MonoBehaviour {

    protected Skill_Manager _skillManager;

    [Serializable]
    public class SkillInfo  // 스킬의 타입, 쿨타임, 글로벌 쿨타임이 있을때 스킬사용이 가능한지 유무등의 정보를 지정하는 클래스
    {
        public Constants.ComboType comboType;
        public float cooltime = 0f;
        public float globalCooltime = 0f;
        public bool effectOnGlobal = true;
    }

    public SkillInfo _skillInfo;

    protected Animator anim;
    protected AnimatorStateInfo animState;
    protected float curCooltime = 0f;
    protected bool isSkillState = false;
    protected bool isNoCooltime = true;

    protected IEnumerator checkAnimEnd;
    
    protected bool isAtkSkill = false;

    [NonSerialized]
    public bool isPlayer = true;
    
    public void InitVals()
    {
        _skillManager = GetComponent<Skill_Manager>();

        anim = GetComponent<Controller_Base>().character.GetComponent<Animator>();
    }
    
    // 스킬 사용을 시도할때 외부에서 델리게이트에 의해 호출되는 함수. 결과를 반환한다.
    public int TrySkill()
    {
        int isSkillUse = Constants.FailUseSkill;    // 스킬시도 결과를 반환하는 변수

        if (isNoCooltime && _skillManager.CheckSkillEnable(_skillInfo.effectOnGlobal, _skillInfo.comboType))
        {
            if(isAtkSkill)
            {
                isSkillUse = Constants.AtkSkill;
            }
            else
            {
                isSkillUse = Constants.NotAtkSkill;
            }

            if (isPlayer)
            {
                UI_Manager.GetInstance().CheckSkillIconDiable(_skillInfo.comboType);
            }
            UseSkill();
        }

        return isSkillUse;
    }

    public float GetCooltime()
    {
        return curCooltime / _skillInfo.cooltime;
    }

    public bool GetEffectOnGlobal()
    {
        return _skillInfo.effectOnGlobal;
    }

    public virtual void UseSkill()
    {
    }

    public virtual void StopSkill()
    {
    }

    // 쿨타임을 체크하고 감소시키는 함수
    protected void CheckCooltime()
    {
        if (!isNoCooltime && (!_skillManager.isSkillState || _skillManager.isBlockingState) && (curCooltime <= 0f))
        {
            isNoCooltime = true;
        }
        else if (!_skillManager.isSkillState && (curCooltime > 0f))
        {
            curCooltime -= Time.deltaTime;
        }
        else if(_skillManager.isSkillState && ((curCooltime / _skillInfo.cooltime) < 1f))
        {
            curCooltime -= Time.deltaTime;
        } 
    }

    // 스킬사용을 위해 진입할때 호출되는 함수
    protected void EnterSkillState()
    {
        if (_skillManager.curSkill != null)
        {
            _skillManager.curSkill.StopSkill();
        }

        isSkillState = true;
        curCooltime = _skillInfo.cooltime;
        isNoCooltime = false;
        _skillManager.isSkillState = true;
        _skillManager.curSkill = this;
        _skillManager.curComboState = _skillInfo.comboType;
    }
    
    // 스킬이 끝날 때나 스킬을 중지시켜야할 때 호출되는 함수
    protected void ExitSkillState()
    {
        if (checkAnimEnd != null)
        {
            StopCoroutine(checkAnimEnd);
        }

        if(isSkillState)
        {
            isSkillState = false;
            _skillManager.curGlobalCool += _skillInfo.globalCooltime;
        }

        anim.SetInteger("skill", Constants.SkillEnd);
        _skillManager.curSkill = null;
        _skillManager.isSkillState = false;
        _skillManager.isComboEnable = false;
        _skillManager.curComboState = Constants.ComboType.Single;
    }

    // 해당 이름의 애니메이션이 매개변수로 전달받은 비율(0f~1f)만큼 진행되면 종료되는 코루틴
    protected IEnumerator WaitForAnimation(string name, float ratio)
    {
        animState = anim.GetCurrentAnimatorStateInfo(0);

        while ((animState.normalizedTime < ratio) || !animState.IsName(name))
        {
            animState = anim.GetCurrentAnimatorStateInfo(0);

            yield return new WaitForEndOfFrame();
        }
    }

    // 애니메이션이 특정비율이 되면 스킬을 종료시키는 코루틴
    protected IEnumerator CheckAnimEnd(string name, float ratio)
    {
        while (true)
        {
            yield return StartCoroutine(WaitForAnimation(name, ratio));

            break;
        }

        StopSkill();
    }
}
