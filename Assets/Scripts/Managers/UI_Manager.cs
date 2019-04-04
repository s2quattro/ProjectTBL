using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

// UI와 관련된 모든것들을 관리하는 스크립트
public class UI_Manager : MonoBehaviour {

    public Transform spawnNearPos;
    public Transform spawnFarPos;

    private List<GameObject> spawnObjects = new List<GameObject>();

    // 드롭다운 UI의 정보를 저장하는 클래스
    private class DropdownClass
    {
        private int dropChar;
        private int dropForce;
        private int dropOrder;
        private int dropLocation;

        public int DropChar { get; set; }
        public int DropForce { get; set; }
        public int DropOrder { get; set; }
        public int DropLocation { get; set; }
    }
    private DropdownClass _dropDown = new DropdownClass();

    static UI_Manager instance = null;
    static public UI_Manager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public delegate float SkillCoolDeligate();
    public delegate bool SkillGlobalDeligate();

    [Serializable]
    public class SkillUIInfo // 각각의 스킬에대한 쿨타임(델리게이트), 스킬이미지, 스킬타입 등의 정보를 가지는 클래스
    {
        public Sprite skillIcon;

        [HideInInspector]
        public SkillCoolDeligate skillCoolDel = null;
        //[HideInInspector]
        //public SkillUseDeligate skillUseDel = null;
        [HideInInspector]
        public SkillGlobalDeligate skillGlobalDel = null;
        [HideInInspector]
        public Image skillImage;
        [HideInInspector]
        public bool isUsing = false;
        [HideInInspector]
        public Constants.ComboType comboType;
    }

    public SkillUIInfo[] _skillInfos;

    private Skill_Manager _skillManager;

    private Color defaultColor;
    private Color cooltimeColor;

    private Color whiteColor = new Color(255f / 255f, 255f / 255f, 255f / 255f);
    private Color grayColor = new Color(63f / 255f, 59f / 255f, 59f / 255f);
    private Color redColor = new Color(231f / 255f, 121f / 255f, 121f / 255f);

    private float cooltimeRatio;


    // Use this for initialization
    void Start() {

        cooltimeColor = new Color(44 / 255f, 44 / 255f, 44 / 255f);
        defaultColor = new Color(255 / 255f, 255 / 255f, 255 / 255f);

        Image[] images = transform.GetChild(0).GetComponentsInChildren<Image>();    // 배열로 이미지 컴포넌트들을 참조한다.

        for (int i = 0; i < _skillInfos.Length; i++)
        {
            // 해당 인덱스의 이미지가 존재하면 해당 스킬을 사용하는 것으로 간주하고 스킬UI 클래스가 이미지등을 참조하도록 한다.
            if (images[i] != null)
            {
                _skillInfos[i].skillImage = images[i];
                _skillInfos[i].skillImage.sprite = _skillInfos[i].skillIcon;
                _skillInfos[i].isUsing = true;
            }
            else
            {
                break;
            }
        }
    }

    // Update is called once per frame
    void Update() {

        CheckCooltimeAndChangeIcon();

        CheckSpawnKeyInput();
    }

    // 플레이어 컨트롤러 스크립트가 해당 함수를 호출하여 스킬에 대한 스크립트들을 매개변수로 전달한다.
    public void SetSkillInfo(Player_Controller playerController, Skill_Manager skillManager, Skill_Base[] skills)
    {
        // 이후 스크립트와 델리게이트들을 참조하여 사용한다.

        _skillManager = skillManager;

        for (int i = 0; i < skills.Length; i++)
        {
            if (i < _skillInfos.Length)
            {
                if (skills[i].isActiveAndEnabled)
                {
                    //_skills[i].skillUseDel = skills[i].TrySkill;
                    _skillInfos[i].skillCoolDel = skills[i].GetCooltime;
                    _skillInfos[i].skillGlobalDel = skills[i].GetEffectOnGlobal;
                    _skillInfos[i].comboType = skills[i]._skillInfo.comboType;
                    _skillInfos[i].isUsing = true;
                }
                else
                {
                    _skillInfos[i].isUsing = false; // 해당 스킬 컴포넌트가 비활성화 돼있으므로 스킬이 없는것으로 취급
                }
            }
            else
            {
                break;
            }
        }

        //skillChangeDel = playerController.SkillChange;
    }

    // 쿨타임을 체크하여 아이콘 등을 갱신하는 함수
    private void CheckCooltimeAndChangeIcon()
    {
        for (int i = 0; i < _skillInfos.Length; i++)
        {
            if (_skillInfos[i].isUsing && (_skillInfos[i].skillCoolDel != null))
            {
                cooltimeRatio = _skillInfos[i].skillCoolDel();  // 델리게이트를 통해 해당 스킬의 쿨타임 비율을 가져온다.

                if (cooltimeRatio > 0f)
                {
                    _skillInfos[i].skillImage.color = cooltimeColor;    // 쿨타임으로 아이콘의 색깔 결정
                    if (cooltimeRatio < 1f)
                    {
                        _skillInfos[i].skillImage.fillAmount = 1f - cooltimeRatio;  // 아이콘을 통해 쿨타임을 알 수 있도록 한 부분
                    }
                }
                else
                {
                    _skillInfos[i].skillImage.fillAmount = 1f;
                    if (!_skillManager.isSkillState || _skillManager.isBlockingState)
                    {
                        _skillInfos[i].skillImage.color = defaultColor;
                    }
                }

                // 개별 쿨타임 외에 글로벌 쿨타임이 있을 때에도 쿨타임이 있는 것으로 간주
                if (!_skillManager.isSkillState && _skillInfos[i].skillGlobalDel() && (_skillManager.curGlobalCool > 0f))
                {
                    _skillInfos[i].skillImage.color = cooltimeColor;
                }
                else if (_skillManager.cantUseSkill)
                {
                    _skillInfos[i].skillImage.color = cooltimeColor;
                }
            }
            else
            {
                _skillInfos[i].skillImage.color = cooltimeColor;
            }
        }
    }

    // 스킬 관련 스크립트에서 호출하여 싱글 및 콤보스킬 경우에 따라 아이콘 컬러를 변경
    // EX) 콤보스킬 사용중에는 콤보관련 스킬만 아이콘을 정상적으로 표시하고 그외의 스킬들은 모두 쿨타임으로 표시
    public void CheckSkillIconDiable(Constants.ComboType comboType)
    {
        if ((comboType == Constants.ComboType.Single) || (comboType == Constants.ComboType.Finish))
        {
            for (int i = 0; i < _skillInfos.Length; i++)
            {
                if (_skillInfos[i].isUsing)
                {
                    _skillInfos[i].skillImage.color = cooltimeColor;
                }
            }
        }
        else if (comboType == Constants.ComboType.Combo)
        {
            for (int i = 0; i < _skillInfos.Length; i++)
            {
                if (_skillInfos[i].isUsing && (_skillInfos[i].comboType == Constants.ComboType.Single))
                {
                    _skillInfos[i].skillImage.color = cooltimeColor;
                }
            }
        }
    }

    // AI 생성 관련 키 인풋을 처리하는 함수
    private void CheckSpawnKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SpawnAI();
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            DeleteAllAI();
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            CallMainMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }

    // 드롭다운 종류마다 각각의 핸들러를 설정해서 클래스에 드롭다운 정보를 저장
    public void DropdownCharacterHandler(Dropdown drop)
    {
        _dropDown.DropChar = drop.value;
    }
    public void DropdownForceHandler(Dropdown drop)
    {
        _dropDown.DropForce = drop.value;
    }
    public void DropdownOrderHandler(Dropdown drop)
    {
        _dropDown.DropOrder = drop.value;
    }
    public void DropdownLocationHandler(Dropdown drop)
    {
        _dropDown.DropLocation = drop.value;
    }

    // AI를 소환하는 함수
    public void SpawnAI()
    {
        GameObject spawnCharacter = ObjectPool_Manager.GetInstance().GetItem("AI/Brute");   // 오브젝트풀 매니저로부터 AI 개체를 하나 가져온다.

        int count = spawnCharacter.transform.GetChild(0).transform.GetChild(1).transform.childCount;    // 머테리얼 색깔 변경을 위한 모델링 오브젝트 개수를 저장

        spawnObjects.Add(spawnCharacter);    // 이후 일괄삭제를 위해 소환한 캐릭터들을 따로 리스트로 관리

        switch (_dropDown.DropForce)
        {
            // 드롭다운 클래스에 저장된 정보에 따라 AI 설정을 각각 다르게 초기화하여 소환
            case Constants.DropForce1:
                {
                    //spawnCharacter.GetComponent<Controller_Entity>().SetTagLayer(Controller_Entity.Force.Force2, Constants.LayerForce1);
                    spawnCharacter.GetComponent<Controller_Base>().SetTagLayer(Controller_Base.Force.Force1);   // AI의 세력 설정
                    for (int i = 0; i < count; i++)
                    {
                        // 세력 구분을 위해 AI 모델링 오브젝트의 색깔 변경
                        spawnCharacter.transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material.color = whiteColor;
                    }
                    break;
                }
            case Constants.DropForce2:
                {
                    //spawnCharacter.GetComponent<Controller_Entity>().SetTagLayer(Controller_Entity.Force.Force1, Constants.LayerForce2);
                    spawnCharacter.GetComponent<Controller_Base>().SetTagLayer(Controller_Base.Force.Force2);
                    for (int i = 0; i < count; i++)
                    {
                        spawnCharacter.transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material.color = grayColor;
                    }
                    break;
                }
            case Constants.DropForce3:
                {
                    //spawnCharacter.GetComponent<Controller_Entity>().SetTagLayer(Controller_Entity.Force.Force1, Constants.LayerForce2);
                    spawnCharacter.GetComponent<Controller_Base>().SetTagLayer(Controller_Base.Force.Force3);
                    for (int i = 0; i < count; i++)
                    {
                        spawnCharacter.transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material.color = redColor;
                    }
                    break;
                }
            default:
                {
                    //spawnCharacter.GetComponent<Controller_Entity>().SetTagLayer(Controller_Entity.Force.Force2, Constants.LayerForce1);
                    spawnCharacter.GetComponent<Controller_Base>().SetTagLayer(Controller_Base.Force.Force1);
                    for (int i = 0; i < count; i++)
                    {
                        spawnCharacter.transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material.color = whiteColor;
                    }
                    break;
                }
        }

        switch (_dropDown.DropOrder)
        {
            // AI의 State들 중 Move가 없는 경우를 선택시 Move State를 사용안함으로 변경
            case Constants.DropOrderPatrol:
                {
                    spawnCharacter.GetComponent<State_Move>().enabled = false;
                    break;
                }

            case Constants.DropOrderMove:
                {
                    spawnCharacter.GetComponent<State_Move>().enabled = true;
                    break;
                }
            default:
                {
                    spawnCharacter.GetComponent<State_Move>().enabled = true;
                    break;
                }
        }

        Vector3 vec;

        // 랜덤위치에 생성하기위해 난수 생성
        vec.x = UnityEngine.Random.Range(-8f, 8f);
        vec.y = UnityEngine.Random.Range(0f, 0f);
        vec.z = UnityEngine.Random.Range(-8f, 8f);

        switch (_dropDown.DropLocation)
        {
            case Constants.DropLocationFar:
                {
                    Vector3 v = spawnFarPos.position + vec;
                    spawnCharacter.transform.position = v;
                    break;
                }
            case Constants.DropLocationNear:
                {
                    Vector3 v = spawnNearPos.position + vec;
                    spawnCharacter.transform.position = v;
                    break;
                }
            default:
                {
                    Vector3 v = spawnFarPos.position + vec;
                    spawnCharacter.transform.position = v;
                    break;
                }
        }

        spawnCharacter.GetComponent<State_Controller>().InitState();
    }

    // 생성된 AI들을 일괄 삭제시키는 함수
    private void DeleteAllAI()
    {
        for (int i = 0; i < spawnObjects.Count; i++)
        {
            // 리스트에 저장된 각각의 AI 개체들을 제거한다. 제거할 때에는 오브젝트풀 매니저에 AI 개체를 push하여 이후 다시 재사용할 수 있도록 한다.
            spawnObjects[i].GetComponent<State_Controller>().DeleteCharacter();
            //ObjectPool_Manager.GetInstance().ReleaseItem(spawnCharacters[i].GetComponent<Controller_Entity>().resourcePath, spawnCharacters[i]);
        }

        spawnObjects.Clear();    // 다 제거한 후 리스트를 초기화
    }

    private void CallMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    //public String[] deleteCharacterNames;
    //public delegate int SkillUseDeligate();
    //private delegate void SkillChangeDeligate(int num, SkillUseDeligate del);
    //private SkillChangeDeligate skillChangeDel;
}