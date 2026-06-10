using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;

    public override void OnJoinedRoom()
    {
        SpawnLocalPlayer();
    }

    private void SpawnLocalPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerSpawner: playerPrefab is not assigned!");
            return;
        }

        Vector3 spawnPos = SpawnPointManager.Instance.GetRandomSpawnPoint();
        Debug.Log($"Spawning player at {spawnPos}");

        PhotonNetwork.Instantiate(
            playerPrefab.name,
            spawnPos,
            Quaternion.identity
        );
    }
}