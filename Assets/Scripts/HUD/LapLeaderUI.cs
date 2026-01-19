using TMPro;
using UnityEngine;

public class LapLeaderUI : MonoBehaviour
{
    public PositionHandler positionHandler;
    public TMP_Text lapText;

    void Start()
    {
        UpdateLeaderLapText();

        foreach (var car in positionHandler.carLapCounters)
            car.OnPassCheckpoint += OnCheckpoint;
    }

    private void OnCheckpoint(CarLapCounter car)
    {
        UpdateLeaderLapText();
    }

    private void UpdateLeaderLapText()
    {
        if (positionHandler.carLapCounters == null || positionHandler.carLapCounters.Count == 0)
            return;

        CarLapCounter leader = positionHandler.carLapCounters[0];

        lapText.text = $"Lap {leader.LapsCompleted}/{leader.lapsToComplete}";
    }
}
