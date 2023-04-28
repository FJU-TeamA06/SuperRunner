using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SquareScript : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the touching object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Get the PlayerController script from the touching object
            PlayerController playerController = other.GetComponent<PlayerController>();

            // Ensure that the playerController component is attached
            if (playerController != null)
            {
                // Get the player ID from the playerController script
                //int playerID = playerController.PlayerID;

                //Debug.Log("Square A touched by ID: " + playerID);
            }
        }
    }
}
