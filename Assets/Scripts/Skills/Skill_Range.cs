using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Skill_Range : Skill_Base
{
    [Serializable]
    public class RangeInfo
    {
        public string animName;
        public int animParam;
        public Collider atkCollider;
        public GameObject effect;
        public AudioClip effectAudio;
        [Range(0f, 0.9f)] public float atkStartRatio = 0.5f;
        [Range(0f, 0.9f)] public float atkEndRatio = 0.8f;
        [Range(0f, 0.9f)] public float animEndRatio = 0.9f;
        [Range(0f, 200f)] public int damage = 50;
    }

    public RangeInfo _rangeInfo;

    public Collision_Manager.ColliderData colliderData = new Collision_Manager.ColliderData();

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

        anim.SetInteger("skill", _rangeInfo.animParam);
        checkAnimEnd = CheckAnimEnd(_rangeInfo.animName, _rangeInfo.animEndRatio);
        StartCoroutine(checkAnimEnd);
        StartCoroutine("CheckAtkStartRatio");
        StartCoroutine("CheckAtkEndRatio");
    }

    public override void StopSkill()
    {
        Collision_Manager.GetInstance().RemoveCollider(_rangeInfo.atkCollider);
        ExitSkillState();
        StopCoroutine("CheckAtkStartRatio");
        StopCoroutine("CheckAtkEndRatio");
        _rangeInfo.atkCollider.gameObject.SetActive(false);
        _rangeInfo.effect.SetActive(false);
    }

    private IEnumerator CheckAtkStartRatio()
    {
        yield return StartCoroutine(WaitForAnimation(_rangeInfo.animName, _rangeInfo.atkStartRatio));

        _rangeInfo.atkCollider.gameObject.SetActive(true);
        _rangeInfo.effect.SetActive(true);
        colliderData.damage = _rangeInfo.damage;
        Collision_Manager.GetInstance().AddCollider(_rangeInfo.atkCollider, colliderData);
        Audio_Manager.GetInstance().PlayAuio(_rangeInfo.effectAudio);
        //_rangeInfo.effect.GetComponent<ParticleSystem>().Play();
    }

    private IEnumerator CheckAtkEndRatio()
    {
        yield return StartCoroutine(WaitForAnimation(_rangeInfo.animName, _rangeInfo.atkEndRatio));
        Collision_Manager.GetInstance().RemoveCollider(_rangeInfo.atkCollider);
        _rangeInfo.atkCollider.gameObject.SetActive(false);
    }
}