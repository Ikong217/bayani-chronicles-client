using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SkipLevel : MonoBehaviour
{
    [SerializeField] private GameObject questionHandler;
    [SerializeField] private ObjectsContainer activateObjects;
    [SerializeField] private ObjectsContainer removeObjects;

    private bool toggleFinish = false;
    private void Start()
    {
        toggleFinish = false;
        Novels novel = QuestionsRequestHandler.Instance.novel;
        string levelname = SceneManager.GetActiveScene().name;

        PlayerNovels currentLevel = PlayerLevelsManager.Load(novel);
        try
        {
            if(currentLevel.FindLevel(levelname).stars <= 0) MeDestroy();
        }catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    public void Skip()
    {
        if (!toggleFinish)
        {
            ObjectsHandler.ActivateObject(activateObjects);
            ObjectsHandler.DestroyObject(removeObjects,questionHandler);
            questionHandler.SetActive(true);
            toggleFinish = true;
            MeDestroy();
        }
        else
        {
            print("This Method Already used");
        }
    }

    private void MeDestroy() => Destroy(gameObject);
}
