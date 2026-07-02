using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ProfileSetupUI : MonoBehaviour
{
    [SerializeField] private GameObject setupPanel;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_Text errorText;

    private void Start()
    {
        // skip setup if profile already exists
        if (UserProfile.HasProfile())
        {
            setupPanel.SetActive(false);
            return;
        }

        setupPanel.SetActive(true);
    }

    public void OnConfirmPressed()
    {
        string username = usernameInput.text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            errorText.text = "Please enter a username.";
            return;
        }

        if (username.Length < 3)
        {
            errorText.text = "Username must be at least 3 characters.";
            return;
        }

        if (username.Length > 16)
        {
            errorText.text = "Username must be 16 characters or less.";
            return;
        }

        UserProfile.CreateProfile(username);

        StatsUI statsUI = FindFirstObjectByType<StatsUI>();
        if (statsUI != null)
            statsUI.Refresh();
            
        setupPanel.SetActive(false);
    }
}