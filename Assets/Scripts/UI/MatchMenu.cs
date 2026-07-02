using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class MatchMenu : MonoBehaviour
{
    [SerializeField] private GameObject matchMenuPanel;

    public void ShowMatchMenuPanel()
    {
        matchMenuPanel.SetActive(true);
    }

    public void CloseMatchMenuPanel()
    {
        matchMenuPanel.SetActive(false);
    }

    public void OnBackToMenuPressed()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }


}
