using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        PhotonNetwork.AutomaticallySyncScene = true; // ← add here too

        if (string.IsNullOrEmpty(PhotonNetwork.NickName))
            PhotonNetwork.NickName = "Player " + Random.Range(1000, 9999);
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected) return;
        PhotonNetwork.NickName = "Player " + Random.Range(1000, 9999);
        PhotonNetwork.AutomaticallySyncScene = true; // ← must be BEFORE ConnectUsingSettings
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to Photon...");
    }

    // call this when returning to lobby from game
    public void ReturnToLobby()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        else
            PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnLeftRoom()
    {
        // after leaving room we're back on Master, now join lobby
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby — ready for matchmaking");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Disconnected: {cause}");
        Connect();
    }
}