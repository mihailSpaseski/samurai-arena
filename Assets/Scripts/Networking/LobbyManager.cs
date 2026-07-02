using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int maxPlayers = 6;
    [SerializeField] private int minPlayers = 2;
    // [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private GameObject startButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text playerCountText;

    private bool isStarting = false;

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            NetworkManager.Instance.Connect();
        }
        else if (PhotonNetwork.InRoom)
        {
            // still in old room from last game — leave first
            NetworkManager.Instance.ReturnToLobby();
        }
        else if (PhotonNetwork.IsConnectedAndReady)
        {
            JoinOrCreateRoom();
        }
    }

    public override void OnConnectedToMaster()
    {
        JoinOrCreateRoom();
    }

    public override void OnJoinedLobby()
    {
        JoinOrCreateRoom();
    }

    public void JoinOrCreateRoom()
    {
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayers,
            IsVisible = true,
            IsOpen = true
        };
        PhotonNetwork.JoinOrCreateRoom("FFA_Room", options, TypedLobby.Default);
    }

    public override void OnJoinedRoom() => UpdateUI();
    public override void OnPlayerEnteredRoom(Player newPlayer) => UpdateUI();
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        isStarting = false;
        StopAllCoroutines();
        UpdateUI();
    }

    private void UpdateUI()
    {
        int count = PhotonNetwork.CurrentRoom.PlayerCount;

        if (playerCountText != null)
            playerCountText.text = $"Players: {count}/{maxPlayers}";

        if (startButton != null)
            startButton.SetActive(
                PhotonNetwork.IsMasterClient &&
                count >= minPlayers &&
                !isStarting
            );

        if (statusText != null)
        {
            if (count <= minPlayers)
                statusText.text = $"Waiting for players... ({count}/{minPlayers} minimum)";
            else if (PhotonNetwork.IsMasterClient)
                statusText.text = "Press START when ready!";
            else
                statusText.text = "Waiting for host to start...";
        }
    }

    public void StartGame()
    {
        Debug.Log($"StartGame called. IsMasterClient: {PhotonNetwork.IsMasterClient}, PlayerCount: {PhotonNetwork.CurrentRoom.PlayerCount}, isStarting: {isStarting}");

        if (!PhotonNetwork.IsMasterClient) return;
        if (isStarting) return;

        isStarting = true;
        startButton.SetActive(false);

        Debug.Log("Sending RPC_StartCountdown to all");
        photonView.RPC("RPC_StartCountdown", RpcTarget.All);
    }

    public void OnMainMenuPressed()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    [PunRPC]
    private void RPC_StartCountdown()
    {
        Debug.Log("RPC_StartCountdown received!");
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        for (int i = 3; i > 0; i--)
        {
            if (statusText != null)
                statusText.text = $"Starting in {i}...";
            yield return new WaitForSeconds(1f);
        }

        if (statusText != null)
            statusText.text = "GO!";

        yield return new WaitForSeconds(0.5f);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(2);
            // PhotonNetwork.LoadLevel(gameSceneName);
        }
    }
}