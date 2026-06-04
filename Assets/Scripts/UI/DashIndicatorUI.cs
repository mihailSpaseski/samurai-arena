using UnityEngine;
using UnityEngine.UI;

public class DashIndicatorUI : MonoBehaviour
{
    [SerializeField] private Image indicatorImage;
    [SerializeField] private Color readyColor = new Color(0.27f, 0.53f, 1f);
    [SerializeField] private Color cooldownColor = new Color(0.3f, 0.3f, 0.3f);
    [SerializeField] private float flashSpeed = 4f;

    private bool isReady;
    private bool wasReady;
    private float flashTimer;

    public void SetReady(bool ready)
    {
        isReady = ready;
    }

    private void Update()
    {
        if (isReady)
        {
            // just became ready — flash
            if (!wasReady)
            {
                flashTimer = 1f;
                wasReady = true;
            }

            if (flashTimer > 0)
            {
                flashTimer -= Time.deltaTime * flashSpeed;
                float t = Mathf.PingPong(flashTimer * flashSpeed, 1f);
                indicatorImage.color = Color.Lerp(readyColor, Color.white, t);
            }
            else
            {
                indicatorImage.color = readyColor;
            }
        }
        else
        {
            wasReady = false;
            indicatorImage.color = cooldownColor;
        }
    }
}