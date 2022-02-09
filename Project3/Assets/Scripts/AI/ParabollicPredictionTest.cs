using UnityEngine;

public class ParabollicPredictionTest : MonoBehaviour
{

    static bool simulationDone = false;
    bool isMain = false;

    PrecalculatedPredictionSystem precalculatedPrediction;

    Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        if (simulationDone)
            return;

        isMain = true;
        simulationDone = true;
        rb = GetComponent<Rigidbody2D>();

        precalculatedPrediction = new PrecalculatedPredictionSystem(rb, 2f, 100, 1000,5f);

    }


    float currentDegree = 0f;

    // Update is called once per frame
    void Update()
    {
        if (!isMain)
            return;

        
        if (Input.GetKeyUp(KeyCode.Space))
            rb.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        int si = precalculatedPrediction.GetSimulationIndex(currentDegree);

        precalculatedPrediction.DrawTrajectory(si);


        currentDegree += Time.deltaTime * 90f;
        currentDegree %= 360f;

    }

}
