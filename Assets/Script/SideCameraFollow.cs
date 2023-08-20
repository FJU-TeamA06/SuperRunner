using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SideCameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private NetworkRunner networkRunner;

    private void Start()
    {
        offset = new Vector3(-10f, 5f, 0f);
        networkRunner = FindObjectOfType<NetworkRunner>();
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        while (target == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                NetworkObject networkObject = player.GetComponent<NetworkObject>();

                if (networkObject != null && networkObject.InputAuthority == networkRunner.LocalPlayer)
                {
                    target = player.transform;
                    break;
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
