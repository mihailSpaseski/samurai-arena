using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SkinEntryUI : MonoBehaviour
{
    [SerializeField] private Image thumbnailImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject selectedHighlight;
    [SerializeField] private Button selectButton;

    public int SkinID { get; private set; }

    public void Setup(SkinData skin, bool isSelected, Action<int> onSelected)
    {
        SkinID = skin.skinID;
        thumbnailImage.sprite = skin.thumbnail;
        nameText.text = skin.skinName;

        SetSelected(isSelected);

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelected(SkinID));
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedHighlight != null)
            selectedHighlight.SetActive(isSelected);
    }
}