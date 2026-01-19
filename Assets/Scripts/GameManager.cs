using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public enum RaceMode
    {
        StandardRace,
        Elimination
    }

    public static GameManager Instance;

    [Header("Modo de Jogo")]
    public RaceMode raceMode = RaceMode.StandardRace;

    [Header("UI")]
    [SerializeField] private GameObject postRaceUI;

    [Header("Corrida")]
    private bool raceStarted = false;

    private List<CarLapCounter> activeCars = new List<CarLapCounter>();
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float countdownTime = 3f;
    private bool raceEnded = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        activeCars.AddRange(FindObjectsOfType<CarLapCounter>());
        StartCoroutine(CountdownCoroutine());
    }

    void OnEnable()
    {
        CameraElimination.OnCarEliminated += HandleCarEliminated;
    }

    void OnDisable()
    {
        CameraElimination.OnCarEliminated -= HandleCarEliminated;
    }

    IEnumerator CountdownCoroutine()
    {
        Time.timeScale = 0f;
        countdownPanel.SetActive(true);

        float timeLeft = countdownTime;

        while (timeLeft > 0)
        {
            countdownText.text = Mathf.Ceil(timeLeft).ToString();
            yield return new WaitForSecondsRealtime(1f);
            timeLeft--;
        }

        countdownText.text = "VAI!";
        yield return new WaitForSecondsRealtime(0.5f);

        countdownPanel.SetActive(false);
        Time.timeScale = 1f;

        raceStarted = true; // ✅ A CORRIDA COMEÇOU
    }



    // ======================
    // EVENTOS
    // ======================

    void HandleCarEliminated(CarLapCounter car)
    {
         if (!raceStarted || raceEnded) return;

        if (activeCars.Contains(car))
            activeCars.Remove(car);

        Debug.Log($"GameManager: {car.name} eliminado");

        if (raceMode == RaceMode.Elimination)
        {
            CheckEliminationWinCondition();
        }
    }

    // ======================
    // MODO ELIMINAÇÃO
    // ======================

    void CheckEliminationWinCondition()
    {
        if (activeCars.Count <= 1)
        {
            Debug.Log("🏆 Último carro restante!");
            EndRace(activeCars.Count == 1 ? activeCars[0] : null);
        }
    }

    // ======================
    // MODO CORRIDA PADRÃO
    // ======================

    public void OnCarFinishedRace(CarLapCounter car)
    {
         if (!raceStarted || raceEnded) return;

        Debug.Log($"🏁 {car.name} terminou a corrida");
        EndRace(car);
    }

    // ======================
    // FINALIZAÇÃO
    // ======================

    void EndRace(CarLapCounter winner)
    {
        raceEnded = true;

        Debug.Log(
            winner != null
                ? $"🏆 Vencedor: {winner.name}"
                : "🏁 Corrida encerrada"
        );

        ShowPostRaceUI(winner);
    }

    void ShowPostRaceUI(CarLapCounter winner)
    {
        StopAllCoroutines();
        if (postRaceUI != null)
            postRaceUI.SetActive(true);

        Time.timeScale = 0f;
    }

    // ======================
    // BOTÕES
    // ======================

    public void RestartRace()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
