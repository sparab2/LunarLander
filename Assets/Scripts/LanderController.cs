using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LanderController : MonoBehaviour
{
    public float thrust = 1.0f;
    public float rotationSpeed = 100.0f;
    private Rigidbody2D rb;
    public float fuel = 100; // Initial fuel amount
    public TextMeshProUGUI fuelText; // UI Text element to display fuel
    public int score = 0;
    public TextMeshProUGUI scoreText; // UI Text element to display score
    private float startTime;
    public int baseScore = 1000;
    public float fuelUsedScorePenalty = 5.0f; // Points deducted per unit of fuel used

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startTime = Time.time; // Record the start time when game starts
        //score = baseScore; // Initialize score with baseScore
    }

    void Update()
    {
        // Check for remaining fuel before allowing thrust or rotation
        if (fuel > 0)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                rb.AddForce(transform.up * thrust);
                ConsumeFuel(0.6f); // Consume fuel when thrusting, adjust rate as needed
                IncreaseScore(0.1f);
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                rb.AddTorque(Input.GetKey(KeyCode.LeftArrow) ? rotationSpeed : -rotationSpeed);
                ConsumeFuel(0.3f); // Slightly less fuel consumption for rotation
                IncreaseScore(0.05f); // Smaller score increase for rotation
            }

            Debug.Log("Update running. Fuel: " + fuel); // This will show current fuel in the console
        }

        UpdateFuelText(); // Update the fuel text UI
        UpdateScoreText();
    }

    void ConsumeFuel(float amount)
    {
        // Reduces the fuel by the amount specified and clamps it at zero
        fuel -= amount;
        if (fuel < 0)
        {
            fuel = 0;
        }
        //score -= Mathf.RoundToInt(amount * 0.5f); // Slight penalty for fuel use, adjust the multiplier as needed
    }

    void UpdateFuelText()
    {
        if (fuelText != null) // Check if fuelText is not null
        {
            fuelText.text = "Fuel: " + fuel.ToString("F0");
        }
        else
        {
            Debug.LogError("Fuel Text not assigned in the Inspector");
        }
    }

    void CalculateScore(int landingBonus)
    {
        float endTime = Time.time; // Get the end time when landing is detected
        float timeTaken = endTime - startTime; // Calculate time taken
        int timeScore = Mathf.Max(0, 300 - Mathf.RoundToInt(timeTaken * 10)); // Deduct points based on time taken, 300 is just an example maximum

        // Calculate final score based on remaining fuel
        int fuelScore = Mathf.RoundToInt(fuel); // For example, each unit of fuel remaining could add a point to the score

        score += fuelScore + landingBonus + timeScore; // Include fuel score in the total

        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null) // Check if scoreText is not null
        {
            scoreText.text = "Score: " + score.ToString();
        }
        else
        {
            Debug.LogError("Score Text not assigned in the Inspector");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("LandingZone"))
        {
            float landingVelocity = rb.velocity.magnitude; // Check the speed at landing
            int landingBonus = (landingVelocity < 1.0f) ? 100 : 0; // Bonus for soft landing
            CalculateScore(landingBonus);
        }
    }
    void IncreaseScore(float amount)
    {
        score += Mathf.RoundToInt(amount); // Increase score by the specified amount
    }
}
