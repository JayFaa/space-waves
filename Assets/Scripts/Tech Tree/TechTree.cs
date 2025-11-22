using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TechTree : MonoBehaviour
{
    [SerializeField] List<Node> branchRoots;
    [SerializeField] RectTransform centerImage;
    [SerializeField] int arcSegments = 16;
    [SerializeField] int bufferPixels = 0;

    // Properties
    public float NodeScale { get; private set; }

    // Internal fields
    private Vector2 _previousScreenDimensions;
    private float _radius;

    // Cached components
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        _previousScreenDimensions = new(rectTransform.rect.width, rectTransform.rect.height);
        _radius = StretchedTreeRadius();
    }

    void Start()
    {
        ParseStructure();
        ArrangeTree();
        DrawTreeLines();
        UnlockRootNodes();
    }

    void LateUpdate()
    {
        // If a change in screen size is detected
        if (rectTransform.rect.width != _previousScreenDimensions.x || rectTransform.rect.height != _previousScreenDimensions.y)
        {
            float newRadius = StretchedTreeRadius();

            // If a meaningful change in screen size is detected, re-scale the tech tree
            if (newRadius != _radius)
            {
                _radius = newRadius;
                ArrangeTree();
                DrawTreeLines();
            }
            
            _previousScreenDimensions = new(rectTransform.rect.width, rectTransform.rect.height);
        }
    }

    private float StretchedTreeRadius()
    {
        return Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) / 2f;
    }

    private void ParseStructure()
    {
        // Let each branch analyze its structure to inform how to arrange it
        foreach (Node root in branchRoots)
        {
            root.ParseTreeStructure();
        }
    }

    private void ArrangeTree()
    {
        // If there are no branches, there's nothing to organize
        if (branchRoots.Count == 0) { return; }

        float totalBranches = branchRoots.Count;
        float tierHeight = _radius / (branchRoots.Select(branch => branch.Depth).Max() + 1);

        // Where the arc owned by this branch begins and ends, as a percentage (0 - 1)
        float branchDomainStart = 0f;
        float branchDomainEnd = branchDomainStart + 1f / totalBranches;

        // "0" is the positive X axis, so to allign the first branch with the positive Y axis we add a 1/4 rotation
        float offset = -(branchDomainEnd / 2f) + .25f;

        foreach (Node root in branchRoots)
        {
            root.Arrange(branchDomainStart + offset, branchDomainEnd + offset, tierHeight, tierHeight, Vector3.zero);
            branchDomainStart = branchDomainEnd;
            branchDomainEnd += 1 / totalBranches;
        }

        float maxNodeSize = branchRoots.Select(root => root.LargestPossibleNodeSize(bufferPixels)).Min();
        foreach (Node root in branchRoots)
        {
            centerImage.sizeDelta = new Vector2(maxNodeSize, maxNodeSize) * 2;
            root.Scale(maxNodeSize);
        }
    }

    private void DrawTreeLines()
    {
        foreach (Node root in branchRoots)
        {
            root.DrawLines(arcSegments, Vector2.zero);
        }
    }

    private void UnlockRootNodes()
    {
        foreach (Node root in branchRoots)
        {
            root.Unlock();
        }
    }
}
