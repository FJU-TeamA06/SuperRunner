using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [Networked]
    private TickTimer life { get; set; }

    [SerializeField]
    private float bulletSpeed = 8f;
    public int blood = 0;
    public GameObject bloodPrefab;

    //AudioSource
    public AudioClip seDamage;// 碰撞音效被打到
    private AudioSource collisionSoundSource;

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        collisionSoundSource = gameObject.AddComponent<AudioSource>();
        collisionSoundSource.clip = seDamage;
        collisionSoundSource.loop = false;
    }

        void Start()
    {
        audioClips.Add("damage", seDamage);
    }

    public override void Spawned()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            transform.position += bulletSpeed * transform.forward * Runner.DeltaTime;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();

            string selectedSound = "damage";
            if (audioClips.ContainsKey(selectedSound))
            {
                GetComponent<AudioSource>().clip = audioClips[selectedSound];
                GetComponent<AudioSource>().Play(); // 播放所選擇的音檔

                //backgroundMusicSource.volume = originalBackgroundMusicVolume * 0.1f;
            }
            player.TakeDamage(34);
            /*blood = 1;
            var particleSystem = bloodPrefab.GetComponent<ParticleSystem>();
            if (blood == 1)
            {
                bloodPrefab.SetActive(true);
                particleSystem.Play();
            }
            blood = 0;*/

            Runner.Despawn(Object);
        }
    }
}
