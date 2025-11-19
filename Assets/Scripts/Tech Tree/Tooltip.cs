using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private float offsetRatio = 0.35f;
    [SerializeField] private TMPro.TextMeshProUGUI tooltipText;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string text)
    {
        tooltipText.text = text;
    }

    public void SetPosition(Vector2 nodeCanvasPosition, float nodeSize)
    {
        // Set pivot based on position to keep tooltip towards screen center
        _rectTransform.pivot = new Vector2(Mathf.Sign(nodeCanvasPosition.x) >= 0 ? 1 : 0, Mathf.Sign(nodeCanvasPosition.y) >= 0 ? 1 : 0);

        // Set anchored position to be away from the node position
        float offsetMagnitude = -offsetRatio * nodeSize;
        Vector2 offset = new(offsetMagnitude * Mathf.Sign(nodeCanvasPosition.x), offsetMagnitude * Mathf.Sign(nodeCanvasPosition.y));
        _rectTransform.anchoredPosition = nodeCanvasPosition + offset;
    }
}
