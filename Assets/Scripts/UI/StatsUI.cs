using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text deathsText;
    [SerializeField] private TMP_Text kdText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text matchesText;
    [SerializeField] private TMP_Text winsText;
    [SerializeField] private TMP_Text winRateText;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (usernameText) usernameText.text = UserProfile.GetUsername();
        if (killsText) killsText.text = $"Kills: {UserProfile.Kills}";
        if (deathsText) deathsText.text = $"Deaths: {UserProfile.Deaths}";
        if (kdText) kdText.text = $"K/D: {UserProfile.KDRatio:F2}";
        if (damageText) damageText.text = $"Damage: {UserProfile.DamageDealt}";
        if (matchesText) matchesText.text = $"Matches: {UserProfile.MatchesPlayed}";
        if (winsText) winsText.text = $"Wins: {UserProfile.Wins}";
        if (winRateText) winRateText.text = $"Win Rate: {UserProfile.WinRate:F1}%";
    }
}