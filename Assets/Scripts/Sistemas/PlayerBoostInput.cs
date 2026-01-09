using UnityEngine;

[RequireComponent(typeof(Boost))]
public class PlayerBoostInput : MonoBehaviour
{
    [SerializeField] private KeyCode boostKey = KeyCode.LeftShift;
    [SerializeField] private BoostBar boostBar;

    private Boost boost;

    void Awake()
    {
        boost = GetComponent<Boost>();

        if (boostBar == null)
            Debug.LogError($"PlayerBoostInput: BoostBar não atribuído em {name}");
    }

void Update()
{
    if (Input.GetKeyDown(boostKey))
    {
        if (boostBar == null || boost == null)
            return;

        // 1️⃣ pergunta ao carro
        if (!boost.CanActivateBoost())
            return;

        // 2️⃣ tenta consumir a barra compartilhada
        if (!boostBar.TryConsumeBoost())
            return;

        // 3️⃣ ativa o boost (agora é garantido)
        boost.ActivateBoost();
    }
}
}