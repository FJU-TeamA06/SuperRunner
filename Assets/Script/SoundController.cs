using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundController : MonoBehaviour
{
    public AudioClip collisionSound; // ¸I¼²­µ®Ä
    
    private AudioSource collisionSoundSource;

    void Start()
    {
        collisionSoundSource = GetComponent<AudioSource>();
        collisionSoundSource.clip = collisionSound;
        collisionSoundSource.loop = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collisionSoundSource.PlayOneShot(collisionSound);
        }
    }
}