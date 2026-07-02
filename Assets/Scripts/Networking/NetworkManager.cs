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
        PhotonNetwork.AutomaticallySyncScene = true;

        // always sync username in case profile was just created
        if (UserProfile.HasProfile())
            PhotonNetwork.NickName = UserProfile.GetUsername();
    }

    public void Connect()
    {
        // set nickname immediately regardless
        PhotonNetwork.NickName = UserProfile.HasProfile()
            ? UserProfile.GetUsername()
            : "Player " + Random.Range(1000, 9999);

        if (PhotonNetwork.IsConnected)
        {
            // already connected, nickname is set above, nothing else needed
            return;
        }

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log($"Connecting as: {PhotonNetwork.NickName}");
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