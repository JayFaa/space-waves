using UnityEngine;

public class ChoiceElement : MonoBehaviour
{
    private GameObject _choiceInstance;

    public void SetNodeEffect(GameObject choicePrefab)
    {
        _choiceInstance = Instantiate(choicePrefab, transform);
        _choiceInstance.GetComponent<Node>().SetIsChoiceNode(true);
        _choiceInstance.GetComponent<Node>().SetNodeEffect(_choiceInstance.GetComponent<PurchaseableNode>());
    }
}
