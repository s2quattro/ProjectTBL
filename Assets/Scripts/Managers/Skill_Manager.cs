using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

// 스킬과 관련된 정보들(쿨타임, 콤보 등)을 관리하는 스크립트
public class Skill_Manager : MonoBehaviour {
    
    public float curGlobalCool;
    public bool isSkillState = false;
    public bool cantUseSkill = false;
    public bool isComboEnable = false;
    public bool isBlockingState = false;

    public Constants.ComboType curComboState;

    public Skill_Base curSkill = null;
    
    // Update is called once per frame
    void Update()
    {
        CheckGlobalCooltime();
    }

    // 스킬의 콤보타입 등을 매개변수로 받아서 스킬을 사용할 수 있는지 결과를 반환하는 함수
    public bool CheckSkillEnable(bool effectOnGlobal, Constants.ComboType comboType)
    {
        bool canUseSkill = false;   // 스킬사용이 불가능하면 false값을 반환

        if (!cantUseSkill)
        {
            if (!isSkillState)
            {
                if (effectOnGlobal) // 글로벌 쿨타임이 있을때 스킬사용이 가능한지
                {
                    if (curGlobalCool > 0f)
                    {
                        canUseSkill = false;
                    }
                    else
                    {
                        canUseSkill = true;
                    }
                }
                else
                {
                    canUseSkill = true;
                }
            }
            else
            {
                if (isBlockingState)
                {
                    canUseSkill = true;
                }
                else if (curComboState == Constants.ComboType.Combo
                    && isComboEnable && !isBlockingState)
                {
                    if (comboType == Constants.ComboType.Combo
                        || comboType == Constants.ComboType.Finish)
                    {
                        canUseSkill = true;
                    }
                }
            }
        }

        return canUseSkill;
    }
    
    private void CheckGlobalCooltime()
    {
        if((!isSkillState || isBlockingState) && (curGlobalCool > 0f))
        {
            curGlobalCool -= Time.deltaTime;
        }
    }









 //   [Serializable]
	//public class SkillClass
	//{
	//	[HideInInspector]
	//	public Skill_Manager _skillManager;
		
	//	public void SetSkillManager(Skill_Manager script)
	//	{
	//		_skillManager = script;
	//	}

 //       public Constants.ComboType comboType;

 //       [ConditionalHide("comboType", (int)Constants.ComboType.Combo)]
 //       public float IncSpeed = 0.05f;

 //       public Constants.SkillType skillType;        
 //       public float cooltime = 0f;
 //       public float globalCooltime = 0f;
 //       public bool effectOnGlobal = true;

	//	// 이곳에다가 옮긴다. 그리고 ui의 아이콘을 이곳에다가 참조시킨다.
	//	public Sprite skillIcon;
	//	public Constants.KeyInput skillKey;
	//	//private Image skillImage;

	//	[HideInInspector]
 //       public float currentCooltime = 0f;

 //       [HideInInspector]
 //       public bool isCooldown = false;


 //       public void SetCooltime()
 //       {
 //           currentCooltime = cooltime;
 //       }

	//	public bool UpdateCooltime()
	//	{
	//		bool isCooltimeOver = false;

	//		// 쿨타임이 다 됐으면 

	//		return isCooltimeOver;
	//	}

	//	public bool TrySkill()
	//	{
	//		bool isUseSkill = false;

	//		// 스킬이 사용가능한지 검사. 콤보 유무 검사 등등

	//		switch(skillType)
	//		{
	//			case Constants.SkillType.Melee:
	//				{
	//					//meleeVariables.UseSkill();
	//					break;
	//				}
	//			default:
	//				{
	//					isUseSkill = false;
	//					break;
	//				}
	//		}

	//		return isUseSkill;
	//	}

 //       [ConditionalHide("skillType", (int)Constants.SkillType.Blocking)]
 //       public SkillBlocking blockingVariables;

 //       [ConditionalHide("skillType", (int)Constants.SkillType.Tumbling)]
 //       public SkillTumbling tumblingVariables;

 //       [ConditionalHide("skillType", (int)Constants.SkillType.Melee)]
 //       public SkillMelee meleeVariables;
        
 //       [ConditionalHide("skillType", (int)Constants.SkillType.Range)]
 //       public SkillRange rangeVariables;

 //       [ConditionalHide("skillType", (int)Constants.SkillType.Missile)]
 //       public SkillMissile missileVariables;

 //       [ConditionalHide("skillType", (int)Constants.SkillType.Buff)]
 //       public SkillBuff buffVariables;        
 //   }    

	//public class Skill_Entity
	//{

		
	//}


 //   [Serializable]    
 //   public class SkillBlocking
 //   {
 //       [Range(0f,1f)] public float decDamage = 0.8f;
 //       public int animParam = 1;
 //   }

 //   [Serializable]    
 //   public class SkillTumbling
 //   {
 //       public string animName = "Tumbling";
 //       public int animParam = 2;
 //       public float force = 500f;
 //   }

 //   [Serializable]
 //   public class SkillMelee : Skill_Entity
	//{
 //       public string animName;
 //       public int animParam;
 //       public Collider collider;
 //       public int damage;

	//	[Range(0f, 0.9f)] public float[] attackRatio;

	//	//public void UseSkill()
	//	//{
	//	//	_skillManager.isSkillState = true;
	//	//	// 이 함수를 통해서 곧바로 스킬을 사용할 수 있도록
	//	//	isSkillState = true;
	//	//	StartCoroutine(WaitForAnimation(name, 0.9f, isCombo));
	//	//	skillColl = coll;
	//	//	CollisionManager.GetInstance().AddCollider(skillColl, damage);

	//	//}

	//	//public void UseMeleeSkill(string name, int param, float ratio, bool isCombo, Collider coll, int damage)
	//	//{
	//	//	isSkillState = true;
	//	//	anim.SetInteger("skill", param);
	//	//	StartCoroutine(WaitForAnimation(name, 0.9f, isCombo));
	//	//	skillColl = coll;
	//	//	CollisionManager.GetInstance().AddCollider(skillColl, damage);
	//	//}
	//}

 //   [Serializable]
 //   public class SkillKick
 //   {
 //       public string animName;
 //       public int animParam;
 //       public Collider collider;
 //       public int damage;
 //   }

 //   [Serializable]
 //   public class SkillRange
 //   {
 //       public string animName;
 //       public int animParam;
 //       public Collider collider;
 //       public float range;
 //       public int damage;
 //   }

 //   [Serializable]
 //   public class SkillMissile
 //   {
 //       public string animName;
 //       public int animParam;
 //       public GameObject missile;
 //       public float range;
 //       public float speed;
 //       public int damage;
 //   }

 //   [Serializable]
 //   public class SkillBuff
 //   {
 //       public string animName;
 //       public int animParam;
 //   }
    

 //   CollisionManager _collisionManager;

 //   [HideInInspector] public Collider skillColl = null;

 //   private bool isBlocking = false;
 //   private bool comboEnable = false;

 //   private float comboRatio = 0.65f;
 //   private int comboNum = 1;
    


 //   public bool GetComboState()
 //   {
 //       return comboEnable;
 //   }

 //   public int GetComboNum()
 //   {
 //       return comboNum;
 //   }

 //   //public float GetAnimSpeed()
 //   //{
 //   //    return anim.speed;
 //   //}
    


	//private bool CheckSkill()
	//{
	//	bool canSkillUse = false;

	//	return canSkillUse;
	//}

	//public void UpdateCooltime()
	//{
	//	// player Controller에서 부르면 업데이트 되도록
	//	// 쿨타임이 다 되면 player controller에 메세지 보냄
	//	// 콤보 스테이트가 종료되면(싱글, 콤보스킬이 종료되면) 아이콘 업데이트 
	//}



    //public bool CheckSkillInfo(SkillClass skillInfo)
    //{       
    //    bool isUseSkill = false;
    //    bool isCombo = false;
    //    bool isComboSkill = false;

    //    if (skillInfo.comboType >= Constants.ComboType.Combo)
    //    {
    //        isCombo = true;
    //    }

    //    if (skillInfo.comboType == Constants.ComboType.Combo)
    //    {
    //        isComboSkill = true;
    //    }
    //    else
    //    {
    //        isComboSkill = false;
    //    }

    //    //if ((isCombo && comboEnable) || !isSkillState)
    //    //{
    //    if (!isSkillState)
    //    {
    //    if (isCombo && comboEnable)
    //        {
    //            comboNum++;

    //            if (isComboSkill)
    //            {
    //                anim.speed += skillInfo.IncSpeed;
    //            }
    //        }
            
    //        comboEnable = false;

    //        if ((skillInfo.skillType == Constants.SkillType.Blocking) && !isBlocking && (skillInfo.currentCooltime <= 0f))
    //        {
    //            isUseSkill = true;
    //            isSkillState = true;
    //            isBlocking = true;
    //            anim.SetInteger("skill", skillInfo.blockingVariables.animParam);
    //            anim.SetBool("block", true);
    //        }
    //        else if (skillInfo.skillType == Constants.SkillType.Tumbling)
    //        {
    //            isUseSkill = true;
    //            isSkillState = true;
    //            anim.SetInteger("skill", skillInfo.tumblingVariables.animParam);
    //            StartCoroutine(WaitForAnimation(skillInfo.tumblingVariables.animName, 0.9f, isComboSkill));
    //            rigidbody.AddRelativeForce(Vector3.forward * skillInfo.tumblingVariables.force, ForceMode.Impulse);
    //        }
    //        else if (skillInfo.skillType == Constants.SkillType.Melee)
    //        {
    //            isUseSkill = true;
    //            UseMeleeSkill(skillInfo.meleeVariables.animName, skillInfo.meleeVariables.animParam, 0.9f, isComboSkill, skillInfo.meleeVariables.collider, skillInfo.meleeVariables.damage);
    //        }
    //        else if (skillInfo.skillType == Constants.SkillType.Range)
    //        {
    //            isUseSkill = true;
    //            isSkillState = true;
    //            anim.SetInteger("skill", skillInfo.rangeVariables.animParam);
    //            StartCoroutine(WaitForAnimation(skillInfo.rangeVariables.animName, 0.9f, isComboSkill));
    //        }
    //        else if (skillInfo.skillType == Constants.SkillType.Missile)
    //        {
    //            isUseSkill = true;
    //            isSkillState = true;
    //            anim.SetInteger("skill", skillInfo.missileVariables.animParam);
    //            StartCoroutine(WaitForAnimation(skillInfo.missileVariables.animName, 0.9f, isComboSkill));
    //        }
    //        else if (skillInfo.skillType == Constants.SkillType.Buff)
    //        {
    //            isUseSkill = true;
    //            isSkillState = true;
    //            anim.SetInteger("skill", skillInfo.buffVariables.animParam);
    //            StartCoroutine(WaitForAnimation(skillInfo.buffVariables.animName, 0.9f, isComboSkill));
    //        }
    //    }
    //    else if (isSkillState)
    //    {
    //        if ((skillInfo.skillType == Constants.SkillType.Blocking) && isBlocking)
    //        {
    //            isBlocking = false;
    //            isUseSkill = true;
    //            skillInfo.currentCooltime = skillInfo.cooltime;
    //            anim.SetBool("block", false);
    //            SkillOver();
    //        }
    //    }

    //    return isUseSkill;
    //}



    //public void UseBlockingSkill(int param)
    //{
    //    if(isBlocking)
    //    {
    //        isBlocking = false;
    //        anim.SetInteger("skill", Constants.SkillOver);
    //        anim.SetBool("block", false);
    //    }
    //    else
    //    {
    //        isBlocking = true;
    //        anim.SetInteger("skill", param);
    //        anim.SetBool("block", true);
    //    }
    //}

    //IEnumerator WaitForAnimation(string name, float ratio, bool isCombo)
    //{
    //    CheckAnimInfo();

    //    bool isSkillStart = false;
    //    bool isComboState = false;
    //    bool isComboActive = false;

    //    int tmpParam = anim.GetInteger("skill");

    //    while ((animInfo.normalizedTime < ratio) || !animInfo.IsName(name))
    //    {
    //        CheckAnimInfo();

    //        if(isSkillStart)
    //        {     
    //            if (!isComboActive && isCombo && (animInfo.normalizedTime > comboRatio))
    //            {
    //                isComboActive = true;
    //                comboEnable = true;                   
    //            }

    //            if (tmpParam != anim.GetInteger("skill"))
    //            {
    //                // 다른스킬이 도중에 시작됐으므로 콤보스킬로 인식하고 코루틴 종료                        
    //                isComboState = true;
    //                break;
    //            }
    //        }
    //        else
    //        {
    //            if (animInfo.IsName(name))
    //            {
    //                isSkillStart = true;
    //            }
    //        }                        

    //        yield return new WaitForEndOfFrame();
    //    }
        
    //    if (!isComboState)
    //    {
    //        SkillOver();
    //    }        
    //}

    //void CheckAnimInfo()
    //{
    //    animInfo = anim.GetCurrentAnimatorStateInfo(0);
    //}

    //public void SkillOver()
    //{
    //    isSkillState = false;
    //    comboEnable = false;
    //    comboNum = 1;
    //    anim.speed = Constants.NormalSpeed;
    //    anim.SetInteger("skill", Constants.SkillOver);
    //    if (skillColl != null)
    //    {
    //        CollisionManager.GetInstance().RemoveCollider(skillColl);
    //        skillColl = null;
    //    }
    //}


}
