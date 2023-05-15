using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class StartWall : NetworkBehaviour
{
    public override void Spawned()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        
    }
    public void DespawnWall()
    {
        Runner.Despawn(Object);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestDespawnWall_RPC()
    {
        DespawnWall();
    }
}
