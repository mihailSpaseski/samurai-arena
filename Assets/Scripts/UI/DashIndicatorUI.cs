using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DashIndicatorUI : MonoBehaviour
{
    [SerializeField] private Image indicatorImage;
    [SerializeField] private TMP_Text indicatorText;
    [SerializeField] private Color readyColor = new Color(0.27f, 0.53f, 1f);
    [SerializeField] private Color cooldownColor = new Color(0.3f, 0.3f, 0.3f);

    private bool isReady = true;

    public void SetReadyTrue()
    {
        isReady = true;
    }
    public void SetReadyFalse()
    {
        isReady = false;
    }

    public void SetText(float x)
    {
        indicatorText.text = x.ToString("F2");
    }

    private void Update()
    {
        if(isReady)
        {
            indicatorImage.color = readyColor;
        }
        else
        {
            indicatorImage.color = cooldownColor;
        }
    }
}