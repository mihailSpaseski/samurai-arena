using UnityEngine;

[CreateAssetMenu(fileName = "NewSkin", menuName = "Game/Skin Data")]
public class SkinData : ScriptableObject
{
    public string skinName;
    public Sprite thumbnail;
    public GameObject modelPrefab;
    public int skinID; // 0, 1, 2
}