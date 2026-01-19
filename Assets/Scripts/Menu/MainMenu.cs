using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject telaControles;
    [SerializeField] private GameObject telaCreditos;
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;

    public void Play()
    {
        SceneManager.LoadScene("TelaDeSeleção");
    }

    public void Settings()
    {
        BloquearMenu();
        telaControles.SetActive(true);
    }

    public void CloseSettings()
    {
        telaControles.SetActive(false);
        DesbloquearMenu();
    }

    public void Credits()
    {
        BloquearMenu();
        telaCreditos.SetActive(true);
    }

    public void CloseCredits()
    {
        telaCreditos.SetActive(false);
        DesbloquearMenu();
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    void BloquearMenu()
    {
        mainMenuCanvasGroup.interactable = false;
        mainMenuCanvasGroup.blocksRaycasts = false;
    }

    void DesbloquearMenu()
    {
        mainMenuCanvasGroup.interactable = true;
        mainMenuCanvasGroup.blocksRaycasts = true;
    }
}
