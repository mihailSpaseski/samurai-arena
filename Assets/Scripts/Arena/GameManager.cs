using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    private const string ALIVE_KEY = "ALIVE_PLAYERS";
    private bool matchEnded = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        InitializeAlivePlayers();
    }

    // ---------------------------
    // INITIALIZATION
    // ---------------------------
    private void InitializeAlivePlayers()
    {
        List<int> alive = new List<int>();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            alive.Add(p.ActorNumber);
        }

        SetAlivePlayers(alive);

        Debug.Log($"[GameManager] Initialized alive players: {alive.Count}");
    }

    // ---------------------------
    // DEATH REPORT ENTRY POINT
    // ---------------------------
    public void ReportDeath(int actorNumber)
    {
        if (matchEnded) return;

        // Only send to master if we're not master
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_ReportDeath), RpcTarget.MasterClient, actorNumber);
            return;
        }

        HandleDeath(actorNumber);
    }

    [PunRPC]
    private void RPC_ReportDeath(int actorNumber)
    {
        HandleDeath(actorNumber);
    }

    // ---------------------------
    // MASTER LOGIC
    // ---------------------------
    private void HandleDeath(int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient || matchEnded) return;

        var alive = new List<int>(GetAlivePlayers());

        if (!alive.Contains(actorNumber))
            return;

        alive.Remove(actorNumber);

        SetAlivePlayers(alive);

        Debug.Log($"Player died: {actorNumber} | Remaining: {alive.Count}");

        CheckWinCondition(alive);
    }

    private void CheckWinCondition(List<int> alive)
    {
        if (matchEnded) return;

        if (alive.Count > 1)
            return;

        matchEnded = true;

        string winnerName = "No one";

        if (alive.Count == 1)
        {
            int winnerId = alive[0];
            Player winner = PhotonNetwork.CurrentRoom.GetPlayer(winnerId);
            winnerName = winner != null ? winner.NickName : "Unknown";
        }

        photonView.RPC(nameof(RPC_EndMatch), RpcTarget.All, winnerName);
    }

    // ---------------------------
    // END MATCH
    // ---------------------------
    [PunRPC]
    private void RPC_EndMatch(string winnerName)
    {
        if (matchEnded == false)
            matchEnded = true;

        Debug.Log($"[GameManager] RPC_EndMatch received. Winner: {winnerName}");

        bool localWon = winnerName == PhotonNetwork.NickName;

        WinnerScreenUI.Instance?.ShowWinnerScreen(winnerName, localWon);
    }

    // ---------------------------
    // PLAYER LEAVE HANDLING
    // ---------------------------
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (matchEnded) return;

        Debug.Log($"[GameManager] Player left: {otherPlayer.NickName}");

        HandleDeath(otherPlayer.ActorNumber);
    }

    // ---------------------------
    // ROOM PROPERTY STORAGE
    // ---------------------------
    private int[] GetAlivePlayers()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(ALIVE_KEY, out object value))
        {
            return (int[])value;
        }

        return new int[0];
    }

    private void SetAlivePlayers(List<int> alive)
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
    {
        { ALIVE_KEY, alive.ToArray() }
    });
    }

    
}