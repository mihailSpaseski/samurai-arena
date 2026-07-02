using UnityEngine;

public static class UserProfile
{
    // keys
    private const string KEY_USERNAME = "username";

    private static string Username => PlayerPrefs.GetString(KEY_USERNAME, "");

    // stat keys prefixed by username
    private static string Key(string stat) => $"{Username}_{stat}";

    private const string KEY_SKIN = "selected_skin";

    // ── Setup ──────────────────────────────────────────

    public static bool HasProfile() =>
        !string.IsNullOrEmpty(PlayerPrefs.GetString(KEY_USERNAME, ""));

    public static void CreateProfile(string username)
    {
        PlayerPrefs.SetString(KEY_USERNAME, username);
        PlayerPrefs.Save();
    }

    public static string GetUsername() =>
        PlayerPrefs.GetString(KEY_USERNAME, "Player");

    // ── Stats ──────────────────────────────────────────

    public static int Kills => PlayerPrefs.GetInt(Key("kills"), 0);
    public static int Deaths => PlayerPrefs.GetInt(Key("deaths"), 0);
    public static int DamageDealt => PlayerPrefs.GetInt(Key("damage"), 0);
    public static int MatchesPlayed => PlayerPrefs.GetInt(Key("matches"), 0);
    public static int Wins => PlayerPrefs.GetInt(Key("wins"), 0);
    public static float WinRate => MatchesPlayed == 0 ? 0f :
        (float)Wins / MatchesPlayed * 100f;
    public static float KDRatio => Deaths == 0 ? Kills :
        (float)Kills / Deaths;

    public static void AddKills(int amount)
    {
        PlayerPrefs.SetInt(Key("kills"), Kills + amount);
        PlayerPrefs.Save();
    }

    public static void AddDeath()
    {
        PlayerPrefs.SetInt(Key("deaths"), Deaths + 1);
        PlayerPrefs.Save();
    }

    public static void AddDamage(int amount)
    {
        PlayerPrefs.SetInt(Key("damage"), DamageDealt + amount);
        PlayerPrefs.Save();
    }

    public static void AddMatch(bool won)
    {
        PlayerPrefs.SetInt(Key("matches"), MatchesPlayed + 1);
        if (won) PlayerPrefs.SetInt(Key("wins"), Wins + 1);
        PlayerPrefs.Save();
    }

    public static int SelectedSkinID
    {
        get => PlayerPrefs.GetInt(Key("skin"), 0);
        set
        {
            PlayerPrefs.SetInt(Key("skin"), value);
            PlayerPrefs.Save();
        }
    }
}