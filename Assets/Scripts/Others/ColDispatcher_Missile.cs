using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColDispatcher_Missile : MonoBehaviour {
    
    Missile_Entity _missileEntity;
    
    // Use this for initialization
    void Start () {
        _missileEntity = GetComponentInParent<Missile_Entity>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        // 캐릭터가 아닌 미사일이 다른 콜라이더와 충돌했을 경우 미사일의 최상위 개체 스크립트와 미사일과 충돌한 다른 콜라이더의 정보를 콜리전 매니저의 함수에 매개변수로 보낸다.
        Collision_Manager.GetInstance().ReceiveCollision(_missileEntity, other);
    }
}
