using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip bgmBackground;
    public AudioClip seShoot;
    public AudioClip seCollision;
    public AudioClip seDamage;

    List<AudioSource> audios = new List<AudioSource>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 3; i++)
        {
          var audio = this.gameObject.AddComponent<AudioSource>;
          audios.Add(audio);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Play(int index, string name,bool isloop)
    {
        var clip = GetAudioClip(name);
        if(clip != null)
        {
            var audio = audios[index];
            audio.clip = clip;
            audio.loop = isloop;
            audio.Play();
        }
    }

    AudioClip GetAudioClip(string name)
    {
        switch (name)
        {
            case "bgmBackground":
                return bgmBackground;
            case "seShoot":
                return seShoot;
            case "seCollision":
                return seCollision;
            case "seDamage":
                return seDamage;
        }
        return null;
    }
}
