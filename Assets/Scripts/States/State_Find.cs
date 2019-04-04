using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Find : State_Base {

    [Range(0f, 100f)] public float ifTargetExist = 0f;
    [Range(1f, 100f)] public float ifTargetNull = 2f;
    [Range(1f, 100f)] public float ifTargetOnView = 100f;

    [Range(1f, 1000f)] public float viewDistance = 15f;
    [Range(30f, 360f)] public float viewAngle = 120f;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    //public int TargetMask
    //{
    //    get;
    //    set;
    //}

    // 타겟이 없을 경우 가중치 증가
    public override float GetMultPer()
    {
        float multPer = 1f;

        if (curCooltime > 0f)
        {
            multPer = 0f;
        }
        else if (_stateController.target)
        {
            multPer = ifTargetExist;
        }
        else
        {
            if (FindTarget())
            {
                multPer = ifTargetOnView;
            }
        }

        return multPer;
    }

    public override void OnEnterState()
    {
		StateEnterInit();
		
        _stateController.anim.SetBool("find", true);

		canStateOver = true;
		StartCoroutine(WaitForAnimation(_stateInfo.stateName, 0.5f));
	}

    public override void OnUpdateState()
    {
        CheckTime();

        if (_stateController.target)
        {
            _stateController.requestUpdate = true;
            
        }
        else
        {
            if (FindTarget(viewAngle * (3f / 2f)))
            {
                _stateController.requestUpdate = true;
            }
        }

        if (isAnimOver)
        {
            isAnimOver = false;

            _stateController.anim.SetBool("find", false);
        }
    }

    public override void OnExitState()
    {
		StateExitInit(); 

		_stateController.anim.SetBool("find", false);		
	}

    public bool FindTarget(float angle)
    {
        //GameObject targetObject = null;
        Collider target = null;
        Collider[] targets = Physics.OverlapSphere(character.transform.position, viewDistance, targetMask);
        Vector3 dirToTarget;
        float distToTarget;
        float maxDistToTarget = viewDistance;

        bool isTargetFind = false;

        for (int i = 0; i < targets.Length; i++)
        {
            target = targets[i];
            dirToTarget = (target.transform.position - character.transform.position).normalized;

            //_transform.forward와 dirToTarget은 모두 단위벡터이므로 내적값은 두 벡터가 이루는 각의 Cos값과 같다.

            //내적값이 시야각/2의 Cos값보다 크면 시야에 들어온 것이다.
            if (Vector3.Dot(character.transform.forward, dirToTarget) > Mathf.Cos((angle / 2) * Mathf.Deg2Rad))
            {
                distToTarget = Vector3.Distance(character.transform.position, target.transform.position);

                if (!Physics.Raycast(character.transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    if (distToTarget <= maxDistToTarget)
                    {
                        maxDistToTarget = distToTarget;
                        if (target.GetComponentInParent<Controller_Base>()
                            && target.GetComponentInParent<Controller_Base>().IsActiveState)
                        {
                            _stateController.target = target.gameObject;
                            _stateController.targetController = target.gameObject.GetComponentInParent<Controller_Base>();
                            //targetObject = target.gameObject;
                            isTargetFind = true;
                        }
                    }
                }
            }           
        }

        //if (isTargetFind)
        //{

        //}

        return isTargetFind;
        //return targetObject;
    }

    public bool FindTarget()
    {
        //GameObject targetObject = null;
        Collider target = null;
        Collider[] targets = Physics.OverlapSphere(character.transform.position, viewDistance, targetMask);
        Vector3 dirToTarget;
        float distToTarget;
        float maxDistToTarget = viewDistance;

        bool isTargetFind = false;

        for (int i = 0; i < targets.Length; i++)
        {
            target = targets[i];
            dirToTarget = (target.transform.position - character.transform.position).normalized;

            //_transform.forward와 dirToTarget은 모두 단위벡터이므로 내적값은 두 벡터가 이루는 각의 Cos값과 같다.

            //내적값이 시야각/2의 Cos값보다 크면 시야에 들어온 것이다.
            if (Vector3.Dot(character.transform.forward, dirToTarget) > Mathf.Cos((viewAngle / 2) * Mathf.Deg2Rad))
            {
                distToTarget = Vector3.Distance(character.transform.position, target.transform.position);

                if (!Physics.Raycast(character.transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    if (distToTarget <= maxDistToTarget)
                    {
                        maxDistToTarget = distToTarget;
                        if (target.GetComponentInParent<Controller_Base>()
                            && target.GetComponentInParent<Controller_Base>().IsActiveState)
                        {
                            _stateController.target = target.gameObject;
                            _stateController.targetController = target.gameObject.GetComponentInParent<Controller_Base>();
                            //targetObject = target.gameObject;
                            isTargetFind = true;
                        }
                    }
                }
            }
        }

        //if (isTargetFind)
        //{

        //}

        return isTargetFind;
        //return targetObject;
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (curCooltime > 0f)
        {
            curCooltime -= Time.deltaTime;
        }
    }
}
