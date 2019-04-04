using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 델리게이트 연습용 스크립트
public class DeligateTest : MonoBehaviour {
	
	public delegate void del(float a);
	public del func1 = null;

	public class Character1
	{
		public void attack(float damage)
		{
            
			Debug.Log("데미지 : " + damage * 1.5f);
		}
	}

	public class Character2
	{
		public void attack(float damage)
		{
			Debug.Log("데미지 : " + damage * 3.5f);
		}
	}	

	

	// Use this for initialization
	void Start () {
		Character1 character1 = new Character1();
		Character2 character2 = new Character2();

		func1 = character1.attack;
		func1(10);
		func1 = character2.attack;
		func1(10);

		float num = 0.1f;
		func1 = delegate (float n1)
		{
			Debug.Log(n1 + num);
		};
		func1(10);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
