using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    // [SerializeField] private GameObject mainMenuPanel;
    // [SerializeField] private GameObject settingsPanel;

    [Header("Scene Names")]
    [SerializeField] private string lobbySceneName = "LobbyScene";

    private void Start()
    {
        // ShowMainMenu();
    }

    // public void ShowMainMenu()
    // {
    //     mainMenuPanel.SetActive(true);
    //     settingsPanel.SetActive(false);
    // }

    // 🔥 MAIN CHANGE: Go to Lobby instead of Game directly
    public void OnPlayPressed()
    {
        // Optional: ensure Photon starts connecting early
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        // Move to Lobby Scene (networking continues in background)
        SceneManager.LoadScene(lobbySceneName);
    }

    public void OnSettingsPressed()
    {
        // mainMenuPanel.SetActive(false);
        // settingsPanel.SetActive(true);
    }

    public void OnQuitPressed()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}