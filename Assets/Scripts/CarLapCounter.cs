using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CarLapCounter : MonoBehaviour
{
    [Header("Configurações da Corrida")]
    [Tooltip("Número de voltas necessárias para completar a corrida.")]
    public int lapsToComplete = 4;

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
    private static Checkpoint[] allCheckpoints; // cache global para todos os carros (mais rápido)

    void Start()
    {
        InitializeCheckpoints();
    }

    void InitializeCheckpoints()
    {
        // Usa cache estático para evitar chamar FindObjectsByType em todos os carros
        if (allCheckpoints == null || allCheckpoints.Length == 0)
        {
            allCheckpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        }

        if (allCheckpoints == null || allCheckpoints.Length == 0)
        {
            Debug.LogError($"[{name}] Nenhum Checkpoint encontrado na cena. Adicione objetos com o script Checkpoint e tag \"CheckPoint\".");
            return;
        }

        totalCheckpoints = allCheckpoints.Length;

        // Tenta encontrar a linha de chegada (primeiro checkpoint com isFinishLine = true)
        var finish = allCheckpoints.FirstOrDefault(c => c.isFinishLine);
        if (finish != null)
        {
            finishIndex = finish.checkpointIndex;
        }
        else
        {
            finishIndex = 0;
            Debug.LogWarning($"[{name}] Nenhuma linha de chegada marcada. Assumindo checkpointIndex 0 como linha de chegada.");
        }

        // Começa imediatamente antes da linha de chegada
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
            Debug.LogWarning($"[{name}] Trigger com tag CheckPoint, mas sem Checkpoint.cs no objeto.");
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

                if (LapsCompleted >= lapsToComplete)
                {
                    IsRaceCompleted = true;
                    Debug.Log($"[{name}] Corrida COMPLETA! ({LapsCompleted} voltas)");
                }
            }

            // Notifica o PositionHandler
            OnPassCheckpoint?.Invoke(this);

            Debug.Log($"[{name}] Passou Checkpoint {cp.checkpointIndex} (finish={cp.isFinishLine}) | Volta {LapsCompleted}/{lapsToComplete}");
        }
        else
        {
            Debug.Log($"[{name}] Passou checkpoint {cp.checkpointIndex} fora da ordem (esperava {expectedNext}). Ignorando.");
        }
    }
}
