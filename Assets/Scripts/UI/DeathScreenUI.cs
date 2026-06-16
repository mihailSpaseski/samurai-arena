using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class DeathScreenUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private GameObject spectateButton;
    [SerializeField] private GameObject exitButton;

    [Header("Spectate")]
    [SerializeField] private SpectatorCamera spectatorCamera;

    public static DeathScreenUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        deathPanel.SetActive(false);
    }

    public void ShowDeathScreen(int damageDealt, int kills)
    {
        deathPanel.SetActive(true);

        damageText.text = $"Damage Dealt: {damageDealt}";
        killsText.text = $"Kills: {kills}";
    }

    public void OnSpectatePressed()
    {
        deathPanel.SetActive(false);
        spectatorCamera.gameObject.SetActive(true);
        spectatorCamera.EnableSpectator();
    }

    public void OnExitPressed()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }
}