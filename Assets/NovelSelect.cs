using UnityEngine;
using UnityEngine.UI;
using com.ondad.alertpanels;

public class NovelSelect : MonoBehaviour
{
    [SerializeField] private Button ElFili;
    [SerializeField] private GameObject lockSprite;
    [SerializeField] private AudioSource adsrc;
    
    private void OnEnable()
    {
        string gradeLvl = MyData.Load().grade_lvl;

        if(string.IsNullOrEmpty(gradeLvl) || gradeLvl != "Grade - 10")
        {

            //clear event listener of button
            ElFili.onClick = new Button.ButtonClickedEvent();

            //register Listener
            ElFili.onClick.AddListener(()=> {
                AlertManager.GetInstance().ShowInfoPanel("You Must Be Grade - 10 to unlock this Level");
            });

            ElFili.onClick.AddListener(() => adsrc.Play());

            lockSprite.SetActive(true);
        }
    }
}
