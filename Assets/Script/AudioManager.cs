using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class Source
{
    public string name;
    public AudioClip clip;
    public AudioSource source;
    public float volume;
    public bool loop;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
    }

    // source의 clip를 실행하는 함수
    public void Play()
    {
        source.Play();
    }

    // source의 clip를 실행 중단하는 함수
    public void Stop()
    {
        source.Stop();
    }

    // source의 loop를 true로 바꾸는 함수
    public void SetLoop()
    {
        source.loop = true;
    }

    // source의 loop를 false로 바꾸는 함수
    public void SetLoopCancel()
    {
        source.loop = false;
    }

    // source의 volume을 설정하는 함수
    public void SetVolume()
    {
        source.volume = volume;
    }
}

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    Source[] sounds;

    static public AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // 인스펙터 창에서 sounds에 넣어놓은 audio 갯수만큼 반복
        for (int i = 0; i < sounds.Length; i++)
        {
            // sound 하나당 하나의 GameObject 생성
            GameObject soundObject = new GameObject("사운드 파일 이름 : " + i + " = " + sounds[i].name);
            sounds[i].SetSource(soundObject.AddComponent<AudioSource>());
            // AudioManager 오브젝트 아래로 위치
            soundObject.transform.SetParent(this.transform);
        }
    }

    // sounds 클래스의 Play 함수를 호출하기 위한 함수
    public void Play(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            // 실행하고자 하는 sound 파일을 찾기
            if (_name == sounds[i].name)
            {
                sounds[i].Play();
                return;
            }
        }
    }

    // sounds 클래스의 Stop 함수를 호출하기 위한 함수
    public void Stop(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            // 실행하고자 하는 sound 파일을 찾기
            if (_name == sounds[i].name)
            {
                sounds[i].Stop();
                return;
            }
        }
    }

    // sounds 클래스의 SetLoop 함수를 호출하기 위한 함수
    public void SetLoop(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            // 실행하고자 하는 sound 파일을 찾기
            if (_name == sounds[i].name)
            {
                sounds[i].SetLoop();
                return;
            }
        }
    }

    // sounds 클래스의 SetLoopCancel 함수를 호출하기 위한 함수
    public void SetLoopCancel(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            // 실행하고자 하는 sound 파일을 찾기
            if (_name == sounds[i].name)
            {
                sounds[i].SetLoopCancel();
                return;
            }
        }
    }

    // sounds 클래스의 SetVolume 함수를 호출하기 위한 함수
    public void SetVolume(string _name, float _volume)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            // 실행하고자 하는 sound 파일을 찾기
            if (_name == sounds[i].name)
            {
                sounds[i].volume = _volume;
                sounds[i].SetVolume();
                return;
            }
        }
    }
}
