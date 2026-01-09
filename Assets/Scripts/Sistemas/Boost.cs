using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(CarLapCounter))]
public class Boost : MonoBehaviour
{

    [SerializeField] private BoostBar boostBar;
    public float AccelerationBoost = 15f;
    public float SpeedBoost = 22f;
    public float BoostDuration = 2f;

    private CarController carController;
    private CarLapCounter carLapCounter;
    private PositionHandler positionHandler;
    private bool isBoostActive = false;
    private bool canActivateBoost = false;    

    public float cooldownTimeProgress = 0f;
    private Coroutine boostCooldownCoroutine;


void Update()
{
    if (boostBar == null) return;

    bool boostReady = boostBar.IsBoostFull();

    if (boostReady != canActivateBoost)
    {
        canActivateBoost = boostReady;

        if (canActivateBoost)
            Debug.Log("Boost is ready to use.");
        else
            Debug.Log("Boost is not ready yet.");
    }
}
    void Start()
    {
        carController = GetComponent<CarController>();
        carLapCounter = GetComponent<CarLapCounter>();
        positionHandler = FindObjectOfType<PositionHandler>();
    }

    public void ActivateBoost()
    {
        if (isBoostActive)
            return;

        int carPosition = carLapCounter.GetCarPosition();
        int totalCars = positionHandler.carLapCounters.Count;

        if (carPosition == totalCars)
        {
            StartCoroutine(BoostCoroutine());
        }
    }

    private IEnumerator BoostCoroutine()
    {
        isBoostActive = true;
        canActivateBoost = false;

        float originalMaxSpeed = carController.MaxSpeed;
        carController.MaxSpeed += SpeedBoost;
        carController.AccelerationFactor += AccelerationBoost;

        Debug.Log($"[{gameObject.name}] Speed boosted to {carController.MaxSpeed}");
        Debug.Log($"[{gameObject.name}] Acceleration boosted to {carController.AccelerationFactor}");

        yield return new WaitForSeconds(BoostDuration);

        carController.MaxSpeed = originalMaxSpeed;
        carController.AccelerationFactor -= AccelerationBoost;
        carController.rb.linearVelocity = Vector2.Min(carController.rb.linearVelocity, carController.rb.linearVelocity.normalized * originalMaxSpeed);
        isBoostActive = false;

        Debug.Log($"[{gameObject.name}] Speed returned to normal ({originalMaxSpeed})");
        Debug.Log($"[{gameObject.name}] Acceleration returned to normal ({carController.AccelerationFactor})");

        boostCooldownCoroutine = StartCoroutine(CooldownCoroutine(5f));
    }

    private IEnumerator CooldownCoroutine(float cooldownTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            cooldownTimeProgress = elapsedTime / cooldownTime;
            yield return null;
        }
        canActivateBoost = true;
    }

    public bool CanUseBoost()
{
    return canActivateBoost && !isBoostActive;
}

public bool CanActivateBoost()
{
    if (isBoostActive)
        return false;

    int carPosition = carLapCounter.GetCarPosition();
    int totalCars = positionHandler.carLapCounters.Count;

    return canActivateBoost && carPosition == totalCars;
}

}
