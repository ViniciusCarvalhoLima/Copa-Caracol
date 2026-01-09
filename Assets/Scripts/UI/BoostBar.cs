using UnityEngine;
using UnityEngine.UI;

public class BoostBar : MonoBehaviour
{
    [SerializeField] private Boost boost;
    private Image barraImage;

    private float boostAmount = 0f;
    private const float MaxBoost = 1f;
    [SerializeField] private float boostRegenRate = 0.1f;

    void Awake()
    {
        barraImage = transform.Find("Barra")?.GetComponent<Image>();
        if (barraImage == null)
            barraImage = GetComponentInChildren<Image>();

        if (boost == null)
            boost = GetComponentInParent<Boost>();
    }

    void Update()
    {
        boostAmount += boostRegenRate * Time.deltaTime;
        boostAmount = Mathf.Clamp(boostAmount, 0f, MaxBoost);

        if (barraImage != null)
            barraImage.fillAmount = boostAmount / MaxBoost;
    }

    // 👇 usados pelo carro
    public bool IsBoostFull()
    {
        return boostAmount >= MaxBoost;
    }

    public void ConsumeBoost()
    {
        boostAmount = 0f;
    }

    public bool TryConsumeBoost()
{
    if (boostAmount < MaxBoost)
        return false;

    boostAmount = 0f;
    return true;
}

}
