using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(CarLapCounter))]
public class Boost : MonoBehaviour
{

    [SerializeField] private BoostBar boostBar;
    public float SpeedBoost = 35f;
    public float BoostDuration = 2f;

    private CarController carController;
    private CarLapCounter carLapCounter;
    private PositionHandler positionHandler;
    private bool isBoostActive = false;
    private bool canActivateBoost = true;    

    public float cooldownTimeProgress = 0f;
    private Coroutine boostCooldownCoroutine;


    public void Update()
    {
         if(boostBar != null && boostBar.IsBoostFull())
        {
            canActivateBoost = true;
            print("Boost is ready to use.");
        }
        else
        {
            canActivateBoost = false;
            print("Boost is not ready yet.");
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

        Debug.Log($"[{gameObject.name}] Speed boosted to {carController.MaxSpeed}");

        yield return new WaitForSeconds(BoostDuration);

        carController.MaxSpeed = originalMaxSpeed;
        carController.rb.linearVelocity = Vector2.Min(carController.rb.linearVelocity, carController.rb.linearVelocity.normalized * originalMaxSpeed);
        isBoostActive = false;

        Debug.Log($"[{gameObject.name}] Speed returned to normal ({originalMaxSpeed})");

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
}
