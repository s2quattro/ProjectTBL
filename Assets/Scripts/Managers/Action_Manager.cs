using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Action_Manager : MonoBehaviour
{
    public Animator anim;
    private Transform characterTrans;
    private Rigidbody characterRigid;

    private GameObject bloodEffect = null;
    private float deltaTime = 0f;
    private bool isBloodStart = false;

    private int animGetWalk = 0;
    private int animGetRun = 0;

    private bool isJump = false;
    private bool isInit = false;

    private float jumpStartDelay = 0f;

    private float speedMultValue = 1f;
    public float SpeedMultValue { get; set; }

    private void Awake()
    {
        // 캐릭터의 애니메이터, 위치정보, 리지디바디 등의 정보를 미리 셋팅
        anim = gameObject.GetComponent<Controller_Base>().character.GetComponent<Animator>();
        characterTrans = gameObject.GetComponent<Controller_Base>().character.GetComponent<Transform>().transform;
        characterRigid = gameObject.GetComponent<Controller_Base>().character.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // 점프상태라면 점프상태를 검사
        if (isJump)
        {
            CheckJump();
        }

        // 피 이펙트 오브젝트가 호출되었다면 사라지기까지 시간을 계산
        if(isBloodStart)
        {
            CheckBloodTime();
        }
    }

    public bool GetJumpState
    {
        get
        {
            return isJump;
        }
    }       

    // 캐릭터의 hit 관련으로 호출되어 애니메이션 파라메터 조정
    public void Hit(int hitState)
    {
        anim.SetInteger("hit", hitState);
        anim.SetTrigger("hit trigger");

        // hit 애니메이션 중지 요청시 애니메이션 스피드를 원래상태로 복구
        if(hitState == Constants.HitStop)
        {
            anim.speed = Constants.NormalSpeed * speedMultValue;
        }
    }
    
    public void Idle()
    {
        anim.SetTrigger("idle");
    }
    
    // 회전 관련으로 호출되어 애니메이션 파라메터 조정
    public void Turn(int turn)
    {
        if(turn == Constants.TurnRight)
        {
            anim.SetInteger("turn", Constants.TurnRight);
        }
        else if(turn == Constants.TurnLeft)
        {
            anim.SetInteger("turn", Constants.TurnLeft);
        }
        else if (turn == Constants.TurnStop)
        {
            anim.SetInteger("turn", Constants.TurnStop);
        }
    }

    // 캐릭터가 이동할 경우 멈추기, 걷기, 뛰기, 방향, 속도 등을 파라메터로 전달받아 호출됨
    public void Move(int action, int direction, float speed)
    {                      
        // 정지 요청이 아닐경우 현재 애니메이션 상태와 요청 상태를 참고하여 애니메이션 파라메터를 결정
        if (action != Constants.MoveStop)
        {
            animGetWalk = anim.GetInteger("walk");
            animGetRun = anim.GetInteger("run");           
            
            if(direction == Constants.MoveFront)
            {

                if ((action == Constants.MoveWalk) && (animGetWalk != Constants.MoveFront))
                {
                    anim.SetInteger("walk", Constants.MoveFront);
                    anim.SetInteger("run", Constants.MoveStop);
                }
                else if ((action == Constants.MoveRun) && (animGetRun != Constants.MoveFront))
                {
                    anim.SetInteger("run", Constants.MoveFront);
                    anim.SetInteger("walk", Constants.MoveStop);
                }

                characterTrans.Translate(0f, 0f, speed * speedMultValue);
            }
            else if (direction == Constants.MoveRight)
            {
                if ((action == Constants.MoveWalk) && (animGetWalk != Constants.MoveRight))
                {
                    anim.SetInteger("walk", Constants.MoveRight);
                    anim.SetInteger("run", Constants.MoveStop);
                }
                else if ((action == Constants.MoveRun) && (animGetRun != Constants.MoveRight))
                {
                    anim.SetInteger("run", Constants.MoveRight);
                    anim.SetInteger("walk", Constants.MoveStop);
                }

                characterTrans.Translate(speed * speedMultValue, 0f, 0f);
            }
            else if (direction == Constants.MoveLeft)
            {
                if ((action == Constants.MoveWalk) && (animGetWalk != Constants.MoveLeft))
                {
                    anim.SetInteger("walk", Constants.MoveLeft);
                    anim.SetInteger("run", Constants.MoveStop);
                }
                else if ((action == Constants.MoveRun) && (animGetRun != Constants.MoveLeft))
                {
                    anim.SetInteger("run", Constants.MoveLeft);
                    anim.SetInteger("walk", Constants.MoveStop);
                }

                characterTrans.Translate(speed * speedMultValue, 0f, 0f);
            }
            else if (direction == Constants.MoveBack)
            {
                if ((action == Constants.MoveWalk) && (animGetWalk != Constants.MoveBack))
                {
                    anim.SetInteger("walk", Constants.MoveBack);
                    anim.SetInteger("run", Constants.MoveStop);
                }
                else if ((action == Constants.MoveRun) && (animGetRun != Constants.MoveBack))
                {
                    anim.SetInteger("run", Constants.MoveBack);
                    anim.SetInteger("walk", Constants.MoveStop);
                }

                characterTrans.Translate(0f, 0f, speed * speedMultValue);
            }            
        }
        else if (action == Constants.MoveStop)
        {
            anim.SetInteger("walk", Constants.MoveStop);
            anim.SetInteger("run", Constants.MoveStop);
        }
    }

    //점프할 경우 점프 방향과 힘 등을 파라메터로 전달받아 호출됨
    public void Jump(int direction, float upForce, float dirForce)
    {
        // 점프상태가 아닐경우 점프상태로 진입
        if(!isJump)
        {
            isJump = true;

            characterRigid.AddRelativeForce(Vector3.up * upForce, ForceMode.Impulse);

            // 매개변수로 전달받은 방향정보에 따라 다른방향으로 캐릭터에게 힘을 전달
            if (direction == Constants.JumpVer)
            {
                characterRigid.AddRelativeForce(Vector3.forward * dirForce, ForceMode.Impulse);
            }            
            else if (direction == Constants.JumpHor)
            {
                characterRigid.AddRelativeForce(Vector3.right * dirForce, ForceMode.Impulse);
            }
        }        
        else
        {
            //점프상태일 경우 중지요청이 들어오면 점프를 끝냄
            if(direction == Constants.JumpEnd)
            {
                isJump = false;
                jumpStartDelay = 0f;
                anim.SetFloat("jump", Constants.JumpEnd);
            }
        }
    }

    // 점프상태를 체크하고 애니메이션 파라메터를 갱신
    private void CheckJump()
    {
        float distance = CheckJumpDistance();   // 캐릭터와 지면 사이의 위치를 계산
        anim.SetFloat("jump", Mathf.Lerp(0f, 1f, distance * Constants.JumpDistValue));        
                        
        if(((jumpStartDelay >= 0.1f) && (distance < 0.1f)) || (jumpStartDelay >= 3f))
        {
            isJump = false;
            isInit = false;
            jumpStartDelay = 0f;
        }

        if (jumpStartDelay < 3f)
        {
            jumpStartDelay += Time.deltaTime;            
        }

        if (!isInit && (jumpStartDelay > 0.1f))
        {
            isInit = true;
            anim.SetInteger("walk", Constants.MoveStop);
            anim.SetInteger("run", Constants.MoveStop);
        }
    }

    // 캐릭터와 지면 사이의 거리를 계산
    private float CheckJumpDistance()
    {
        float distance = 0f;
        RaycastHit hit;        

        // 레이캐스트를 아래방향으로 쏴서 맞을 경우 맞은 위치와 캐릭터의 위치 사이의 거리를 계산하여 반환
        if (Physics.Raycast(characterTrans.position - new Vector3(0, 0.1f, 0), -Vector3.up, out hit))
        {
            distance = characterTrans.position.y - hit.transform.position.y;
        }

        return distance;
    }

    public void JumpToIdle()
    {
        anim.SetFloat("jump", Constants.JumpEnd);
    }

    public void CombatMode()
    {
        anim.SetBool("combat", true);
        anim.SetTrigger("change");
    }

    public void DefaultMode()
    {
        anim.SetBool("combat", false);
        anim.SetTrigger("change");
    }

    public void Idle(bool value)
    {
        anim.SetBool("idle", value);
    }

    public void Death(int num)
    {
        anim.SetInteger("death", num);
    }

    // 피효과를 위해 호출되는 함수. 피효과가 지속되는 시간을 파라메터로 전달받음
    public void BloodEffect(float time)
    {
        // 피효과가 진행중일 경우 중지한다.
        if(isBloodStart)
        {
            StopBloodEffect();
        }
        
        // 피효과가 진행중이지 않고 피효과 이펙트 오브젝트가 호출된 상태가 아닐때(null)
        if(!isBloodStart && !bloodEffect)
        {
            // 오브젝트풀 매니저를 통해 피효과 파티클 오브젝트를 하나 할당받음
            bloodEffect = ObjectPool_Manager.GetInstance().GetItem("Effect/BloodSplat");
            bloodEffect.SetActive(false);
            bloodEffect.transform.position = characterTrans.transform.position + Vector3.up;
            bloodEffect.SetActive(true);

            deltaTime = time;

            isBloodStart = true;
        }

        //StartCoroutine(CheckEffectEnd(time));
    }

    // 피효과 이펙트가 호출되어 사라지기까지 진행중인 시간을 체크
    private void CheckBloodTime()
    {
        if (deltaTime > 0f)
        {
            deltaTime -= Time.deltaTime;
        }
        else
        {
            deltaTime = 0f;
            StopBloodEffect();  // 시간이 0이되면 피효과 중지
        }
    }

    // 피효과를 중지할 때 호출되는 함수
    public void StopBloodEffect()
    {
        // 피효과 오브젝트가 할당되어 null상태가 아닐때
        if (bloodEffect)
        {
            // 오브젝트풀 매니저를 통해 피효과 오브젝트를 반환하고 피효과 오브젝트를 null상태로 만든다.
            bloodEffect.SetActive(false);
            ObjectPool_Manager.GetInstance().ReleaseItem("Effect/BloodSplat", bloodEffect);
            bloodEffect = null;
            isBloodStart = false;
        }
    }
    
    //private IEnumerator CheckEffectEnd(float time)
    //{
    //    GameObject bloodEffect = ObjectPool_Manager.GetInstance().GetItem("Effect/BloodSplat");
    //    bloodEffect.SetActive(false);
    //    bloodEffect.transform.position = characterTrans.transform.position + Vector3.up;
    //    bloodEffect.SetActive(true);
    //    yield return new WaitForSeconds(time);
    //    ObjectPool_Manager.GetInstance().ReleaseItem("Effect/BloodSplat", bloodEffect);
    //}
}
