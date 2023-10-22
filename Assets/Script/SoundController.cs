//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SoundController : MonoBehaviour
//{
//    public AudioClip backgroundMusic; // 背景音樂
//    public AudioClip collisionSound; // 碰撞音效

//    private AudioSource backgroundMusicSource;
//    private AudioSource collisionSoundSource;

//    private float originalBackgroundMusicVolume;

//    private void Awake()
//    {
//        // 添加背景音樂的AudioSource
//        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
//        backgroundMusicSource.clip = backgroundMusic;
//        backgroundMusicSource.loop = true;
//        backgroundMusicSource.Play();

//        // 添加碰撞音效的AudioSource
//        collisionSoundSource = gameObject.AddComponent<AudioSource>();
//        collisionSoundSource.clip = collisionSound;
//        collisionSoundSource.loop = false;

//        originalBackgroundMusicVolume = backgroundMusicSource.volume; 
//    }

    //public void Play(int index, string name, bool isloop)
    //{
    //    var clip = GetAudioClip(name);
    //    if (clip != null)
    //    {
    //        var audio = audios[index];
    //        audio.clip = clip;
    //        audio.loop = isloop;
    //        audio.Play();
    //    }
    //}

    //AudioClip GetAudioClip(string name)
    //{
    //    switch (name)
    //    {
    //        case "bgmBackground":
    //            return bgmBackground;
    //        case "seCollision":
    //            return seCollision;
    //    }
    //    return null;
    //}

//    private void OnCollisionEnter(Collision collision)
//    {
//        Debug.Log("Collision detected"); // 輸出碰撞檢測訊息

//        if (collision.gameObject.tag == "Player") // 碰撞到的物體的Tag設置為"Player"
//        {
//            if (!collisionSoundSource.isPlaying) // 檢查音效是否正在播放，以避免重疊播放
//            {
//                // 在碰撞時播放音效
//                collisionSoundSource.Play();
//                // 降低背景音樂的音量
//                backgroundMusicSource.volume = originalBackgroundMusicVolume * 0.1f;
//                // 將音量降低為原來的 30%
//            }
//        }

//    }

//    private void OnCollisionExit(Collision collision)
//    {
//        // 在碰撞結束時恢復背景音樂音量
//        backgroundMusicSource.volume = originalBackgroundMusicVolume;
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundController : MonoBehaviour
{
    public AudioClip collisionSound; // 碰撞音效
    
    private AudioSource collisionSoundSource;

    private float originalBackgroundMusicVolume;

    //void Start()
    //{
    //    collisionSoundSource = GetComponent<AudioSource>();
    //    collisionSoundSource.clip = collisionSound;
    //    collisionSoundSource.loop = false;

    //    originalBackgroundMusicVolume = backgroundMusicSource.volume;
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        collisionSoundSource.PlayOneShot(collisionSound);
    //        backgroundMusicSource.volume = originalBackgroundMusicVolume * 0.1f;
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    // 在碰撞結束時恢復背景音樂音量
    //    backgroundMusicSource.volume = originalBackgroundMusicVolume;
    //}
}