using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("References")]
    [SerializeField] private StatsUI statsUI;
    [SerializeField] private TMP_Text welcomeText;

    [Header("Play Button")]
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_Text playButtonText;
    [SerializeField] private string playText = "PLAY";
    [SerializeField] private string noConnectionText = "NO CONNECTION";

    [Header("Scene Names")]
    [SerializeField] private string lobbySceneName = "LobbyScene";

    private void Start()
    {
        if (UserProfile.HasProfile())
            welcomeText.text = $"Welcome, {UserProfile.GetUsername()}!";

        ShowMainMenu();
        CheckConnection();
        InvokeRepeating(nameof(CheckConnection), 1f, 2f);
    }

    private void OnEnable()
    {
        // re-check whenever menu becomes active (e.g. returning from lobby)
        CheckConnection();
    }

    private void CheckConnection()
    {
        bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;

        if (playButton != null)
            playButton.interactable = hasInternet;

        if (playButtonText != null)
            playButtonText.text = hasInternet ? playText : noConnectionText;
    }

    // ── Navigation ─────────────────────────────────────

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        statsPanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        CheckConnection();
    }

    public void ShowStats()
    {
        mainMenuPanel.SetActive(false);
        statsPanel.SetActive(true);
        statsUI.Refresh();
    }

    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    // ── Buttons ────────────────────────────────────────

    public void OnPlayPressed()
    {
        // double check right before attempting to connect
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            CheckConnection();
            return;
        }

        if (!PhotonNetwork.IsConnected)
            NetworkManager.Instance.Connect();

        SceneManager.LoadScene(lobbySceneName);
    }

    public void OnStatsPressed() => ShowStats();
    public void OnSettingsPressed() => ShowSettings();
    public void OnBackPressed() => ShowMainMenu();

    public void OnQuitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}