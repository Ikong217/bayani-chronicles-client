using UnityEngine;

public class RunInBg : MonoBehaviour
{
    [SerializeField] private bool RunInBG = true;
    private void Awake()
    {
        Application.runInBackground = RunInBG;
    }
}
