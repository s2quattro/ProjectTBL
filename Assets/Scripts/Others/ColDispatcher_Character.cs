using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColDispatcher_Character : MonoBehaviour {

    Controller_Base _controllerEntity;

	// Use this for initialization
	void Start () {
        _controllerEntity = GetComponentInParent<Controller_Base>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        // 캐릭터가 충돌했을 경우 콜리전 매니저의 함수를 호출하여 충돌이 발생된 캐릭터의 컨트롤러 스크립트와 콜라이더, 그리고 캐릭터와 충돌한 다른 콜라이더를 매개변수로 보낸다.
        Collision_Manager.GetInstance().ReceiveCollision(_controllerEntity, GetComponent<Collider>(), other);
    }
}
