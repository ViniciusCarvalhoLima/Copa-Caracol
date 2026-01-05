using UnityEngine;
using UnityEngine.UI;


public class BoostBar : MonoBehaviour
{   

    [SerializeField] private Boost boost;
    private Image barraImage;

    private MaxBoost maxBoost;
    public void Awake()
    {
        barraImage = transform.Find("Barra")?.GetComponent<Image>();
        if (barraImage == null)
        {
            barraImage = GetComponentInChildren<Image>();
            if (barraImage == null)
                Debug.LogWarning($"BoostBar: 'Barra' child with Image component not found on '{name}'");
            else
                Debug.Log($"BoostBar: found Image on '{barraImage.gameObject.name}'");
        }

        // Try to resolve Boost automatically if not assigned in Inspector
        if (boost == null)
        {
            boost = GetComponent<Boost>() ?? GetComponentInParent<Boost>() ?? GetComponentInChildren<Boost>() ?? FindObjectOfType<Boost>();
            if (boost == null)
                Debug.LogWarning($"BoostBar: Boost reference is null on '{name}' - assign the Boost component in the Inspector or set it in code.");
            else
                Debug.Log($"BoostBar: resolved Boost on '{boost.gameObject.name}'");
        }

        maxBoost = new MaxBoost();
    }

    public class MaxBoost
    {
        public const float MXBoost = 0.83f;

        public float boostAmount = 0f;
        public float boostregenRate = 0.1f;
    }

    public void Update()
    {
        maxBoost.boostAmount += maxBoost.boostregenRate * Time.deltaTime;
        maxBoost.boostAmount = Mathf.Clamp(maxBoost.boostAmount, 0f, MaxBoost.MXBoost);
        if (barraImage != null)
            barraImage.fillAmount = maxBoost.boostAmount / MaxBoost.MXBoost;
        else
            Debug.LogWarning($"BoostBar: barraImage is null on '{name}' - ensure a child named 'Barra' with an Image component exists.");

        if (Input.GetKeyDown(KeyCode.LeftShift) && maxBoost.boostAmount >= MaxBoost.MXBoost)
        {
            if (boost != null)
                boost.ActivateBoost();
            else
                Debug.LogWarning($"BoostBar: Boost reference is null on '{name}' - assign the Boost component in the Inspector or set it in code.");
            maxBoost.boostAmount = 0f;
        }
    }

     public bool IsBoostFull()
        {
            if (maxBoost.boostAmount >= MaxBoost.MXBoost)
                return true;
            else
                return false;
        }

}
    