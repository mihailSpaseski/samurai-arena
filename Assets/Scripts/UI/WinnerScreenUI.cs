using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class WinnerScreenUI : MonoBehaviour
{
    public static WinnerScreenUI Instance { get; private set; }

    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private GameObject backToMenuButton;

    private void Awake()
    {
        Instance = this;
        winnerPanel.SetActive(false);
    }

    public void ShowWinnerScreen(string winnerName, bool localPlayerWon)
    {
        winnerPanel.SetActive(true);

        winnerText.text = localPlayerWon
            ? "YOU WIN!"
            : $"{winnerName} WINS!";

        UserProfile.AddMatch(localPlayerWon);

        // hide death screen if it's showing
        if (DeathScreenUI.Instance != null)
            DeathScreenUI.Instance.HideDeathScreen();
    }

    public void OnBackToMenuPressed()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }
}