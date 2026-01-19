using UnityEngine;

public class CameraModeSelector : MonoBehaviour
{
    [SerializeField] private MonoBehaviour ModoStandard;
    [SerializeField] private MonoBehaviour CameraElimination;

    void Awake()
    {
        string modo = PlayerPrefs.GetString("ModoSelecionado", "Standard");

        // Desativa ambos primeiro
        ModoStandard.enabled = false;
        CameraElimination.enabled = false;

        // Ativa o modo correto
        if (modo == "Clássico")
        {
            ModoStandard.enabled = true;
        }
        else if (modo == "Eliminação")
        {
            CameraElimination.enabled = true;
        }

        Debug.Log("Modo de jogo ativo: " + modo);
    }
}
