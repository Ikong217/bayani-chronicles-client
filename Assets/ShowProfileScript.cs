using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class ShowProfileScript : MonoBehaviour
{
    [Header("Character Sprite")]
    [SerializeField] Sprite maleCharacterSprite;
    [SerializeField] Sprite femaleCharacterSprite;


    [Header("Panel Component")]
    public Image characterImageContainer;
    public TextMeshProUGUI usernameTxt;
    public TextMeshProUGUI emailTxt;
    public TextMeshProUGUI gradeLevelTxt;
    public TextMeshProUGUI starsTxt;
    public TextMeshProUGUI scrollsTxt;
    public static ShowProfileScript Instance;

    //Drefault Parameters
    //[Header("Default Parameters")]
    //string maxStars = "20";
    //string maxScrolls = "20";

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        UpdateUserData();
    }

    public void UpdateUserData()
    {
        MyData data = MyData.Load();

        //change character sprie
        string gender = data.gender?.ToLower() ?? "";
        characterImageContainer.sprite =
            gender == "female" ? femaleCharacterSprite : maleCharacterSprite;

        //change username text
        string username = data.username ?? "No Username";
        usernameTxt.text = username;

        //change email text
        string email = data.email ?? "No Email";
        emailTxt.text = email;

        string grade_lvl = data.grade_lvl ?? "";
        string section_name = data.section_name ?? "";

        string grade_sec = string.Join("-", new[] { grade_lvl, section_name }
                                        .Where(s => !string.IsNullOrEmpty(s)));

        grade_sec = string.IsNullOrEmpty(grade_sec) ? "Not Yet Enrolled" : grade_sec;
        gradeLevelTxt.text = grade_sec;

        string stars = PlayerLevelsData.GetAllStars().ToString();
        string scrolls = ScrollInventoryContainer.Count().ToString();

        string maxStars = PlayerLevelsData.GetMaxStars().ToString();
        string maxScrolls = PlayerLevelsData.GetMaxScrolls().ToString();

        starsTxt.text = string.Format("{0}/{1}", stars, maxStars);
        scrollsTxt.text = string.Format("{0}/{1}", scrolls, maxScrolls);

    }

    public void Logout()
    {
        SceneManager.LoadScene("LoginScene");
    }
}
