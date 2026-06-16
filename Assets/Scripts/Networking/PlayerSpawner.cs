using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        // already in room when scene loads — spawn immediately
        if (PhotonNetwork.InRoom)
        {
            SpawnLocalPlayer();
        }
    }

    public override void OnJoinedRoom()
    {
        // fallback if joining room after scene loads
        SpawnLocalPlayer();
    }

    private void SpawnLocalPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerSpawner: playerPrefab not assigned!");
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