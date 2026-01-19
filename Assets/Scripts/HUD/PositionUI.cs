using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlayersPositionUI : MonoBehaviour
{
    [Header("Referências de corrida")]
    public CarLapCounter player1Car;
    public CarLapCounter player2Car;
    public PositionHandler positionHandler;

    [Header("UI Images")]
    public Image player1Image;
    public Image player2Image;

    [Header("Sprites Player 1")]
    public Sprite player1Primeiro;
    public Sprite player1Segundo;

    [Header("Sprites Player 2")]
    public Sprite player2Primeiro;
    public Sprite player2Segundo;

    [Header("Ajustes")]
    public float waitTimeout = 2f;

    [SerializeField] private Vector2 normalSize = new Vector2(100, 100);
    [SerializeField] private Vector2 firstPlaceSize = new Vector2(130, 130);


    void OnEnable()
    {
        StartCoroutine(SetupRoutine());
    }

    void OnDisable()
    {
        Unsubscribe();
        StopAllCoroutines();
    }

    IEnumerator SetupRoutine()
    {
        float t = 0f;

        while (positionHandler == null && t < waitTimeout)
        {
            positionHandler = FindObjectOfType<PositionHandler>();
            t += Time.deltaTime;
            yield return null;
        }

        if (positionHandler == null)
        {
            Debug.LogError("[PlayersPositionUI] PositionHandler não encontrado.");
            yield break;
        }

        yield return null;

        if (positionHandler.carLapCounters.Count >= 2)
        {
            if (player1Car == null)
                player1Car = positionHandler.carLapCounters[0];

            if (player2Car == null)
                player2Car = positionHandler.carLapCounters[1];
        }

        Subscribe();
        UpdateUI();
    }

    void Subscribe()
    {
        Unsubscribe();

        foreach (var car in positionHandler.carLapCounters)
            car.OnPassCheckpoint += OnAnyCheckpoint;
    }

    void Unsubscribe()
    {
        if (positionHandler == null) return;

        foreach (var car in positionHandler.carLapCounters)
            car.OnPassCheckpoint -= OnAnyCheckpoint;
    }

    void OnAnyCheckpoint(CarLapCounter car)
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (player1Car == null || player2Car == null)
            return;

        int p1Pos = player1Car.GetCarPosition();
        int p2Pos = player2Car.GetCarPosition();

        // Player 1
        bool p1First = p1Pos == 1;
        SetImage(player1Image, p1First ? player1Primeiro : player1Segundo);
        SetImageSize(player1Image, p1First);

        // Player 2
        bool p2First = p2Pos == 1;
        SetImage(player2Image, p2First ? player2Primeiro : player2Segundo);
        SetImageSize(player2Image, p2First);
    }


    void SetImage(Image img, Sprite sprite)
    {
        if (img == null) return;

        img.sprite = sprite;
        img.enabled = sprite != null;
    }

    void SetImageSize(Image image, bool isFirst)
    {
        RectTransform rt = image.rectTransform;
        rt.sizeDelta = isFirst ? firstPlaceSize : normalSize;
    }

}
