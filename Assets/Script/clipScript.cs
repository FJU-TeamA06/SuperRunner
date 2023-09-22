using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clipScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 新增碰撞檢測
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Player")
            {
                // 碰到 Player 物件，Despawn 掉自己
                //Destroy(gameObject);
            }
        }
    }
}