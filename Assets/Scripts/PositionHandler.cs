using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositionHandler : MonoBehaviour
{
    [Header("Referência dos Carros")]
    [Tooltip("Lista automática dos carros detectados na cena.")]
    public List<CarLapCounter> carLapCounters = new List<CarLapCounter>();

    void Start()
    {
        // Nova forma (Unity 2022+): busca todos os carros de forma rápida e não ordenada
        CarLapCounter[] foundCounters = FindObjectsByType<CarLapCounter>(FindObjectsSortMode.None);
        carLapCounters = foundCounters.ToList();

        // Conecta o evento de cada carro ao método de atualização de posição
        foreach (CarLapCounter lapCounter in carLapCounters)
        {
            lapCounter.OnPassCheckpoint += OnPassCheckpoint;
        }

        // Atualiza posições iniciais
        RecalculatePositions();
    }

    void OnDestroy()
    {
        // Remove os eventos ao destruir o objeto (evita memory leak)
        foreach (CarLapCounter lapCounter in carLapCounters)
        {
            if (lapCounter != null)
                lapCounter.OnPassCheckpoint -= OnPassCheckpoint;
        }
    }

    // Chamado sempre que um carro passa por um checkpoint
    void OnPassCheckpoint(CarLapCounter carLapCounter)
    {
        RecalculatePositions();

        // Atualiza a posição atual do carro individualmente
        int position = carLapCounters.IndexOf(carLapCounter) + 1;
        carLapCounter.SetCarPosition(position);

        Debug.Log($"[{carLapCounter.name}] Posição atual: {position}");
    }

    // Recalcula a ordem dos carros com base no progresso
    void RecalculatePositions()
    {
        carLapCounters = carLapCounters
            .OrderByDescending(c => c.GetLapsCompleted())              // quem tem mais voltas completas fica na frente
            .ThenByDescending(c => c.GetNumberOfCheckpointsPassed())  // depois quem passou mais checkpoints na volta atual
            .ThenBy(c => c.GetTimeAtLastCheckpoint())                 // quem passou o último checkpoint antes (menor tempo)
            .ToList();

        // Atualiza posição de todos os carros
        for (int i = 0; i < carLapCounters.Count; i++)
        {
            CarLapCounter counter = carLapCounters[i];
            if (counter != null)
                counter.SetCarPosition(i + 1);
        }
    }
}
