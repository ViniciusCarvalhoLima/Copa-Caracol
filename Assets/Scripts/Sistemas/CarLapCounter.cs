using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CarLapCounter : MonoBehaviour
{
    [Header("Configurações da Corrida")]
    public int lapsToComplete = 3;

    [Header("Estado Atual (somente leitura)")]
    [SerializeField] private int totalCheckpoints = 0;
    [SerializeField] private int finishIndex = 0;
    [SerializeField] private int currentCheckpointIndex = -1;

    public int LapsCompleted { get; private set; } = 0;
    public int CheckpointsPassedThisLap { get; private set; } = 0;
    public float TimeAtLastPassedCheckpoint { get; private set; } = 0f;
    public bool IsRaceCompleted { get; private set; } = false;

    // Evento disparado ao passar um checkpoint
    public event Action<CarLapCounter> OnPassCheckpoint;

    private int carPosition = 0;
    private static Checkpoint[] allCheckpoints;

    void Start()
    {
        InitializeCheckpoints();
    }

    void InitializeCheckpoints()
    {
        if (allCheckpoints == null || allCheckpoints.Length == 0)
        {
            allCheckpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        }

        totalCheckpoints = allCheckpoints.Length;

        // Tenta encontrar primeiro checkpoint com isFinishLine = true)
        var finish = allCheckpoints.FirstOrDefault(c => c.isFinishLine);
        if (finish != null)
        {
            finishIndex = finish.checkpointIndex;
        }
        currentCheckpointIndex = (finishIndex - 1 + totalCheckpoints) % totalCheckpoints;
        CheckpointsPassedThisLap = 0;
        LapsCompleted = 0;
        IsRaceCompleted = false;
    }

    public void SetCarPosition(int position)
    {
        carPosition = position;
    }

    public int GetCarPosition() => carPosition;
    public int GetNumberOfCheckpointsPassed() => CheckpointsPassedThisLap;
    public float GetTimeAtLastCheckpoint() => TimeAtLastPassedCheckpoint;
    public int GetLapsCompleted() => LapsCompleted;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsRaceCompleted) return;

        if (!other.CompareTag("CheckPoint")) return;

        Checkpoint cp = other.GetComponent<Checkpoint>();
        if (cp == null)
        {
            return;
        }

        if (totalCheckpoints == 0)
            InitializeCheckpoints();

        int expectedNext = (currentCheckpointIndex + 1) % totalCheckpoints;

        // Só aceita se for o próximo checkpoint da sequência
        if (cp.checkpointIndex == expectedNext)
        {
            currentCheckpointIndex = cp.checkpointIndex;
            CheckpointsPassedThisLap++;
            TimeAtLastPassedCheckpoint = Time.time;

            // Se for linha de chegada, incrementa volta
            if (cp.isFinishLine)
            {
                LapsCompleted++;
                CheckpointsPassedThisLap = 0; // reseta o contador de checkpoints da volta atual

                if (LapsCompleted > lapsToComplete)
                {
                    IsRaceCompleted = true;
                    GameManager.Instance.OnCarFinishedRace(this);
                    enabled = false;
                    Debug.Log($"[{name}] Corrida COMPLETA! ({lapsToComplete} voltas)");
                }
            }
            OnPassCheckpoint?.Invoke(this);
        }
    }
}
