using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace com.ondad.alertpanels
{
    public class ButtonEffect : MonoBehaviour
    {
        private float bounceStrength = 0.5f; 

        private int tweenId;
        private float initScale;

        private void Start()
        {
            initScale = transform.localScale.x;

            EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };

            pointerEnterEntry.callback.AddListener((eventData) => { OnPointerEnter((PointerEventData)eventData); });

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };

            pointerExitEntry.callback.AddListener((eventData) => { OnPointerExit((PointerEventData)eventData); });

            eventTrigger.triggers.Add(pointerEnterEntry);
            eventTrigger.triggers.Add(pointerExitEntry);
        }

        private void OnDisable()
        {
            // Cancel any active tweens when the object is disabled
            LeanTween.cancel(tweenId);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            LeanTween.cancel(tweenId);
            // Create new tween with bounce effect
            tweenId = LeanTween.scale(gameObject, Vector3.one * AlertPanel_Config.Instance.alertConfig.uiButtonHoverScale, AlertPanel_Config.Instance.alertConfig.uiBtnAnimSpeed)
                .setEase(LeanTweenType.easeOutBounce) // Bounce effect when scaling up
                .setOvershoot(bounceStrength) // Adjust bounce intensity
                .id;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Cancel any existing tween
            LeanTween.cancel(tweenId);

            // Create new tween with bounce effect
            tweenId = LeanTween.scale(gameObject, Vector3.one * initScale, AlertPanel_Config.Instance.alertConfig.uiBtnAnimSpeed)
                .setEase(LeanTweenType.easeOutBounce) // Bounce effect when scaling down
                .setOvershoot(bounceStrength) // Adjust bounce intensity
                .id;
        }
    }
}
