using UnityEngine;

[CreateAssetMenu(fileName = "NodeEffectSO", menuName = "Scriptable Objects/NodeEffectSO")]
public class NodeEffectSO : ScriptableObject
{
    public int price;
    public int maxPurchases;
    public Sprite iconSprite;
    public Color nodeColor;
    public float magnitude;
    public string description;
    public bool unlocksChildNodes;
}
