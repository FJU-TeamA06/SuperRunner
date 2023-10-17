using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip bgmBackground; // 背景音樂
    //public AudioClip[] collisionSounds;// 碰撞音效
    public AudioClip seShoot;// 碰撞音效槍
    public AudioClip seCollision;// 碰撞音效
    public AudioClip seDamage;// 碰撞音效被打到

    private AudioSource backgroundMusicSource;
    private AudioSource collisionSoundSource1;
    private AudioSource collisionSoundSource2;
    private AudioSource collisionSoundSource3;

    private float originalBackgroundMusicVolume;

    // Start is called before the first frame update
    void Start()
    {
        // 添加背景音樂的AudioSource
        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource.clip = bgmBackground;
        backgroundMusicSource.loop = true;
        backgroundMusicSource.Play();

        //collisionSoundSources = new AudioSource[collisionSounds.Length];
        //for (int i = 0; i < collisionSounds.Length; i++)
        //{
        //    collisionSoundSources[i] = gameObject.AddComponent<AudioSource>();
        //    collisionSoundSources[i].clip = collisionSounds[i];
        //}

        // 添加碰撞音效的AudioSource
        collisionSoundSource1 = gameObject.AddComponent<AudioSource>();
        collisionSoundSource2 = gameObject.AddComponent<AudioSource>();
        collisionSoundSource3 = gameObject.AddComponent<AudioSource>();
        collisionSoundSource1.clip = seShoot;
        collisionSoundSource2.clip = seCollision;
        collisionSoundSource3.clip = seDamage;

        originalBackgroundMusicVolume = backgroundMusicSource.volume;

        //for(int i = 0; i < 3; i++)
        //{
        //  var audio = this.gameObject.AddComponent<AudioSource>;
        //  audios.Add(audio);
        //}
    }

    //private void PlayRandomCollisionSound()
    //{
    //    int randomIndex = Random.Range(0, collisionSoundSources.Length);
    //    collisionSoundSources[randomIndex].Play();
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // 碰撞到的物體的Tag設置為"Player"
        {
            collisionSoundSource1.clip = seShoot;
            collisionSoundSource1.Play();
            
            // 降低背景音樂的音量
            backgroundMusicSource.volume = originalBackgroundMusicVolume * 0.3f;
        }
        else if (collision.collider.CompareTag("trapdead") && collision.other.gameObject.CompareTag("Player"))
        {
            collisionSoundSource2.clip = seCollision;
            collisionSoundSource3.Play();

            // 降低背景音樂的音量
            backgroundMusicSource.volume = originalBackgroundMusicVolume * 0.3f;
        }
        else if (collision.gameObject.CompareTag("Object3"))
        {
            collisionSoundSource3.clip = seDamage;
            collisionSoundSource3.Play();

            // 降低背景音樂的音量
            backgroundMusicSource.volume = originalBackgroundMusicVolume * 0.3f;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // 在碰撞結束時恢復背景音樂音量
        backgroundMusicSource.volume = originalBackgroundMusicVolume;
    }

    //List<AudioSource> audios = new List<AudioSource>();

    //void Play(int index, string name,bool isloop)
    //{
    //var clip = GetAudioClip(name);
    //if(clip != null)
    //{
    //    var audio = audios[index];
    //    audio.clip = clip;
    //    audio.loop = isloop;
    //    audio.Play();
    //}
    //}

    //AudioClip GetAudioClip(string name)
    //{
    //switch (name)
    //{
    //    case "bgmBackground":
    //        return bgmBackground;
    //    case "seShoot":
    //        return seShoot;
    //    case "seCollision":
    //        return seCollision;
    //    case "seDamage":
    //        return seDamage;
    //}
    //return null;
    // }
}
