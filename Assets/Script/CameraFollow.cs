using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CameraFollow : NetworkBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void Start()
    {
        offset = new Vector3(0f, 5f, -10f); // 修改这里的值以更改摄像机的初始位置
        StartCoroutine(FindPlayer());
    }


    private IEnumerator FindPlayer()
    {
        while (target == null)
        {
            // 找到當前玩家
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            print("1");
            foreach (GameObject player in players)
            {
                NetworkObject networkObject = player.GetComponent<NetworkObject>();
                
                if (networkObject != null && networkObject.HasStateAuthority)
                {
                    
                    target = player.transform;
                    
                    break;
                }
            }

            // 等待一段時間再次檢查
            yield return new WaitForSeconds(1f);
        }
        print("ok");
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        // 計算新的攝影機位置
        
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 讓攝影機不朝向目標
        //transform.LookAt(target);
    }
}
