using UnityEngine;

public class ChoiceWindow : MonoBehaviour
{
    [SerializeField] ChoiceElement[] choiceElements;
    [SerializeField] TMPro.TMP_Text descriptionText;

    private Node _changingNode;

    void Awake()
    {
        choiceElements = GetComponentsInChildren<ChoiceElement>();
        SetDescription("");
        gameObject.SetActive(false);
    }

    public void SetDescription(string description)
    {
        descriptionText.text = description;
    }

    public void SetChangingNode(Node node)
    {
        _changingNode = node;
    }

    public void SetNodeEffectChoices(GameObject[] choicePrefabs)
    {
        for (int i = 0; i < choiceElements.Length && i < choicePrefabs.Length; i++)
        {
            choiceElements[i].SetNodeEffect(choicePrefabs[i]);
        }
    }

    public void MakeChoice(PurchaseableNode chosenEffect)
    {
        _changingNode.SetNodeEffect(chosenEffect);
        gameObject.SetActive(false);
    }
}
