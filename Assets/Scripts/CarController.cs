using UnityEngine;

[RequireComponent(typeof(Boost))]
public class CarController : MonoBehaviour
{
    public enum ControlScheme { Player1, Player2 }

    [Header("Configuração de Controle")]
    public ControlScheme controlScheme = ControlScheme.Player1;

    [Header("Configuração de Movimento")]
    public float AccelerationFactor = 10.0f;
    public float BrakeFactor = 1f;

    public float TurnFactor = 3.5f;
    public float DriftFactor = 0.05f;
    public float MaxSpeed = 12;

    float AccelerationInput = 0;
    float SteeringInput = 0;
    float RotationAngle = 0;
    float VelocityVsUp = 0;

    public Rigidbody2D rb;
    Boost boost;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        RotationAngle = rb.rotation;
        boost = GetComponent<Boost>();
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        ApplyEngineForce();
        OrthogonalVelocity();
        ApplySteering();
    }

    void HandleInput()
    {
        // Reseta input antes de ler
        AccelerationInput = 0;
        SteeringInput = 0;

        if (controlScheme == ControlScheme.Player1)
        {
            // Player 1 → WASD
            if (Input.GetKey(KeyCode.W)) AccelerationInput = 1;
            else if (Input.GetKey(KeyCode.S)) AccelerationInput = -1;

            if (Input.GetKey(KeyCode.A)) SteeringInput = -1;
            else if (Input.GetKey(KeyCode.D)) SteeringInput = 1;
        }
        else if (controlScheme == ControlScheme.Player2)
        {
            // Player 2 → Setas
            if (Input.GetKey(KeyCode.UpArrow)) AccelerationInput = 1;
            else if (Input.GetKey(KeyCode.DownArrow)) AccelerationInput = -1;

            if (Input.GetKey(KeyCode.LeftArrow)) SteeringInput = -1;
            else if (Input.GetKey(KeyCode.RightArrow)) SteeringInput = 1;
        }
    }

    void ApplyEngineForce()
{
    VelocityVsUp = Vector2.Dot(transform.up, rb.linearVelocity);

    if (VelocityVsUp > MaxSpeed && AccelerationInput > 0)
        return;
    if (VelocityVsUp < -MaxSpeed * 0.5f && AccelerationInput < 0)
        return;
    if (rb.linearVelocity.sqrMagnitude > MaxSpeed * MaxSpeed && AccelerationInput > 0)
        return;

    if (Mathf.Approximately(AccelerationInput, 0))
        rb.linearDamping = Mathf.Lerp(rb.linearDamping, 1.5f, Time.fixedDeltaTime * 3);
    else
        rb.linearDamping = 0;

    float appliedFactor = AccelerationInput > 0
        ? AccelerationFactor       
        : BrakeFactor;            

    Vector2 engineForceVector =
        transform.up * AccelerationInput * appliedFactor;

    rb.AddForce(engineForceVector, ForceMode2D.Force);
}


    void ApplySteering()
    {
        // Calcula rotação baseado na velocidade
        float minSpeedTurning = rb.linearVelocity.magnitude / 8;
        minSpeedTurning = Mathf.Clamp01(minSpeedTurning);

        // Determina se o carro está indo para frente ou para trás
        float direction = Mathf.Sign(Vector2.Dot(rb.linearVelocity, transform.up));

        // Inverte o sentido da rotação se estiver de ré
        RotationAngle -= SteeringInput * TurnFactor * minSpeedTurning * direction;

        rb.MoveRotation(RotationAngle);
    }

    public void OrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.linearVelocity, transform.right);

        rb.linearVelocity = forwardVelocity + rightVelocity * DriftFactor;
    }
}
