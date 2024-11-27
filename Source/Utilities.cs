using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GPTT
{

    [DefaultExecutionOrder(9999)]
    public class LostAndFoundNodeElement : MonoBehaviour
    {
        private RectTransform Anchor;
        private const int SPACER_MARGIN = 14;

        public void AnchorObject(Transform transformAnchor, int spacing)
        {
            Anchor = transformAnchor.GetComponent<RectTransform>();
        }

        void Update()
        {
            // Continuously reset the´position to follow the anchor
            if (Anchor != null)
            {
                transform.position = new Vector2(Anchor.position.x - (Anchor.sizeDelta.x / 2),
                    Anchor.position.y + (Anchor.sizeDelta.y / 2) + SPACER_MARGIN);
            }
        }
    }

    class Utilities
    {
        public static void DestroyObjectsWithComponent<T>(Transform parent) where T : Component
        {
            // Find all objects in the scene that have the specified component
            T[] objectsWithComponent = parent.GetComponentsInChildren<T>();
            Debug.Log($"[GPTT-Organizer] Found {objectsWithComponent.Length} GameObjects with component: {typeof(T).Name}.");

            // Iterate through each object and destroy its GameObject
            foreach (T obj in objectsWithComponent)
            {
                Debug.Log($"[GPTT-Organizer] Destroying GameObject: {obj.gameObject.name} with component: {typeof(T).Name}");
                GameObject.Destroy(obj.gameObject);
            }
        }

        public static string FindPartMod(AvailablePart part)
        {
            var configs = GameDatabase.Instance.GetConfigs("PART");

            UrlDir.UrlConfig config = Array.Find<UrlDir.UrlConfig>(configs, (c => (part.name == c.name.Replace('_', '.').Replace(' ', '.'))));
            if (config == null)
            {
                config = Array.Find<UrlDir.UrlConfig>(configs, (c => (part.name == c.name)));
                if (config == null)
                    return "";
            }
            var id = new UrlDir.UrlIdentifier(config.url);
            return id[0];
        }

        public static GameObject CreateHeaderPrefab(AssetBundle assetBundle, string modName)
        {
            GameObject header = GameObject.Instantiate(assetBundle.LoadAsset("round-rectangle") as GameObject);
            header.GetChild("text-object").AddComponent<TextMeshProUGUI>();
            var headerComp = header.AddComponent<AssetPrefabs.RoundRectangle>();

            headerComp.rectTransform.sizeDelta = new Vector2(263, 20);

            headerComp.imageComponent.pixelsPerUnitMultiplier = 3.5f;

            headerComp.textRectTransform.anchorMin = new Vector2(0, 0); // Stretch to fill the parent
            headerComp.textRectTransform.anchorMax = new Vector2(1, 1);
            headerComp.textRectTransform.sizeDelta = Vector2.zero; // No extra size
            headerComp.textRectTransform.pivot = new Vector2(0f, 0.5f); // Center pivot
            
            headerComp.textObject.text = modName.Contains("<UNKNOWN>") ? "Unknown Parts / Upgrades" : $"Parts from \"{modName}\"";
            headerComp.textObject.font = UISkinManager.TMPFont; // Use the KSP UI font
            headerComp.textObject.margin = new Vector4(6, 1, 0, 0); // Set text margins
            headerComp.textObject.overflowMode = TextOverflowModes.Truncate; // Prevent text overflow
            headerComp.textObject.enableWordWrapping = false; // Disable word wrapping
            headerComp.textObject.fontSize = 12;
            headerComp.textObject.fontWeight = 800;
            headerComp.textObject.alignment = TextAlignmentOptions.MidlineLeft; // Center align
            headerComp.textObject.color = Color.white;

            return header;
        }
    }
}
