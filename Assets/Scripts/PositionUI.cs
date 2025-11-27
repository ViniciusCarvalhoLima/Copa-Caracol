using TMPro;
using UnityEngine;

public class PositionUI : MonoBehaviour
{
    public TMP_Text positionText;
    public PositionHandler positionHandler;

    void Start()
    {
        UpdateUI();

        foreach (var car in positionHandler.carLapCounters)
            car.OnPassCheckpoint += OnCheckpoint;
    }

    private void OnCheckpoint(CarLapCounter car)
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (positionHandler.carLapCounters.Count < 2)
            return;

        positionText.text = "";

        int pos1 = positionHandler.carLapCounters[0].GetCarPosition();
        int pos2 = positionHandler.carLapCounters[1].GetCarPosition();

        // exibe apenas 1st e 2nd
        positionText.text = $"{pos1}st\n{pos2}nd";
    }
}
