using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Skill_Missile : Skill_Base
{
    [Serializable]
    public class MissileInfo
    {
        public string animName;
        public int animParam;
        
        [Serializable]
        public class MissileData
        {
            public GameObject missile;
            public Transform missilePos;

            [Range(0f, 0.9f)] public float fireRatio = 0.4f;
            [Range(1f, 20f)] public float stopTime = 3f;
            [Range(0.1f, 3f)] public float explosionTime = 1f;
            [Range(1f, 50f)] public float speed = 25f;
            [Range(1, 100)] public int hitDamage = 20;
            [Range(1, 100)] public int rangeDamage = 30;
        }

        public MissileData[] _missileDatas;

        [Range(0f, 0.9f)] public float animEndRatio = 0.9f;
    }

    public MissileInfo _missileInfo;

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

        anim.SetInteger("skill", _missileInfo.animParam);
        checkAnimEnd = CheckAnimEnd(_missileInfo.animName, _missileInfo.animEndRatio);
        StartCoroutine(checkAnimEnd);
        StartCoroutine("CheckFireRatio");

    }

    public override void StopSkill()
    {
        StopCoroutine("CheckFireRatio");
        ExitSkillState();
    }

    private IEnumerator CheckFireRatio()
    {
        int i = 0;

        while (i < _missileInfo._missileDatas.Length)
        {
            yield return StartCoroutine(WaitForAnimation(_missileInfo.animName, _missileInfo._missileDatas[i].fireRatio));
            _missileInfo._missileDatas[i].missile.SetActive(true);
            _missileInfo._missileDatas[i].missile.transform.rotation = _missileInfo._missileDatas[i].missilePos.rotation;
            _missileInfo._missileDatas[i].missile.transform.position = _missileInfo._missileDatas[i].missilePos.position;
            _missileInfo._missileDatas[i].missile.GetComponent<Missile_Entity>().StartMissile(
                _missileInfo._missileDatas[i].speed,
                _missileInfo._missileDatas[i].stopTime,
                _missileInfo._missileDatas[i].explosionTime,
                _missileInfo._missileDatas[i].hitDamage,
                _missileInfo._missileDatas[i].rangeDamage,
                GetComponentInParent<Controller_Base>().tag);

            i++;
        }
    }
}
