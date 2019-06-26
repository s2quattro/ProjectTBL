
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// 플레이어 캐릭터의 기본적인 설정들, 그리고 인풋과 관련된 것들을 처리하는 컨트롤러 스크립트
public class Player_Controller : Controller_Base
{   
    private Constants.CurrentState currentState;
	
    [Serializable]
    public class StateClass // 인스펙터 조정 가능한 상태 관련 변수 클래스
    {
        public bool canTurn = true;
        public bool canRun = true;
        public bool canJump = true;
        public bool canUseSkills = true;

        public enum ModeSelect
        {
            OnlyDefault = 1,
            OnlyCombat,
            Both,
        }
        public ModeSelect modeSelect = ModeSelect.Both;
    }    

    [Serializable]
    public class CharacterClass // 인스펙터 조정 가능한 캐릭터 관련 변수 클래스
    {
        [HideInInspector] public float turnSensitivity = 1f;
        [Range(0.5f, 4f)] public float moveSpeed = 1.5f;
        [Range(0.1f, 1f)] public float backSpeed = 0.5f;
        [Range(0.1f, 1f)] public float sideSpeed = 0.8f;
        [Range(1f,5f)] public float runSpeedMult = 2.5f;
        
        [HideInInspector] public float jumpUpForce = 400f;
        [Range(50f, 500f)] public float jumpFrontForce = 200f;
        [Range(40f, 400f)] public float jumpSideForce = 150f;
        [Range(25f, 250f)] public float jumpBackForce = 100f;
        [Range(0f, 1f)] public float alterSensitivity = 0.3f;
        [Range(0.1f, 5f)] public float jumpDelay = 0.5f;

        //[Range(1f, 100f)] public float hitRecovery = 10f;
    }    
    
    [Serializable]
    public class CameraClass    // 인스펙터 조정 가능한 카메라 관련 변수 클래스
    {
        public GameObject playerCamera;
        [Range(0f, 90f)] public float degreeLimitY = 20f;
        public Vector2 cameraSmoothing = new Vector2(5f, 5f);
        public Vector2 cameraSensitivity = new Vector2(1.5f, 1.5f);

        [HideInInspector] public Vector2 cameraDregrees;
        [HideInInspector] public Vector2 mouseInput;
        [HideInInspector] public Vector2 mouseSmooth = new Vector2(0f, 0f);
        [HideInInspector] public Vector2 mouseAbsolute;
    }
	        
    public StateClass _state;
    public CharacterClass _character;
    public CameraClass _camera;

    [HideInInspector] public bool isAttack = false;

    private bool isCombatMode = false;
    private bool isTurn = false;
    private bool canCameraMove = true;
    private bool isRotate = false;
    private bool isVertical = false;
    private bool isGlobalCheck = true;
    private bool isComboTextOn = false;

    private float hor;
    private float ver;
    private float horRaw;
    private float verRaw;
    private float rotateAngle = 0f;
    private float rotateSpeed = 3.5f;
    private int keyValue;
    private int direction;

    private float jumpDelay = 0f;
    private float changeDelay;
    private float globalCooltime = 0f;
    private float comboTextDelay = 0f;
	
    KeyCode[] keyCodes;

    [HideInInspector]
    public delegate int SkillUseDeligate();
    private delegate void SkillStopDeligate();
    public SkillUseDeligate[] skillUseDels;
    private SkillStopDeligate[] skillStopDels;

    private bool[] skillEnable = new bool[12];

    public override void InitState()
    {
        IsActiveState = true;
    }

    // Use this for initialization
    void Start()
    {
        InitState();    // 상태 초기화

        SetTagLayer();  // 태그 및 레이어 설정

        Skill_Base[] skills = GetComponents<Skill_Base>();  // 스킬 관련 컴포넌트 스크립트들을 모두 가져온다.

        skillUseDels = new SkillUseDeligate[skills.Length]; // 스킬 컴포넌트 스크립트들의 개수만큼 스킬 사용 함수 델리게이트 배열을 할당
        skillStopDels = new SkillStopDeligate[skills.Length];   // 마찮가지로 스킬 중지 함수 델리게이트 할당

        // 전체 스킬 목록중 사용가능한(설정돼있는) 스킬이 몇개인지 알아내기 위해 초기에 설정
        for(int i=0; i<skillEnable.Length; i++)
        {
            skillEnable[i] = false;
        }

        // 각각의 스킬 컴포넌트 클래스들에 접근하여 스킬 사용 및 중지 델리게이트 배열들에 함수를 대입시킨다.
        // EX) 스킬 1,2,3,4,5가 존재한다면 델리게이트[5]가 할당되고 각각의 델리게이트가 각 스킬의 사용 및 중지 함수를 가지고있다.
        for (int i=0; i<skills.Length; i++)
        {
            if (skills[i].isActiveAndEnabled)
            {
                skillEnable[i] = true;  // 전체 스킬목록중 현재 스킬목록에 스킬이 존재한다
                skillUseDels[i] = skills[i].TrySkill;   // 델리게이트와 연결된 TrySkill() 함수를 통해 스킬을 사용을 시도하게 된다.
                if (skills[i]._skillInfo.comboType != Constants.ComboType.Blocking)
                {
                    skillStopDels[i] = skills[i].StopSkill; // 스킬을 중지하는 함수도 마찮가지로 델리게이트에 대입한다.
                }
            }
        }

        UI_Manager.GetInstance().SetSkillInfo(this, _skillManager, skills);  // UI 매니저에서 스킬과 관련된 것들(예를들어 쿨타임, 스킬 아이콘 등)을 참조하게 하기 위해 스킬 관련 정보들을 보내 설정시킨다.
        
        // 스킬을 사용할 때 눌러야할 기본 키들의 목록을 설정해 놓는다.
        keyCodes = new KeyCode[10];
        keyCodes[0] = KeyCode.Alpha1;
        keyCodes[1] = KeyCode.Alpha2;
        keyCodes[2] = KeyCode.Alpha3;
        keyCodes[3] = KeyCode.Alpha4;
        keyCodes[4] = KeyCode.Alpha5;
        keyCodes[5] = KeyCode.Alpha6;
        keyCodes[6] = KeyCode.Q;
        keyCodes[7] = KeyCode.E;
        keyCodes[8] = KeyCode.R;
        keyCodes[9] = KeyCode.T;
        
        // 전투상태만 가능한 캐릭터라면 처음부터 전투모드가 되도록 설정
        if (_state.modeSelect == StateClass.ModeSelect.OnlyCombat)
        {
            isCombatMode = true;
            _actionManager.CombatMode();
            _skillManager.cantUseSkill = false;
        }
        else
        {
            _skillManager.cantUseSkill = true; // 그렇지 않다면 기본모드로 시작하므로 모드 변경 전까지는 스킬을 사용할 수 없다.
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // 카메라 이동이 가능한 상태일때 카메라 관련 함수를 작동시킨다.
		if (canCameraMove)
        {
            CameraController(); // 실질적으로 카메라를 이동시키는 등 카메라와 관련된 모든 기능이 포함돼있는 함수
        }

        // 현재 State 상태에 따라 다양한 상태를 체크
        switch (currentState)
        {
            case Constants.CurrentState.Idle:
                {
                    // Idle 상태에서 좌우 회전할때 턴하는 애니메이션을 취할 수 있도록 설정한 캐릭터라면 체크하여 턴시킨다.
                    if (_state.canTurn) 
                    {
                        CheckTurn();
                    }

                    // 전투, 비전투 모드 변환이 가능할 때 모드 변환을 체크한다.
                    if (_state.modeSelect == StateClass.ModeSelect.Both && !isTurn)
                    {
                        CheckModeChange();
                    }

                    break;
                }
            case Constants.CurrentState.Skill:
                {
                    CheckSkillEnd();   // 스킬 사용중인 상태일때에는 스킬이 끝났는지를 체크한다.
                    break;
                }
            default:
                {
                    break;
                }
        }

        // 캐릭터가 이동중인 상태 이하일 때(Idle 상태 등) 움직임 관련 인풋 및 상태를 체크
        if(currentState <= Constants.CurrentState.Move)
        {
            CheckMove();
        }

        if (_state.canJump) // 점프가 가능한 캐릭터일때
        {
            // 점프 상태 이하일 때 점프 관련 인풋과 상태를 체크한다.
            if (currentState <= Constants.CurrentState.Jump)
            {
                CheckJump();
            }

            // 점프 상태가 아니라면 점프 재시도까지의 딜레이를 처리
            if (currentState != Constants.CurrentState.Jump)
            {
                if(jumpDelay > 0f)
                {
                    jumpDelay -= Time.deltaTime;
                }
            }
        }        
        
        // 전투모드에서 스킬이 사용가능한 상태이면
        if (isCombatMode && (_state.canUseSkills == true))
        {         
            // 캐릭터가 움직이는 상태 이하이거나 스킬을 사용중일때 스킬과 관련된 인풋을 처리한다.
            if ((currentState <= Constants.CurrentState.Move) || (currentState == Constants.CurrentState.Skill))
            {
                CheckSkillInput();
            }            
        }
    }

    // 캐릭터가 데미지를 입었을때(Hit 되었을때) 충돌 매니저로부터 함수가 호출되어 처리된다.
    public override void ReceiveDamage(int damage)
    {
        // 입은 데미지와 히트 리커버리 스탯에 따라 애니메이션 속도가 조절된다. (히트 리커버리 스탯과 같은 데미지를 입으면 1의 속도가 된다)
        anim.speed = 0.6f + ((hitRecovery / (float)damage) * 0.4f);

        // 블록킹(방어스킬) 상태일때는 데미지를 입지 않도록 설정해놓은 상태
        if (_skillManager.isBlockingState)
        {
            _actionManager.Hit(Constants.HitAnim1);
            StartCoroutine(CheckHitEnd("Blocking Hit"));
        }
        else
        {
            IsAttackState = false;  // 캐릭터가 공격중인 상태인지 참조하기 위한 변수이다. 피격상태일땐 공격이 취소되기 때문에 false로 처리
            currentState = Constants.CurrentState.Hit;  // hit state가 된다.
            _skillManager.cantUseSkill = true;  // 스킬 사용 불가능

            // 이동, 회전, 점프 등을 취소시킨다.
            _actionManager.Move(Constants.MoveStop, 0, 0f);
            _actionManager.Turn(Constants.TurnStop);
            _actionManager.Jump(Constants.JumpEnd, 0f, 0f);

            // 모든 스킬들의 사용중지 함수를 deligate를 통해 바로 호출한다.
            for (int i = 0; i < skillStopDels.Length; i++)
            {
                if (skillStopDels[i] != null)
                {
                    skillStopDels[i]();
                }
            }
            
            // 랜덤 난수로 애니메이션 설정
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                _actionManager.Hit(Constants.HitAnim1);
                StartCoroutine(CheckHitEnd("Hit 1"));

            }
            else
            {
                _actionManager.Hit(Constants.HitAnim2);
                StartCoroutine(CheckHitEnd("Hit 2"));
            }

            _actionManager.BloodEffect(0.4f);   // 피 이펙트 호출 함수
        }

        anim.speed = Constants.NormalSpeed;
    }

    // 애니메이션 이름을 통해 해당 Hit 애니메이션 종료 후에 설정을 초기화하는 코루틴
    private IEnumerator CheckHitEnd(string name)
    {
        // Blocking(방어스킬) 사용유무에따라 다르게 처리
        if(_skillManager.isBlockingState)
        {
            yield return StartCoroutine(WaitForAnimation(name, 0.1f));  // 해당 애니메이션의 비율(0f~1f)까지 기다린 후 종료되는 코루틴
            _actionManager.Hit(Constants.HitStop);
        }   
        else
        {
            // 애니메이션이 거의 끝까지 재생될 때까지 기다렸다가 그 후에 관련 변수들을 초기화
            yield return StartCoroutine(WaitForAnimation(name, 0.9f));
            _actionManager.Hit(Constants.HitStop);
            currentState = Constants.CurrentState.Idle;
            canCameraMove = true;

            if (isCombatMode)
            {
                _skillManager.cantUseSkill = false;
            }
        }
    }

    // 인풋에 따른 카메라의 이동 등을 처리하는 함수
    private void CameraController()
    {
        // Clamp 함수를 통해 카메라 최대 상하각도 조절
        _camera.mouseAbsolute.y = Mathf.Clamp(_camera.mouseAbsolute.y, -_camera.degreeLimitY, _camera.degreeLimitY);

        // 마우스 인풋 저장
        _camera.mouseInput.x = Input.GetAxisRaw("Mouse X");
        _camera.mouseInput.y = Input.GetAxisRaw("Mouse Y");

        // 민감도와 부드러움에 따라 스케일한다. 점프 상태면 같은 인풋일때 더 둔하게 움직이도록 설정
        if (currentState == Constants.CurrentState.Jump)
        {
            _camera.mouseInput = Vector2.Scale(_camera.mouseInput, new Vector2(
            _camera.cameraSensitivity.x * _character.alterSensitivity,
            _camera.cameraSensitivity.y * _character.alterSensitivity));
        }
        else
        {
            _camera.mouseInput = Vector2.Scale(_camera.mouseInput, new Vector2(
            _camera.cameraSensitivity.x,
            _camera.cameraSensitivity.y));
        }

        // 인풋을 보간하여 부드러운 움직임을 구현
        _camera.mouseSmooth.x = Mathf.Lerp(_camera.mouseSmooth.x, _camera.mouseInput.x, 1f / _camera.cameraSmoothing.x);
        _camera.mouseSmooth.y = Mathf.Lerp(_camera.mouseSmooth.y, _camera.mouseInput.y, 1f / _camera.cameraSmoothing.y);

        _camera.mouseAbsolute += _camera.mouseSmooth;   // 최종적으로 보간된 변수를 더해 그만큼 카메라를 움직인다.

        // 카메라의 x방향 이동에 따라 캐릭터도 회전시키고, y방향 이동은 캐릭터가 아닌 카메라만 이동시킨다.
        character.transform.localRotation = Quaternion.AngleAxis(_camera.mouseAbsolute.x + rotateAngle, Vector3.up);
        _camera.playerCamera.transform.localRotation = Quaternion.Euler(new Vector3(-_camera.mouseAbsolute.y, -rotateAngle, 0.0f));
    }

    // 입력 관련 전역변수에 입력값을 받아오는 함수 
    private void GetAxsis()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
    }

    // 제자리에서 카메라의 좌우 이동에 따라 캐릭터가 회전하는 것을 체크하고 구현하는 함수
    private void CheckTurn()
    {
        if (isTurn)
        {
            // 턴 상태일때 턴 민감도보다 마우스 회전 입력값의 절대값이 낮다면 회전을 중지시킨다. 
            if (isTurn && (Mathf.Abs(_camera.mouseSmooth.x) < _character.turnSensitivity))
            {
                _actionManager.Turn(Constants.TurnStop);
                isTurn = false;
            }
        }
        else
        {
            // 반대로 턴 상태가 아닐 때에는 민감도보다 마우스 회전 입력값이 높다면 캐릭터를 회전시킨다.
            if (_camera.mouseSmooth.x > _character.turnSensitivity)
            {
                _actionManager.Turn(Constants.TurnRight);
                isTurn = true;
            }
            else if (_camera.mouseSmooth.x < -_character.turnSensitivity)
            {
                _actionManager.Turn(Constants.TurnLeft);
                isTurn = true;
            }
        }
    }

    // 캐릭터의 이동과 관련하여 인풋값과 움직임을 처리하고 그 결과를 반환하는 함수
    private bool CheckMove()
    {
        bool isMove = false;    // 반환할 변수값 설정

        keyValue = KeyInputCheck(); // 키 입력에 따라 움직임 상태를 받아온다. (정지, 걷기, 달리기 등)

        verRaw = Input.GetAxisRaw("Vertical");
        horRaw = Input.GetAxisRaw("Horizontal");

        if (!isRotate)
        {
            // 캐릭터가 사선방향으로 이동하여 회전하고 있는 상황이 아니라면 회전한 상태에서 원래 상태까지 보간하여 회전상태를 복구시킨다.
            // EX) W,A 키를 동시에 눌러서 좌측 상단으로 캐릭터가 이동하며 회전했다가 사선이 아닌 방향으로 이동하면 다시 원래 상태로 복구되는 경우
            rotateAngle = Mathf.Lerp(rotateAngle, 0, rotateSpeed * Time.deltaTime);
        }

        if ((hor != 0) || (ver != 0))
        {
            isMove = true;  // 수평, 수직값이 존재하면 이동중인 것으로 처리

            if((horRaw == 0f) || (verRaw == 0f))
            {
                isRotate = false;   // 수평 또는 수직 인풋값이 둘중 하나라도 0이라면 사선이동(회전하며 이동)이 아니기 때문에 false로 처리

                // 수직이동인지 수평이동인지에 따라 방향을 설정한다.
                if(horRaw == 0f)
                {     
                    isVertical = true;

                    if (ver > 0f) // 앞
                    {
                        direction = Constants.MoveFront;
                        ver *= _character.moveSpeed * Time.deltaTime;
                    }
                    else if (ver < 0f)    // 뒤
                    {
                        direction = Constants.MoveBack;
                        ver *= _character.moveSpeed * _character.backSpeed * Time.deltaTime;                        
                    }
                }
                else if(verRaw == 0f)
                {
                    hor *= _character.moveSpeed * _character.sideSpeed * Time.deltaTime;
                    isVertical = false;

                    if (hor > 0f)    // 오른쪽
                    {
                        direction = Constants.MoveRight;
                    }
                    else if (hor < 0f)    // 왼쪽
                    {
                        direction = Constants.MoveLeft;                       
                    }
                }                
            }
            else   // 사선방향으로 이동중일때의 처리
            {
                isRotate = true;
                isVertical = true;                
                
                if(ver > 0f)
                {
                    direction = Constants.MoveFront;
                    ver *= _character.moveSpeed * Time.deltaTime;

                    if (hor > 0f)  // 우측상단
                    {
                        rotateAngle = Mathf.Lerp(rotateAngle, 40f, rotateSpeed * Time.deltaTime);
                    }
                    else if (hor < 0f)  // 좌측상단
                    {
                        rotateAngle = Mathf.Lerp(rotateAngle, -40f, rotateSpeed * Time.deltaTime);                        
                    }
                }
                else if (ver < 0f)
                {
                    direction = Constants.MoveBack;
                    ver *= _character.moveSpeed * _character.backSpeed * Time.deltaTime;

                    if (hor > 0f)  // 우측하단
                    {
                        rotateAngle = Mathf.Lerp(rotateAngle, -40f, rotateSpeed * Time.deltaTime);
                    }
                    else if (hor < 0f)  // 좌측하단
                    {
                        rotateAngle = Mathf.Lerp(rotateAngle, 40f, rotateSpeed * Time.deltaTime);
                    }
                }                             
            }          

            // 만약 입력값이 달리기라면 이동값에 달리기 속도값을 곱한다.
            if(keyValue == Constants.MoveRun)
            {
                hor *= _character.runSpeedMult;
                ver *= _character.runSpeedMult;
            }

            // 실질적인 이동은 actionManager의 Move 함수에 이동값(걷기, 달리기, 중지 등)과 방향, 이동량을 파라메터로 보내는것으로 이루어진다.
            if(isVertical)
            {                
                _actionManager.Move(keyValue, direction, ver);
            }
            else
            {
                _actionManager.Move(keyValue, direction, hor);
            }

            currentState = Constants.CurrentState.Move;

        }
        else if ((currentState != Constants.CurrentState.Idle) && (horRaw == 0f) && (verRaw == 0f))
        {   
            // Idle 상태가 아닐때 입력값이 없으면 정지한것으로 간주하고 이동 정지명령과 함께 Idle 상태로 복귀한다.
            isRotate = false;
            currentState = Constants.CurrentState.Idle;
            _actionManager.Move(keyValue, 0, 0f);
        }

        return isMove;
    }

    // 키보드의 인풋을 체크하고 그 결과를 이동상태로 반환하는 함수
    private int KeyInputCheck()
    {
        int keyInput = Constants.MoveStop;

        GetAxsis();

        if ((hor != 0) || (ver != 0))
        {
            keyInput = Constants.MoveWalk;

            if (_state.canRun && Input.GetKey(KeyCode.LeftShift))
            {
                keyInput = Constants.MoveRun;
            }
        }

        return keyInput;
    }

    // 점프 가능한 상태인지를 체크후에 점프를 실행하고 그 결과를 반환하는 함수
    private bool CheckJump()
    {
        bool isJump = false;

        if(jumpDelay <= 0f) // 점프 재시도 딜레이가 존재
        {
            // 점프 입력이 들어왔을때 점프를 실행
            if ((Input.GetAxisRaw("Jump") > 0f) && ((currentState == Constants.CurrentState.Idle) || (currentState == Constants.CurrentState.Move)))
            {                
                isJump = true;

                currentState = Constants.CurrentState.Jump;

                _skillManager.cantUseSkill = true;  // 스킬을 사용할 수 없다.

                GetAxsis();

                // 움직이는 도중 점프했다면 이동 방향을 함께 파라메터로 전달하여 점프한다.
                if ((hor == 0f) && (ver == 0f))
                {
                    _actionManager.Jump(Constants.JumpUp, _character.jumpUpForce, 0f);  // 실질적으로 점프하는 함수
                }
                else if (ver > 0f)
                {
                    ver *= _character.jumpFrontForce;
                    _actionManager.Jump(Constants.JumpVer, _character.jumpUpForce, ver);
                }
                else if (ver < 0f)
                {
                    ver *= _character.jumpBackForce;
                    _actionManager.Jump(Constants.JumpVer, _character.jumpUpForce, ver);
                }
                else if (hor != 0f)
                {
                    hor *= _character.jumpSideForce;
                    _actionManager.Jump(Constants.JumpHor, _character.jumpUpForce, hor);
                }
            }
            // 점프 상태일때 점프가 끝났다면 다시 Idle 상태로 복귀
            else if ((currentState == Constants.CurrentState.Jump) && !_actionManager.GetJumpState)
            {
                currentState = Constants.CurrentState.Idle;
                jumpDelay = _character.jumpDelay;

                if (isCombatMode)
                {
                    _skillManager.cantUseSkill = false;
                }

                if (isCombatMode)
                {
                    isGlobalCheck = false;
                }

                GetAxsis();

                if((hor == 0f) && (ver == 0f))
                {
                    _actionManager.JumpToIdle();
                    isRotate = false;
                }                
            }
        }

        return isJump;
    }

    // 캐릭터의 모드(전투, 비전투)를 체크하여 입력값에 따라 모드를 변환하는 함수
    private void CheckModeChange()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentState = Constants.CurrentState.ModeChange;
            canCameraMove = false;
            handWeapon.SetActive(true); // 손에 든 무기와 등 뒤의 무기를 스왚
            backWeapon.SetActive(false);

            // 현재의 모드 상태에 따라 다르게 처리한다.
            if (isCombatMode)
            {
                isCombatMode = false;
                _actionManager.DefaultMode();
                StartCoroutine(CheckChangeEnd("Sword Sheath")); // 코루틴을 통해 모드변환을 진행
            }
            else
            {
                isCombatMode = true;
                _actionManager.CombatMode();
                StartCoroutine(CheckChangeEnd("Sword Draw"));                
            }
        }
    }

    // 모드변환 애니메이션이 종료된 후에 관련 변수를 설정하는 코루틴
    private IEnumerator CheckChangeEnd(string name)
    {
        yield return StartCoroutine(WaitForAnimation(name, 0.9f));  // 해당 애니메이션이 종료된 후
        
        // Idle상태로 복귀 및 카메라 이동 복구
        currentState = Constants.CurrentState.Idle;
        canCameraMove = true;

        // 모드에 따라 다르게 처리
        if (isCombatMode)
        {
            isGlobalCheck = false;
            _skillManager.cantUseSkill = false;
        }
        if (!isCombatMode)
        {
            handWeapon.SetActive(false);
            backWeapon.SetActive(true);
            _skillManager.cantUseSkill = true;
        }
    }

    // 스킬키 입력값을 체크한 후 스킬의 발동을 시도하는 함수
    private void CheckSkillInput()
    {
        // 해당 키버튼이 눌린 상태이고 해당 스킬 슬롯에 스킬이 등록된 상태면 deligate를 통해 해당 스킬을 사용하고 관련 변수를 셋팅
        if (Input.GetMouseButtonDown(1) && skillEnable[Constants.InputMouseR])
        {
            // 스킬의 사용은 해당 키값을 index로 해서 deligate 배열을 통해 사용된다.
            // 해당 스킬사용 함수는 자기자신의 스킬 타입을 반환하는데, 이 반환값을 통해 상태 변수를 조정하게 된다
            SetSkillState(skillUseDels[Constants.InputMouseR]());   
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl) && skillEnable[Constants.InputLCtrl])
		{
            SetSkillState(skillUseDels[Constants.InputLCtrl]());
        }
		else
		{
			for (int i = Constants.InputNum1; i <= Constants.InputT; i++)
			{
				if (Input.GetKeyDown(keyCodes[i]) && skillEnable[i])
				{
                    SetSkillState(skillUseDels[i]());
                }
			}
		}
    }        

    // 스킬 사용시 스킬 타입에 따라 상태변수를 조정하는 함수
    private void SetSkillState(int skillType)
    {        
        // 카메라와 캐릭터 이동을 중지시키고 스킬 상태로 전환한다.
        if(skillType == Constants.AtkSkill)
        {
            IsAttackState = true;
            currentState = Constants.CurrentState.Skill;
            canCameraMove = false;
            _actionManager.Move(Constants.MoveStop, 0, 0f);
        }
        else if(skillType == Constants.NotAtkSkill)
        {
            IsAttackState = false;
            currentState = Constants.CurrentState.Skill;
            canCameraMove = false;
            _actionManager.Move(Constants.MoveStop, 0, 0f);
        }
    }
        
    // 스킬이 종료됐는지 체크하는 함수
    private void CheckSkillEnd()
    {
        // 스킬이 종료되면 Idle로 복귀한다.
        if(!_skillManager.isSkillState)
        {
            currentState = Constants.CurrentState.Idle;            
            canCameraMove = true;
            isRotate = false;
            IsAttackState = false;

            if (!CheckJump() && !CheckMove())
            {
                _actionManager.Idle();
            }
        }
    }

    //public void SkillChange(int num, SkillUseDeligate del)
    //{

    //}
}


