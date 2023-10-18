
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

        // 計算相機的目標位置，根據玩家的朝向加上偏移量
        Vector3 desiredPosition = target.position + target.right * offset.x + target.up * offset.y + target.forward * offset.z;
        // 使用線性插值平滑移動相機
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        // 讓相機看向玩家
        transform.LookAt(target);
    }
}