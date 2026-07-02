using UnityEngine;

public class SkinDatabase : MonoBehaviour
{
    public static SkinDatabase Instance { get; private set; }

    [SerializeField] private SkinData[] allSkins;

    private void Awake()
    {
        Instance = this;
    }

    public SkinData[] GetAllSkins() => allSkins;

    public SkinData GetSkin(int id)
    {
        foreach (SkinData skin in allSkins)
            if (skin.skinID == id)
                return skin;

        return allSkins[0]; // fallback
    }
}