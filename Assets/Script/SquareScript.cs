using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SquareScript : NetworkBehaviour
{
    [Networked] public int isFinished { get; set; }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the touching object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Get the PlayerController script from the touching object
            var player = other.GetComponent<PlayerController>();
            // Ensure that the playerController component is attached
            if (player != null)
            {
                // Get the player ID from the playerController script
                //int playerID = playerController.PlayerID;

                //Debug.Log("Square A touched by ID: " + playerID);
                print("Someone Finished "+ "Is " + player.PlayerName);
                isFinished=1;
                Finish_RPC(isFinished);
                
            }
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Finish_RPC(int a)
    {
        print("Square:"+a);
    }
}
    
