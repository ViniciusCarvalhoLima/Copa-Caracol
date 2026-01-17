using UnityEngine;
using UnityEngine.SceneManagement;

public class TelaSeleção : MonoBehaviour
{
    [SerializeField] private GameObject TelaMapas;
    [SerializeField] private GameObject TelaModos;
    [SerializeField] private CanvasGroup TelaSeleçãoCanvasGroup;

    public void Play()
    {
        if (!PlayerPrefs.HasKey("MapaSelecionado"))
        {
            Debug.LogWarning("Nenhum mapa selecionado!");
            return;
        }

        string nomeMapa = PlayerPrefs.GetString("MapaSelecionado");

        Debug.Log("Carregando mapa: " + nomeMapa);

        SceneManager.LoadScene(nomeMapa);
    }

    public void Mapas()
    {
        BloquearMenu();
        TelaMapas.SetActive(true);
    }

    public void CloseMapas()
    {
        TelaMapas.SetActive(false);
        DesbloquearMenu();
    }

    public void Modos()
    {
        BloquearMenu();
        TelaModos.SetActive(true);
    }

    public void CloseModos()
    {
        TelaModos.SetActive(false);
        DesbloquearMenu();
    }

    public void Sair()
    {
        SceneManager.LoadScene("Menu");
    }

    public void MapaSelecionado(string nomeMapa)
    {
        PlayerPrefs.SetString("MapaSelecionado", nomeMapa);
        PlayerPrefs.Save();
        Debug.Log(PlayerPrefs.GetString("MapaSelecionado"));
    }

    public void ModoSelecionado(string nomeModo)
    {
        PlayerPrefs.SetString("ModoSelecionado", nomeModo);
        PlayerPrefs.Save();
        Debug.Log(PlayerPrefs.GetString("ModoSelecionado"));
    }

    void BloquearMenu()
    {
        TelaSeleçãoCanvasGroup.interactable = false;
        TelaSeleçãoCanvasGroup.blocksRaycasts = false;
    }

    void DesbloquearMenu()
    {
        TelaSeleçãoCanvasGroup.interactable = true;
        TelaSeleçãoCanvasGroup.blocksRaycasts = true;
    }
}
