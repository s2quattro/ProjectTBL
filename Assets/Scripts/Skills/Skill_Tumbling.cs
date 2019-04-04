using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Skill_Tumbling : Skill_Base {

    [Serializable]
    public class TumblingInfo
    {
        public string animName;
        public int animParam;
        [Range(300f, 800f)] public float tumblingForce = 500f;
    }

    public TumblingInfo _tumblingInfo;

    // Use this for initialization
    void Start()
    {
        InitVals();
        isAtkSkill = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCooltime();
    }

    public override void UseSkill()
    {
        EnterSkillState();

        anim.SetInteger("skill", _tumblingInfo.animParam);
        GetComponent<Controller_Base>().character.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * _tumblingInfo.tumblingForce, ForceMode.Impulse);
        checkAnimEnd = CheckAnimEnd(_tumblingInfo.animName, 0.9f);
        StartCoroutine(checkAnimEnd);
    }

    public override void StopSkill()
    {
        ExitSkillState();
    }
}
