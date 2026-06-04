using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private Color fullColor = new Color(0.8f, 0.13f, 0.13f);
    [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f);

    private Image[] segments;
    private Transform cameraTransform;
    private Transform segmentContainer; // ← add this

    private void Awake()
    {
        // grab the Canvas child as the container
        segmentContainer = GetComponentInChildren<Canvas>().transform;
    }

    public void Initialize(int maxHealth)
    {
        cameraTransform = Camera.main.transform;

        foreach (Transform child in segmentContainer)
            Destroy(child.gameObject);

        segments = new Image[maxHealth];

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject seg = Instantiate(segmentPrefab, segmentContainer); // ← use container
            segments[i] = seg.GetComponent<Image>();
            segments[i].color = fullColor;
        }
    }

    public void UpdateHealth(int current, int max)
    {
        for (int i = 0; i < segments.Length; i++)
            segments[i].color = i < current ? fullColor : emptyColor;
    }

    private void LateUpdate()
    {
        if (cameraTransform != null)
            transform.LookAt(transform.position + cameraTransform.forward); // ← correct billboard
    }
}