using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyUIManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    [SerializeField] private Transform playerListParent;
    [SerializeField] private GameObject playerTextPrefab;

    private void Start()
    {
        RefreshPlayerList();
    }

    public override void OnJoinedRoom()
    {
        RefreshPlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshPlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RefreshPlayerList();
    }

    private void RefreshPlayerList()
    {
        // Clear old entries
        foreach (Transform child in playerListParent)
        {
            Destroy(child.gameObject);
        }

        // Rebuild list from Photon state
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(playerTextPrefab, playerListParent);

            TMP_Text text = entry.GetComponent<TMP_Text>();

            if (player.IsMasterClient)
            {
                text.text = player.NickName + " (Host)";
            }
            else
            {
                text.text = player.NickName;
            }
        }
    }
}