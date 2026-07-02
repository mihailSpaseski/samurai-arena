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
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject swipeArea;
    [SerializeField] private GameObject dashIndicator;

    [Header("Spectate")]
    [SerializeField] private SpectatorCamera spectatorCamera;
    [SerializeField] private GameObject exitSpectate;

    public static DeathScreenUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        deathPanel.SetActive(false);
    }

    public void ShowDeathScreen(int damageDealt, int kills)
    {
        // save to persistent profile
        UserProfile.AddDeath();
        UserProfile.AddKills(kills);
        UserProfile.AddDamage(damageDealt);
        UserProfile.AddMatch(false); // died = didn't win
        Debug.Log("DMG: " + damageDealt);

        deathPanel.SetActive(true);
        damageText.text = $"Damage Dealt: {damageDealt}";
        killsText.text = $"Kills: {kills}";
    }

    public void OnSpectatePressed()
    {
        deathPanel.SetActive(false);
        joystick.SetActive(false);
        swipeArea.SetActive(false);
        dashIndicator.SetActive(false);
        exitSpectate.SetActive(true);
        spectatorCamera.gameObject.SetActive(true);
        spectatorCamera.EnableSpectator();
    }

    public void OnExitPressed()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }

    public void HideDeathScreen()
    {
        deathPanel.SetActive(false);
    }
}