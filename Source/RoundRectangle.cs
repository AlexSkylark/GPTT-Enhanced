using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssetPrefabs
{
    public class RoundRectangle : MonoBehaviour
    {
        public Image imageComponent;
        public TMPro.TextMeshProUGUI textObject;
        public RectTransform rectTransform;
        public RectTransform textRectTransform;

        // Start is called before the first frame update
        void Awake()
        {
            imageComponent = GetComponent<Image>();
            textObject = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            rectTransform = GetComponent<RectTransform>();
            textRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        }       
    }

}
