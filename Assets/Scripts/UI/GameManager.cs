using UnityEngine;
using System.Collections.Generic;

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
    private List<CarLapCounter> activeCars = new List<CarLapCounter>();
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
    }

    void OnEnable()
    {
        CameraElimination.OnCarEliminated += HandleCarEliminated;
    }

    void OnDisable()
    {
        CameraElimination.OnCarEliminated -= HandleCarEliminated;
    }

    // ======================
    // EVENTOS
    // ======================

    void HandleCarEliminated(CarLapCounter car)
    {
        if (raceEnded) return;

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
        if (raceEnded) return;

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
        if (postRaceUI != null)
            postRaceUI.SetActive(true);

        Time.timeScale = 0f; // pausa jogo
    }
}
