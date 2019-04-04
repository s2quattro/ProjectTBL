using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 오브젝트풀 매니저 스크립트
public class ObjectPool_Manager : MonoBehaviour {

    [Serializable]
    public class ObjectPoolSetup
    {
        public string resourcePath; // 프리팹 리소스가 존재하는 경로
        [Range(1, 100)] public int counts;  // 생성할 개수
    }

    public ObjectPoolSetup[] _objPoolSetup; // 원하는만큼 인스펙터를 통해 미리 생성해놓는다.

    // 실질적인 오브젝트풀 관련 클래스
    class ObjectPool
    {
        // 경로와 개수를 인수로 받아 생성자를 통해 개체들을 만들어놓는다.
        public ObjectPool(string path, int count)
        {
            prefab = Resources.Load(path) as GameObject;
            for (int i = 0; i < count; i++)
            {
                prefab.SetActive(false);
                GameObject obj = Instantiate(prefab);
                stack.Push(obj);    // 스택을 통해 개체를 관리한다.
            }
            this.count = count;
        }
        // 스택에서 개체를 꺼낼때 사용하는 함수
        public GameObject GetItem()
        {
            GameObject ret = null;
            if (useCount >= count)  // 최대개수보다 더 많은 개체가 필요할때는 개체를 인스턴스로 새로 생성한다.
            {
                ret = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
                count++;
            }
            else
            {
                ret = stack.Pop();  // 스택에서 개체를 꺼낸다.
            }
            useCount += 1;
            ret.SetActive(true);
            return ret;
        }
        // 스택에 다시 개체를 집어넣을때 사용하는 함수
        public void ReturnItem(GameObject obj)
        {
            obj.SetActive(false);
            stack.Push(obj);    // 스택에 개체를 다시 집어넣는다.
            useCount--;
        }

        GameObject prefab;
        int count = 0;
        int useCount = 0;
        Stack<GameObject> stack = new Stack<GameObject>();
    }

    // 딕셔너리 자료형은 속도가 빠르기 때문에 리소스의 path경로(key)에 따라 개체를 따로 관리하기 용이하다.
    Dictionary<string, ObjectPool> dicObjPool = new Dictionary<string, ObjectPool>();

    static ObjectPool_Manager instance = null;

    static public ObjectPool_Manager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    // 외부에서 개체를 가져올때 호출하는 함수
    public GameObject GetItem(string path)
    {
        return dicObjPool[path].GetItem();
    }

    // 외부에서 개체를 반환할때 사용하는 함수
    public void ReleaseItem(string path, GameObject obj)
    {
        dicObjPool[path].ReturnItem(obj);
    }

    // Use this for initialization
    void Start () {

        ObjectPool pool;

        for(int i=0; i<_objPoolSetup.Length; i++)
        {
            pool = new ObjectPool(_objPoolSetup[i].resourcePath, _objPoolSetup[i].counts);  // inpecter에 설정했던 정보를 토대로 생성자를 통해 오브젝트풀을 생성한다.
            dicObjPool.Add(_objPoolSetup[i].resourcePath, pool);    // 해당 오브젝트풀을 딕셔너리에 추가한다.
        }
    }
}
