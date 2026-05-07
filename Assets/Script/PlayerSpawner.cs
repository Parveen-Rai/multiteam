using UnityEngine;
using Fusion;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;

    public Transform[] spawnPoint;

    public void PlayerJoined(PlayerRef player)
    {
        if(player == Runner.LocalPlayer)
        {
            Transform selectedSpawnPoint = spawnPoint[Random.Range(0, spawnPoint.Length)];
            Runner.Spawn(playerPrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation, player);
        }
    }
}
