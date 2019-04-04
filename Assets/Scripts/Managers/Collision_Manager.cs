using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Manager : MonoBehaviour {

    static Collision_Manager instance = null;

    // 무기 콜라이더와 관련된 정보들을 클래스화 시켜놓은 것. 무기와 충돌한 콜라이더를 등록시켜 중복 충돌판정이 일어나지 않게 하는 역할
    public class ColliderData
    {
        public int damage;
        
        public List<Collider> colliderList = new List<Collider>();  // 무기와 충돌한 콜라이더들을 기억하기 위해 리스트를 선언
        
        // 무기와 충돌한 콜라이더를 리스트에 추가하는 함수
        public void AddHitColliders(Collider col)
        {
            colliderList.Add(col);            
        }

        // 무기와 충돌한 콜라이더가 리스트에 이미 존재하는지 확인하는 함수
        public bool FindHitColliders(Collider col)
        {
            bool isNotExist = true;

            if(colliderList.Contains(col))
            {
                isNotExist = false;
            }

            return isNotExist;
        }

        // 리스트를 초기화하는 함수
        public void InitColliders()
        {
            colliderList.Clear();            
        }
    }

    public Dictionary<Collider, ColliderData> ColliderDic;  // 딕셔너리를 이용해 해당 무기의 콜라이더를 key로 해서 무기 콜라이더와 관련된 정보 리스트 클래스를 저장시킨다. 
        
    static public Collision_Manager GetInstance()
    {
        return instance;
    }

    // 싱글톤 패턴을 위한 인스턴스 처리
    private void Awake()
    {
        instance = this;        
    }

    // Use this for initialization
    void Start () {
        ColliderDic = new Dictionary<Collider, ColliderData>(); // 무기 콜라이더 딕셔너리 할당
    }

    // 콜라이더 데이터 클래스(리스트)에 무기와 충돌한 콜라이더를 추가하는 함수
    public void AddCollider(Collider col, ColliderData colliderData)
    {
        ColliderDic.Add(col, colliderData);
    }

    // 등록됐던 콜라이더를 제거하는 함수
    public void RemoveCollider(Collider col)
    {
        //col.enabled = true;

        if (ColliderDic.ContainsKey(col))
        {
            ColliderDic[col].InitColliders();

            ColliderDic.Remove(col);
        }
    }

    // 충돌이 발생했을 경우 Dispatcher로부터 충돌한 콜라이더들과 충돌한 캐릭터의 컨트롤러 스크립트를 매개변수로 받는다.
    // 이후 충돌판정이 유효한지 검사한다.
    public void ReceiveCollision(Controller_Base owner, Collider hitCol, Collider atkCol)
    {
        // 해당 무기가 사용되는 상태인지를 검사. 무기의 공격판정이 시작할 때에 딕셔너리에 무기 콜라이더가 등록이 되는데,
        // 현재 등록이 돼 있는지, 무기 콜라이더와 충돌한 대상 콜라이더의 팀이 다른지 검사한다.  
        if(ColliderDic.ContainsKey(atkCol) && (owner.tag != atkCol.tag))
        {
            // 정상적으로 무기와 캐릭터가 충돌한 상태일때 캐릭터가 이미 무기에 한번 충돌이 됐는지를 검사(중복충돌 검사)
            if(ColliderDic[atkCol].FindHitColliders(hitCol))
            {
                // 처음 충돌한 상태라면 캐릭터의 콜라이더를 콜라이더 데이터 클래스(리스트)에 등록시킨다.
                ColliderDic[atkCol].AddHitColliders(hitCol);

                // 공격받은 해당 캐릭터의 스크립트에 무기 콜라이더 데이터 클래스에 등록된 데미지 만큼
                // 입은 데미지의 양을 매개변수로 데미지를 입었다고 전달한다.
                owner.ReceiveDamage(ColliderDic[atkCol].damage);
            }
        }
    }

    // 미사일이 충돌했을 경우는 미사일의 스크립트와 미사일과 충돌한 개체의 콜라이더를 매개변수로 받아온다.
    public void ReceiveCollision(Missile_Entity owner, Collider other)
    {
        // 충돌한 개체가 캐릭터이고(레이어 1~5), 미사일과 충돌 대상의 팀이 다르면 충돌했다고 판정
        if ((other.gameObject.layer >= Constants.LayerForce1) && (owner.tag != other.tag))
        {
            owner.MissileCollision();   // 미사일 스크립트에 충돌했다는 사실을 알림
        }
    }
}
