using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        void OnTriggerEnter(Collision other)  //目前無作用待更改
        {
            if (other.gameObject.CompareTag("Coin"))
            {
                //Debug.Log("Get Coin!");
                Destroy(other.gameObject);
            }
        }
    }
}
