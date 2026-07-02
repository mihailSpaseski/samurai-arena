using UnityEngine;

public class PlayerModelSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] modelVariants; // index matches skinID

    private void Start()
    {
        ApplySkin(UserProfile.SelectedSkinID);
    }

    public void ApplySkin(int skinID)
    {
        for (int i = 0; i < modelVariants.Length; i++)
            modelVariants[i].SetActive(i == skinID);
    }
}