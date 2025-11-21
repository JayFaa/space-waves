using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] PurchaseableNode nodeEffect;
    [SerializeField] LineRenderer treeConnectionForwardsLR;
    [SerializeField] LineRenderer treeConnectionBackwardsLR;
    [SerializeField] LineRenderer treeArcLR;
    [SerializeField] List<Node> childNodes;

    [Header("Node State Colors")]
    [SerializeField] Color lockedColor = Color.grey;
    [SerializeField] Color unlockedColor = Color.white;
    [SerializeField] Color fullyPurchasedColor = Color.green;

    [Header("UI Elements")]
    [SerializeField] GameObject tooltipPrefab;
    [SerializeField] Image iconImageGO;
    [SerializeField] Image innerBackgroundImageGO;
    [SerializeField] Image outerBackgroundImageGO;
    [SerializeField] TMPro.TextMeshProUGUI priceTextGO;
    [SerializeField] TMPro.TextMeshProUGUI purchaseCountText;

    public int Width { get; private set; }
    public int Depth { get; private set; }
    public float DomainStart { get; private set; }
    public float DomainEnd { get; private set; }
    private float DomainCenter { get { return (DomainStart + DomainEnd) / 2f; } }
    public float Radius { get; private set; }
    public float TierHeight { get; private set; }

    private Canvas treeCanvas;
    private ChoiceWindow choiceWindow;

    private RectTransform _rTransform;
    private Tooltip _activeTooltip;
    private bool _locked = true;
    private bool _isChoiceNode = false;

    void Awake()
    {
        // Cache components
        choiceWindow = FindFirstObjectByType<ChoiceWindow>(FindObjectsInactive.Include);

        treeCanvas = GetComponentInParent<Canvas>(true);
        _rTransform = GetComponent<RectTransform>();

        // Initialize UI elements
        outerBackgroundImageGO.color = lockedColor;
        SetNodeEffectUI();
    }

    public void SetIsChoiceNode(bool isChoice)
    {
        Debug.Log($"Setting is choice node on {gameObject.name} to {isChoice}");
        _isChoiceNode = isChoice;
    }

    public void SetNodeEffect(PurchaseableNode effect)
    {
        nodeEffect = effect;
        SetNodeEffectUI();
        outerBackgroundImageGO.color = unlockedColor;
    }

    private void SetNodeEffectUI()
    {
        // Update UI elements
        innerBackgroundImageGO.color = nodeEffect.config.nodeColor;
        iconImageGO.sprite = nodeEffect.config.iconSprite;
        priceTextGO.text = nodeEffect.BuildPriceText();
        purchaseCountText.text = nodeEffect.BuildPurchaseCountText();
    }

    // Positions this node and its children according to the given domain
    public void Arrange(float domainStart, float domainEnd, float r, float tierHeight, Vector2 positionOffset)
    {
        // Cache positional values
        DomainStart = domainStart;
        DomainEnd = domainEnd;
        Radius = r;
        TierHeight = tierHeight;

        // Set this node's position to the middle of its domain at the given radial depth
        // Because child nodes inherit position from their parent, provide an offset to counter this
        _rTransform.anchoredPosition = PolarToCartesian(DomainCenter, Radius) - positionOffset;

        // If there are child nodes, arrange them and draw lines to connect them to the tree
        if (childNodes.Count > 0)
        {
            // If there are children, set their positions as well based on their subdomains
            int totalWidth = childNodes.Select(node => node.Width).Sum();
            float childDomainStart = DomainStart;
            float childDomainEnd;
            float domainSize = DomainEnd - DomainStart;
            foreach (Node child in childNodes)
            {
                childDomainEnd = childDomainStart + domainSize * child.Width / totalWidth;
                Debug.Log($"New child arranging: start {childDomainStart} end {childDomainEnd} radius {Radius + TierHeight}");
                child.Arrange(childDomainStart, childDomainEnd, Radius + TierHeight, TierHeight, positionOffset + _rTransform.anchoredPosition);
                childDomainStart = childDomainEnd;
            }
        }
    }

    public void DrawLines(int arcSegments, Vector2 treeCenterPosition)
    {

        // Update relative tree center position based on the current node's position
        treeCenterPosition -= _rTransform.anchoredPosition;

        // Connect this node to the tree
        DrawBackwardsTreeConnection(treeCenterPosition);

        // If there are children nodes, extend the tree forwards to give them a place to connect
        if (childNodes.Count > 0)
        {
            // Draw the arc following this node
            DrawTreeArc(arcSegments, treeCenterPosition);

            // Draw connections originating from the children
            foreach (Node child in childNodes)
            {
                child.DrawLines(arcSegments, treeCenterPosition);
            }
        }
    }

    private void DrawBackwardsTreeConnection(Vector2 treeCenterPosition)
    {
        DrawTreeConnection(treeConnectionBackwardsLR, treeCenterPosition, Radius - TierHeight / 2f);
    }

    private void DrawForwardsTreeConnection(Vector2 treeCenterPosition)
    {
        DrawTreeConnection(treeConnectionForwardsLR, treeCenterPosition, Radius + TierHeight / 2f);
    }

    private void DrawTreeConnection(LineRenderer lr, Vector2 treeCenterPosition, float r)
    {
        // Draw line from center of node to a point r directly towards or away from the tree's center
        Vector2 lineEnd = treeCenterPosition + PolarToCartesian(DomainCenter, r);

        lr.positionCount = 2;
        lr.SetPosition(0, new(0f, 0f, 0f));
        lr.SetPosition(1, new(lineEnd.x, lineEnd.y, 0f));
    }

    private void DrawTreeArc(int segments, Vector2 treeCenterPosition)
    {
        if (segments < 1)
        {
            Debug.LogWarning("At least one segment must be drawn to make an arc!");
            return;
        }

        // Only draw a connecting arc with more than one child node
        if (childNodes.Count > 1)
        {
            // Calculate arc parameters
            float start = childNodes.First().DomainCenter;
            float end = childNodes.Last().DomainCenter;
            float r = Radius + TierHeight / 2f;

            // Initialize start and end points
            Vector2[] arcPositions = new Vector2[segments + 1];
            arcPositions[0] = PolarToCartesian(start, r);
            arcPositions[segments] = PolarToCartesian(end, r);

            // Fill in middle points
            for (int i = 1; i < segments; i++)
            {
                float t = (float)i / segments;
                float angle = Mathf.Lerp(start, end, t);
                arcPositions[i] = PolarToCartesian(angle, r);
            }

            // Include draw depth and true center offsets on the calculated positions
            Vector3[] finalPositions =
                arcPositions
                    .Select(position => treeCenterPosition + position)
                    .Select(position => new Vector3(position.x, position.y, 0f))
                    .ToArray();

            treeArcLR.positionCount = arcPositions.Length;
            treeArcLR.SetPositions(finalPositions);
        }

        // Draw line forwards to connect following nodes, if there are any children
        if (childNodes.Count > 0)
        {
            DrawForwardsTreeConnection(treeCenterPosition);
        }

    }

    public void Scale(float size)
    {
        _rTransform.sizeDelta = new(size, size * (3f/2f)); // Slightly taller to accomodate additional UI elements

        if (childNodes.Count > 0)
        {
            foreach (Node child in childNodes)
            {
                child.Scale(size);
            }
        }
    }

    public float LargestPossibleNodeSize(float pixelBuffer)
    {
        // The max size might be determined by the distance between domain bounds at the given radius, or the distance between tiers along a single radius
        float maxSize = Mathf.Min(TierHeight / 2f, 2 * Mathf.PI * Radius * DomainEnd - DomainStart) - (pixelBuffer / 2f);

        if (maxSize <= 0f)
        {
            Debug.LogWarning("Node size is too small!");
            maxSize = 5f;
        }

        if (childNodes.Count == 0) { return maxSize; }
        else return Mathf.Min(maxSize, childNodes.Select(child => child.LargestPossibleNodeSize(pixelBuffer)).Min());
    }

    // This does not currently support a cyclical tech tree structure or multiple parents
    public void ParseTreeStructure()
    {
        // If this is a leaf node
        if (childNodes.Count == 0)
        {
            Width = 1;
            Depth = 1;
        }
        else
        {
            // Recursively find depth of node based on layers of children nodes, and how many leaf nodes exist as descendents (weight)
            foreach (Node childNode in childNodes)
            {
                childNode.ParseTreeStructure();
            }

            // This must consider what nodes have already been visited to support multiple parent nodes, otherwise width will be double counted
            Width = childNodes.Select(node => node.Width).Sum();

            Depth = childNodes.Select(node => node.Depth).Max() + 1;
        }
    }

    private Vector2 PolarToCartesian(float arcPercent, float r)
    {
        float angle = arcPercent * Mathf.PI * 2f;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Is choice node: {_isChoiceNode}");
        if (_isChoiceNode)
        {
            choiceWindow.SetDescription(nodeEffect.config.description);
        }
        else if (choiceWindow.gameObject.activeSelf == false)
        {
            // Only create tooltip if this is the node being hovered over, not a parent object
            if (this != eventData.pointerEnter.GetComponentInParent<Node>()) return;

            // Destroy any existing tooltip
            DestroyTooltip();

            // Create new tooltip
            _activeTooltip = Instantiate(tooltipPrefab, treeCanvas.transform).GetComponent<Tooltip>();
            _activeTooltip.SetText(nodeEffect.config.description);
            _activeTooltip.SetPosition(treeCanvas.transform.InverseTransformPoint(_rTransform.position), _rTransform.sizeDelta.y);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isChoiceNode)
        {
            choiceWindow.SetDescription("");
        }
        else
        {
            DestroyTooltip();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isChoiceNode)
        {
            choiceWindow.MakeChoice(nodeEffect);
        }
        else if (!_locked && choiceWindow.gameObject.activeSelf == false && nodeEffect.CanBePurchased())
        {
            nodeEffect.Purchase();
            purchaseCountText.text = nodeEffect.BuildPurchaseCountText();

            if (nodeEffect.PurchaseCount >= nodeEffect.config.maxPurchases)
            {
                outerBackgroundImageGO.color = fullyPurchasedColor;
            }

            if (nodeEffect.config.unlocksChildNodes)
            {
                foreach (Node child in childNodes)
                {
                    child.Unlock();
                }
            }
        }
    }

    private void DestroyTooltip()
    {
        if (_activeTooltip != null)
        {
            Debug.Log("Destroying tooltip");
            Destroy(_activeTooltip.gameObject);
        }
    }

    public void Unlock()
    {
        if (_locked)
        {
            _locked = false;
            outerBackgroundImageGO.color = unlockedColor;
        }
    }
}