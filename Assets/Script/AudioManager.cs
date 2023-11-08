using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip backgroundMusicClip;
    public AudioClip seShoot;// 碰撞音效槍
    public AudioClip seCollision;// 碰撞音效
    public AudioClip seCactus;// 碰撞音效被打到
    public AudioClip shootMusicClip;//level3 backgroundmusic

    private AudioSource backgroundMusicSource;
    private AudioSource collisionMusicSource;
    private AudioSource shootMusicSource;

    private Dictionary<string, List<AudioClip>> audioClips = new Dictionary<string, List<AudioClip>>();

    void Start()
    {
        //backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        //collisionMusicSource = gameObject.AddComponent<AudioSource>();
        //specialConditionMusicSource = gameObject.AddComponent<AudioSource>();

        //backgroundMusicSource.clip = backgroundMusicClip;
        ////collisionMusicSource.clip = seShoot;
        //specialConditionMusicSource.clip = specialConditionMusicClip;

        ////backAudios.Add("background", new List<AudioClip> { bgmBackground });
        ////backAudios.Add("shootbackg", new List<AudioClip> { bgmBackgroundFPS });
        //audioClips.Add("shoot", new List<AudioClip> { seShoot });
        //audioClips.Add("collision", new List<AudioClip> { seCollision, seDamage });
        //audioClips.Add("cactus", new List<AudioClip> { seCactus });

        //// 播放背景音樂
        //backgroundMusicSource.Play();
        //backgroundMusicSource.loop = true;
    }

    void Update()
    {
        //// 在某些條件下觸發背景音樂切換為特殊條件音樂
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    // 停止播放背景音樂
        //    backgroundMusicSource.Stop();

        //    // 播放特殊條件音樂
        //    shootMusicSource.Play();
        //    shootMusicSource.loop = true;
        //}
    }

    // 在碰撞事件中觸發碰撞音樂播放
    private void OnCollisionEnter(Collision collision)
    {
        // 播放碰撞音樂
        //collisionMusicSource.Play();
    }
}