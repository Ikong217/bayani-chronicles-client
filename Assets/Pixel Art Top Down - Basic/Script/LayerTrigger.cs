using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class LayerTrigger : MonoBehaviour
    {
        public string layer;
        public string sortingLayer;

        private void OnTriggerExit2D(Collider2D other)
        {
            int targetLayer = LayerMask.NameToLayer(layer);
            if (targetLayer != -1)
            {
                other.gameObject.layer = targetLayer;
            }
            else
            {
                Debug.LogError("Layer '" + layer + "' not found! Please check the layer name in the Inspector.");
            }

            if (other.gameObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer mainSR))
            {
                mainSR.sortingLayerName = sortingLayer;
            }

            SpriteRenderer[] srs = other.gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in srs)
            {
                sr.sortingLayerName = sortingLayer;
            }
        }
    }
}
