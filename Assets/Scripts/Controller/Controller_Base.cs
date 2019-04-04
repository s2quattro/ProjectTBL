using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 컨트롤러와, AI의 스테이트 컨트롤러의 부모 클래스
public class Controller_Base : MonoBehaviour {

    public enum Force
    {
        Force1 = 8, Force2, Force3, Force4, Force5,
    }
    public Force force;

    public GameObject character;
    public GameObject handWeapon;
    public GameObject backWeapon;
    public GameObject[] otherweapon;

    public int hp;
    [Range(0, 100)] public float hitRecovery = Constants.DefaultHitRecoveryValue;

    public string resourcePath;

    private bool isAttackState = false;
    public bool IsAttackState { get; set; }
    private bool isActiveState = true;
    public bool IsActiveState { get; set; }
    
    [HideInInspector] public Animator anim;
    [HideInInspector] public Action_Manager _actionManager;
    [HideInInspector] public Skill_Manager _skillManager;
        
    private void Awake()
    {
        anim = character.GetComponent<Animator>();
        _actionManager = gameObject.AddComponent<Action_Manager>();
        _skillManager = gameObject.AddComponent<Skill_Manager>();
    }

    public virtual void InitState() { }

    // 캐릭터를 생성할 때 캐릭터의 팀 구별을 위해 tag와 layer을 수정하기위해 호출되는 함수. 캐릭터가 해당되는 팀을 매개변수로 받는다.
    public void SetTagLayer(Force tagForce)
    {
        force = tagForce;   // 캐릭터의 팀을 설정

        GetComponent<State_Find>().targetMask.value = 0;    // layer 정보를 수정하기위해 기본값을 0으로 설정

        // 팀 1~5까지 루프를 돌며 해당되는 팀을 제외하고 나머지 팀을 레이어 값에 더해 추가시킨다.
        for (int i=Constants.LayerForce1; i<=Constants.LayerForce5; i++)
        {
            if(i != (int)force)
            {
                GetComponent<State_Find>().targetMask.value += (1 << i);    // layer 값은 2의 n제곱만큼 시프트한 값을 더해서 추가할 수 있다.
            }
        }

        SetTagLayer();
    }

    // 실질적으로 캐릭터의 태그 및 레이어를 설정하는 함수
    public void SetTagLayer()
    {
        character.layer = (int)force;
        
        switch (force)
        {
            case Force.Force1:
                {
                    gameObject.tag = "Force 1";
                    handWeapon.tag = "Force 1";

                    for(int i=0; i<otherweapon.Length; i++)
                    {
                        otherweapon[i].tag = "Force 1";
                    }
                    break;
                }
            case Force.Force2:
                {
                    gameObject.tag = "Force 2";
                    handWeapon.tag = "Force 2";

                    for (int i = 0; i < otherweapon.Length; i++)
                    {
                        otherweapon[i].tag = "Force 2";
                    }
                    break;
                }
            case Force.Force3:
                {
                    gameObject.tag = "Force 3";
                    handWeapon.tag = "Force 3";

                    for (int i = 0; i < otherweapon.Length; i++)
                    {
                        otherweapon[i].tag = "Force 3";
                    }
                    break;
                }
            case Force.Force4:
                {
                    gameObject.tag = "Force 4";
                    handWeapon.tag = "Force 4";

                    for (int i = 0; i < otherweapon.Length; i++)
                    {
                        otherweapon[i].tag = "Force 4";
                    }
                    break;
                }
            case Force.Force5:
                {
                    gameObject.tag = "Force 5";
                    handWeapon.tag = "Force 5";

                    for (int i = 0; i < otherweapon.Length; i++)
                    {
                        otherweapon[i].tag = "Force 5";
                    }
                    break;
                }
        }
    }

    // 충돌했을 경우 유효한 충돌이면 충돌 매니저가 호출하는 함수
    public virtual void ReceiveDamage(int damage)
    {
    }

    // 해당 애니메이션의 이름과 진행 비율을 매개변수로 입력받아 해당 비율만큼 애니메이션이 완료되면 종료되는 코루틴
    protected IEnumerator WaitForAnimation(string name, float ratio)
    {
        AnimatorStateInfo animInfo = _actionManager.anim.GetCurrentAnimatorStateInfo(0);

        while ((animInfo.normalizedTime < ratio) || !animInfo.IsName(name))
        {
            animInfo = _actionManager.anim.GetCurrentAnimatorStateInfo(0);

            yield return new WaitForEndOfFrame();
        }
    }

    //public void SetTagLayer(Force tagForce, int enemyLayer)
    //{
    //    force = tagForce;
    //    GetComponent<State_Find>().targetMask.value = (1 << enemyLayer);

    //    SetTagLayer();
    //}
}
