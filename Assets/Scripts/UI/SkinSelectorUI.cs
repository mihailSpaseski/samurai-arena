using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SkinSelectorUI : MonoBehaviour
{
    [SerializeField] private Transform listParent; // Vertical Layout Group
    [SerializeField] private GameObject skinEntryPrefab;
    [SerializeField] private SkinDatabase skinDatabase;
    [SerializeField] private GameObject skinSelectorPanel;
    [SerializeField] private GameObject lobbyPanel;

    private List<SkinEntryUI> entries = new();

    private void OnEnable()
    {
        BuildList();
    }

    private void BuildList()
    {
        foreach (Transform child in listParent)
            Destroy(child.gameObject);

        entries.Clear();

        SkinData[] skins = skinDatabase.GetAllSkins();
        int selectedID = UserProfile.SelectedSkinID;

        foreach (SkinData skin in skins)
        {
            GameObject entryObj = Instantiate(skinEntryPrefab, listParent);
            SkinEntryUI entry = entryObj.GetComponent<SkinEntryUI>();

            entry.Setup(skin, skin.skinID == selectedID, OnSkinSelected);
            entries.Add(entry);
        }
    }

    private void OnSkinSelected(int skinID)
    {
        UserProfile.SelectedSkinID = skinID;

        // refresh highlights
        foreach (SkinEntryUI entry in entries)
            entry.SetSelected(entry.SkinID == skinID);
    }

    public void openSkinSelector()
    {
        lobbyPanel.SetActive(false);
        skinSelectorPanel.SetActive(true);
    }

     public void closeSkinSelector()
    {
        lobbyPanel.SetActive(true);
        skinSelectorPanel.SetActive(false);
    }
}