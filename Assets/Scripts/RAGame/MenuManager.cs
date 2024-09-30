using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LoadWhacAMole()
    {
        SceneManager.LoadScene("Whac-A-MoleManager");
    }
    public void LoadMemoria()
    {
        SceneManager.LoadScene("Memoria");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
