using UnityEngine;
using UnityEngine.SceneManagement;

public class SummativeResume : MonoBehaviour
{
    private void Start()
    {
        SummativeDataLog data = SummativeDataLog.Load();
        //print(data.ToJson());

        if (data != null && string.IsNullOrEmpty(data.logs[^1].finishedTime))
        {
            print("pumasok");
            if(data.logs[^1].novel == Novels.NoliMeTangere)
            {
                SceneManager.LoadScene("Noli_Summative");
            }
            else
            {
                SceneManager.LoadScene("ElFi_Summative");
            }
        }
    }
}
