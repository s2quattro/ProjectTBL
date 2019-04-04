using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Manager : MonoBehaviour {

    static Audio_Manager instance = null;
    private AudioSource audioSource;
    
    private void Awake()
    {
        // 싱글톤 패턴을 위해 자기자신을 인스턴스로 지정
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    // 다른 스크립트들에서 접근할 때 호출된다.
    static public Audio_Manager GetInstance()
    {
        return instance;
    }

    // 원하는 소리를 매개변수로 받아 재생시킨다.
    public void PlayAuio(AudioClip audio)
    {
        audioSource.clip = audio;
        audioSource.Play();
    }
}
