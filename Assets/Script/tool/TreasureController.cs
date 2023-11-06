using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        void OnTriggerEnter(Collision other)
        {
            if(other.gameObject.tag == "Player")
            {
                Destroy(other.gameObject);
            }
        }
    }
}
