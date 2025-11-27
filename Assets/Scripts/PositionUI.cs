using System.Collections;
using TMPro;
using UnityEngine;

public class PlayersPositionUI : MonoBehaviour
{
    [Header("Referências (pode deixar vazio para autodetectar)")]
    public CarLapCounter player1Car;
    public CarLapCounter player2Car;
    public PositionHandler positionHandler;

    [Header("TextMeshPro UI")]
    public TMP_Text player1Text; // top (player 1)
    public TMP_Text player2Text; // bottom (player 2)

    [Header("Ajustes")]
    public float waitTimeout = 2f; // tempo máximo para espera de inicialização

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

        // tenta encontrar PositionHandler se não foi atribuído
        while (positionHandler == null && t < waitTimeout)
        {
            positionHandler = FindObjectOfType<PositionHandler>();
            if (positionHandler != null) break;
            t += Time.deltaTime;
            yield return null;
        }

        if (positionHandler == null)
        {
            Debug.LogError("[PlayersPositionUI] PositionHandler não encontrado na cena.");
            yield break;
        }

        // espera um frame para garantir PositionHandler.Start() rodou
        yield return null;

        // tenta popular referências dos jogadores caso estejam vazias
        if (player1Car == null || player2Car == null)
        {
            // tenta pegar da lista do PositionHandler (ordem inicial pode ser qualquer)
            if (positionHandler.carLapCounters != null && positionHandler.carLapCounters.Count >= 2)
            {
                if (player1Car == null) player1Car = positionHandler.carLapCounters[0];
                if (player2Car == null) player2Car = positionHandler.carLapCounters[1];
            }
            else
            {
                // fallback: procura qualquer CarLapCounter na cena
                var found = FindObjectsOfType<CarLapCounter>();
                if (found.Length >= 2)
                {
                    if (player1Car == null) player1Car = found[0];
                    if (player2Car == null) player2Car = found[1];
                }
            }
        }

        // espera até PositionHandler ter carros (evita lista vazia por ordem de execução)
        t = 0f;
        while ((positionHandler.carLapCounters == null || positionHandler.carLapCounters.Count == 0) && t < waitTimeout)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // assina eventos e atualiza a UI inicial
        Subscribe();
        UpdateUI();
    }

    void Subscribe()
    {
        Unsubscribe();

        if (player1Car != null)
            player1Car.OnPassCheckpoint += OnAnyCheckpoint;

        if (player2Car != null)
            player2Car.OnPassCheckpoint += OnAnyCheckpoint;

        // subscribir também a todos os carros listados no PositionHandler (caso a posição mude por outro carro)
        if (positionHandler != null && positionHandler.carLapCounters != null)
        {
            foreach (var c in positionHandler.carLapCounters)
                c.OnPassCheckpoint += OnAnyCheckpoint;
        }
    }

    void Unsubscribe()
    {
        if (player1Car != null)
            player1Car.OnPassCheckpoint -= OnAnyCheckpoint;

        if (player2Car != null)
            player2Car.OnPassCheckpoint -= OnAnyCheckpoint;

        if (positionHandler != null && positionHandler.carLapCounters != null)
        {
            foreach (var c in positionHandler.carLapCounters)
                c.OnPassCheckpoint -= OnAnyCheckpoint;
        }
    }

    void OnAnyCheckpoint(CarLapCounter c)
    {
        // Quando qualquer carro passa um checkpoint, PositionHandler já terá recalculado
        // então atualizamos a UI com base nas posições atuais.
        UpdateUI();
    }

    void UpdateUI()
    {
        if (player1Text == null || player2Text == null)
        {
            Debug.LogError("[PlayersPositionUI] player1Text ou player2Text não atribuídos no Inspector.");
            return;
        }

        // se não tivermos PositionHandler ou lista, limpa a UI
        if (positionHandler == null || positionHandler.carLapCounters == null || positionHandler.carLapCounters.Count == 0)
        {
            player1Text.text = "";
            player2Text.text = "";
            return;
        }

        // obtém posição atual dos players (Setada pelo PositionHandler)
        int p1pos = player1Car != null ? player1Car.GetCarPosition() : -1;
        int p2pos = player2Car != null ? player2Car.GetCarPosition() : -1;

        // fallback: se GetCarPosition não estiver atualizado, determina por busca na lista ordenada
        if (p1pos <= 0 && player1Car != null)
        {
            int idx = positionHandler.carLapCounters.IndexOf(player1Car);
            if (idx >= 0) p1pos = idx + 1;
        }
        if (p2pos <= 0 && player2Car != null)
        {
            int idx = positionHandler.carLapCounters.IndexOf(player2Car);
            if (idx >= 0) p2pos = idx + 1;
        }

        // converte para "1st"/"2nd" (ou vazio se inválido)
        player1Text.text = (p1pos > 0) ? ToOrdinal(p1pos) : "";
        player2Text.text = (p2pos > 0) ? ToOrdinal(p2pos) : "";
    }

    // 1 -> "1st", 2 -> "2nd", 3->"3rd", others -> "4th"
    string ToOrdinal(int n)
    {
        if (n % 100 >= 11 && n % 100 <= 13) return n + "th";
        switch (n % 10)
        {
            case 1: return n + "st";
            case 2: return n + "nd";
            case 3: return n + "rd";
            default: return n + "th";
        }
    }
}
