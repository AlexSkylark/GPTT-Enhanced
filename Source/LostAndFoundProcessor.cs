using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using KSP.UI;
using KSP.UI.Screens;
using System.Collections;
using System;
using System.IO;

namespace GPTT
{   
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class LostAndFoundProcessor : MonoBehaviour
    {
        private bool isInitialized = false;
        private int headerSpacing = 24;
        GameObject generalPartsList;

        AssetBundle assetBundle;

        public void Start()
        {
            Debug.Log("[GPTT-Enhanced] Initialized.");

            if (assetBundle == null)
            {
                string bundlePath = Path.Combine(KSPUtil.ApplicationRootPath, "GameData/GPTT-Enhanced/Assets/prefabs.dat");
                assetBundle = AssetBundle.LoadFromFile(bundlePath);
            }            

            isInitialized = false;
        }

        public void Update()
        {
            var headerTextObject = GameObject.Find("_UIMaster/MainCanvas/ResearchAndDevelopment/ContentSpace/Panel TechTree/content_space/Panel_Right/Panel node/TopNodenameEtc/StratTextHeader/");
            if (ResearchAndDevelopment.Instance != null) {
                if (GameObject.Find("gptt_node_lostandfound") != null && headerTextObject != null)
                {
                    string headerText = headerTextObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
                    var panelNodeObj = headerTextObject.transform.parent.parent.gameObject;

                    if (panelNodeObj.activeSelf && headerText.ToUpper() == "LOST AND FOUND")
                    {
                        if (!isInitialized)
                        {
                            isInitialized = true;
                            ProcessLostAndFoundNode();
                        } else if (generalPartsList != null && generalPartsList.transform.childCount > 0 
                            && generalPartsList.transform.GetChild(0).name.ToUpper().Contains("DUMMY"))
                        {
                            Utilities.DestroyObjectsWithComponent<LostAndFoundNodeElement>(generalPartsList.transform.parent);
                            generalPartsList.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
                            ProcessLostAndFoundNode();
                        }
                    }
                    else
                    {
                        if (isInitialized)
                        {
                            Utilities.DestroyObjectsWithComponent<LostAndFoundNodeElement>(generalPartsList.transform.parent);                            
                            generalPartsList.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
                            isInitialized = false;
                        }
                    }
                }
            }
        }

        private void ProcessLostAndFoundNode()
        {
            Debug.Log("[GPTT-Enhanced] Processing gptt_lostandfound node...");

            // Locate the parts list in the UI hierarchy
            generalPartsList = GameObject.Find("_UIMaster/MainCanvas/ResearchAndDevelopment/ContentSpace/Panel TechTree/" +
                    "content_space/Panel_Right/Panel node/PartList/ListAndScrollbar/Panel/" +
                    "ScrollRect/PartList");

            // Group parts by mod in the "gptt_lostandfound" node
            var groupedParts = GroupPartsInNode("gptt_lostandfound");

            // Log results or prepare for UI customization
            foreach (var group in groupedParts)
            {
                if (group.Key.Contains("<UNKNOWN>"))
                {
                    Debug.Log($"[GPTT-Enhanced] Parts / upgrades without mod: {group.Value.Count}");
                    foreach (var part in group.Value)
                    {
                        Debug.Log($"[GPTT-Enhanced] Adding Unknown Part / Upgrade to Lost and Found node: {part.title}");
                    }
                } else
                {
                    Debug.Log($"[GPTT-Enhanced] Unsupported Mod: {group.Key}, Parts Count: {group.Value.Count}");
                    foreach (var part in group.Value)
                    {
                        Debug.Log($"[GPTT-Enhanced] Adding Part from mod \"{group.Key}\" to Lost and Found node: {part.title}");
                    }
                }
            }

            // You can call a UI customization function here if needed
            StartCoroutine(CustomizeTechTreeUI(groupedParts));
        }

        private Dictionary<string, List<AvailablePart>> GroupPartsInNode(string techID)
        {
            Dictionary<string, List<AvailablePart>> groupedParts = new Dictionary<string, List<AvailablePart>>();

            // Iterate over all loaded parts
            foreach (var part in generalPartsList.GetComponent<RDPartList>().listItems.Select(p => p.myPart))
            {
                // Check if the part belongs to the specified node
                if (part.TechRequired != techID)
                    continue;
                
                // Determine the mod origin or assign "Unknown Mod" if not specified
                string modOrigin = Utilities.FindPartMod(part);

                // Add part to the corresponding mod group
                if (!groupedParts.ContainsKey(modOrigin))
                {
                    groupedParts[modOrigin] = new List<AvailablePart>();
                }

                groupedParts[modOrigin].Add(part);
            }

            var unknownParts = generalPartsList.GetComponent<RDPartList>().listItems.Select(p => p.myPart)
                .Where(part => !groupedParts.Values.SelectMany(list => list).Contains(part));

            if (unknownParts.Count() > 0)
            {
                groupedParts["<UNKNOWN>"] = new List<AvailablePart>();
                foreach (var part in unknownParts)
                {
                    groupedParts["<UNKNOWN>"].Add(part);
                }
            }
            
            return groupedParts;
        }

        private IEnumerator CustomizeTechTreeUI(Dictionary<string, List<AvailablePart>> groupedParts)
        {
            // Log that UI customization is starting
            Debug.Log("[GPTT-Enhanced] Customizing the tech tree UI...");
           
            // Initialize sibling index (position in the list) and group index (used for spacing headers)
            int siblingIndex = 0;
            int groupIndex = 1;

            // Iterate through each mod group in the grouped parts dictionary
            foreach (var group in groupedParts)
            {                
                Debug.Log($"[GPTT-Enhanced] Adding header for mod group: {group.Key}");

                // Dynamically create a header GameObject for the mod group
                GameObject header = Utilities.CreateHeaderPrefab(assetBundle, group.Key);
                header.AddComponent<LostAndFoundNodeElement>();

                // Create a dummy object to use for padding between groups
                GameObject dummy = new GameObject("ModHeaderDummy");
                dummy.AddComponent<RectTransform>();
                dummy.AddComponent<LostAndFoundNodeElement>();

                // Retrieve and order parts in this group based on their mod of origin
                var orderedParts = generalPartsList.GetComponent<RDPartList>().listItems
                                        .Where(p => group.Value.Any(gp => gp.partUrl == p.myPart.partUrl)) // Filter parts belonging to the group
                                        .Select(pl => pl.transform.parent.gameObject)     // Get the parent GameObject of each part
                                        .OrderBy(go => go.GetComponentInChildren<RDPartListItem>().myPart.title) // Sort by part title
                                        .ToArray();

                // Add dummy objects until the sibling index aligns with the grid (multiple of 5)
                while (siblingIndex % 5 != 0)
                {
                    var newDummy = Instantiate(dummy); // Clone the dummy object
                    newDummy.transform.SetParent(generalPartsList.transform, false); // Add it to the parts list
                    newDummy.transform.SetSiblingIndex(siblingIndex); // Set its position in the hierarchy
                    siblingIndex++;
                }

                // Set the header's parent to the parts list's parent and position it at the top of the group's parts
                header.transform.SetParent(generalPartsList.transform.parent);
                header.transform.position = orderedParts[0].transform.position; // Position the header at the first part's location

                // Reposition the group's parts to follow the header and adjust their vertical position
                for (int i = siblingIndex; i < orderedParts.Length + siblingIndex; i++)
                {
                    orderedParts[i - siblingIndex].transform.SetSiblingIndex(i); // Set the part's new sibling index

                    // Adjust the position of key elements (like StateButton and Text) in each part
                    orderedParts[i - siblingIndex].GetChild("StateButton").transform.Translate(0, -headerSpacing * groupIndex, 0);
                    orderedParts[i - siblingIndex].GetChild("Text").transform.Translate(0, -headerSpacing * groupIndex, 0);
                }

                // Anchor the header to the first part's StateButton for alignment and spacing
                header.GetComponent<LostAndFoundNodeElement>().AnchorObject(orderedParts.First().GetChild("StateButton").transform, headerSpacing);

                // Increment the siblingIndex to account for the parts in this group
                siblingIndex += orderedParts.Length;

                // Increment the groupIndex for the next group (used for vertical spacing)
                groupIndex++;
            }            

            // Wait for one frame to allow the UI to update (if necessary)
            yield return null;
            
            // Adjust the content size fitter to stop constraining the vertical fit
            generalPartsList.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            // Adjust the size of the parts list to accommodate the additional headers and spacing
            var listRect = generalPartsList.GetComponent<RectTransform>();
            listRect.sizeDelta = new Vector2(
                        listRect.sizeDelta.x,
                        listRect.sizeDelta.y + (headerSpacing * (groupIndex - 1)) // Add spacing for each group
                    );
        }        
    }
}
