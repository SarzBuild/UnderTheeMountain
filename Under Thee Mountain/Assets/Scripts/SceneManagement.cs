using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void StartGame() => SceneManager.LoadScene("SampleScene");
    public void ExitGame() => SceneManager.LoadScene("__main__");
    public void ExitApp() => Application.Quit();
}
