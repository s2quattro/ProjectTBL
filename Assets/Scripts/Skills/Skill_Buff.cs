using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Skill_Buff : Skill_Base {

    [Serializable]
    public class BuffInfo
    {
        public string animName;
        public int animParam;
        public GameObject effect;
        public AudioClip buffAudio;
        [Range(1f, 2f)] public float multSpeed = 1.2f;
        [Range(1f, 100f)] public float buffTime = 20f;
        [Range(0f, 0.9f)] public float buffStartRatio = 0.2f;
        [Range(0f, 0.9f)] public float animEndRatio = 0.9f;
    }

    public BuffInfo _buffInfo;

	// Use this for initialization
	void Start () {
        InitVals();
        isAtkSkill = false;
    }
	
	// Update is called once per frame
	void Update () {

        CheckCooltime();
    }

    public override void UseSkill()
    {
        EnterSkillState();

        anim.SetInteger("skill", _buffInfo.animParam);
        checkAnimEnd = CheckAnimEnd(_buffInfo.animName, _buffInfo.animEndRatio);
        StartCoroutine(checkAnimEnd);
        StopCoroutine("CheckBuffStartRatio");
        StartCoroutine("CheckBuffStartRatio");
    }

    public override void StopSkill()
    {
        ExitSkillState();
        //StopCoroutine("CheckBuffStartRatio");
        _buffInfo.effect.SetActive(false);
    }

    private IEnumerator CheckBuffStartRatio()
    {
        yield return StartCoroutine(WaitForAnimation(_buffInfo.animName, _buffInfo.buffStartRatio));
        _buffInfo.effect.SetActive(true);
        anim.speed = _buffInfo.multSpeed;
        GetComponent<Action_Manager>().SpeedMultValue = 1.2f;
        Audio_Manager.GetInstance().PlayAuio(_buffInfo.buffAudio);

        yield return new WaitForSeconds(_buffInfo.buffTime);
        anim.speed = Constants.NormalSpeed;
        GetComponent<Action_Manager>().SpeedMultValue = 1f;
    }
}
