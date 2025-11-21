using System.Linq;
using UnityEngine;

public class ChoiceNode : PurchaseableNode
{
    [SerializeField] GameObject[] choicePrefabs;

    private ChoiceWindow choiceWindow;

    protected override void Awake()
    {
        base.Awake();
        choiceWindow = FindFirstObjectByType<ChoiceWindow>(FindObjectsInactive.Include);
    }

    public override void OnPurchaseEffect()
    {
        // TODO: Make sure the underlying tech tree can't be interacted with while a choice is being made
        choiceWindow.SetNodeEffectChoices(GetThreeRandomChoices());
        choiceWindow.SetChangingNode(GetComponent<Node>());
        choiceWindow.gameObject.SetActive(true);
    }

    public GameObject[] GetThreeRandomChoices()
    {
        if (choicePrefabs.Length < 3)
        {
            Debug.LogWarning("Not enough available node effects to choose three from.");
        }
        return choicePrefabs.OrderBy(x => Random.value).Take(3).ToArray();
    }

}
