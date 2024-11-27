using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
public class RDViewCenterer : MonoBehaviour
{
    private ScrollRect techTreeScrollRect;
    private RectTransform techTreeContent;

    void Start()
    {
        // Hook into the R&D complex opening event
        GameEvents.onGUIRnDComplexSpawn.Add(OnRnDOpened);
    }

    void OnDestroy()
    {
        // Unhook the event when the mod is unloaded
        GameEvents.onGUIRnDComplexSpawn.Remove(OnRnDOpened);
    }

    private async void OnRnDOpened()
    {
        Debug.Log("[RDViewCenterer] R&D Center Opened");

        // Wait for the R&D UI to be ready
        await WaitForRnDUI();

        // Locate the ScrollRect and its content
        GameObject rndUI = GameObject.Find("_UIMaster/MainCanvas/ResearchAndDevelopment");
        techTreeScrollRect = rndUI?.GetComponentInChildren<ScrollRect>();
        techTreeContent = techTreeScrollRect?.content;

        if (techTreeScrollRect == null || techTreeContent == null)
        {
            Debug.LogError("[RDViewCenterer] Failed to find tech tree ScrollRect or content.");
            return;
        }

        // Wait for the tech tree to finish rendering
        await WaitForTechTreeRendering();

        // Center the view on the start node
        CenterViewOnStartNode();
    }

    private async Task WaitForRnDUI()
    {
        while (GameObject.Find("_UIMaster/MainCanvas/ResearchAndDevelopment") == null)
        {
            await Task.Delay(100); // Check every 100ms
        }

        Debug.Log("[RDViewCenterer] R&D UI is now ready.");
    }

    private async Task WaitForTechTreeRendering()
    {
        while (techTreeContent.Find("node0_start") == null)
        {
            await Task.Delay(100); // Check every 100ms
        }

        Debug.Log("[RDViewCenterer] Tech tree nodes are now rendered.");
    }

    private void CenterViewOnStartNode()
    {
        // Find the start node of the tech tree
        var objectTransform = techTreeContent.Find("node0_start");
        var rectTransform = objectTransform.Find("RDNodePrefab(Clone)").GetComponent<RectTransform>();

        Vector2 nodeLocalPosition = objectTransform.localPosition; // Local position within the content
        Vector2 nodePivotOffset = new Vector2(
            rectTransform.sizeDelta.x * rectTransform.pivot.x,
            rectTransform.sizeDelta.y * rectTransform.pivot.y
        );

        // Adjust the local position to the node's actual center
        Vector2 nodeCenterPosition = nodeLocalPosition + nodePivotOffset;

        // Normalize the position relative to the content and viewport
        Vector2 contentSize = techTreeContent.sizeDelta;
        Vector2 viewSize = techTreeScrollRect.GetComponent<RectTransform>().sizeDelta;

        Vector2 normalizedPosition = new Vector2(
            Mathf.Clamp01(1f - ((Mathf.Abs(nodeCenterPosition.x) - viewSize.x / 2) / (contentSize.x - viewSize.x))),
            Mathf.Clamp01((Mathf.Abs(nodeCenterPosition.y) - viewSize.y / 2) / (contentSize.y - viewSize.y))
        );

        techTreeScrollRect.normalizedPosition = normalizedPosition;

        Debug.Log($"[RDViewCenterer] View centered on the Start node. Normalized position: {techTreeScrollRect.normalizedPosition}");
    }
}
