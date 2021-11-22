using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Vector2Extension
{

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);

        return v;
    }
}


public class ClassicPredictionSystem
{
    protected Rigidbody2D rb;
    protected Scene predictionScene;
    protected PhysicsScene2D predictionPhysicsScene;
    protected Rigidbody2D rbCopy;
    static int lastID = 0;

    public ClassicPredictionSystem(Rigidbody2D _rb)
    {
        rb = _rb;

        Physics.autoSimulation = false;

        CreateSceneParameters sceneParameters = new CreateSceneParameters(LocalPhysicsMode.Physics2D);
        predictionScene = SceneManager.CreateScene("PredictionTest" + lastID++, sceneParameters);
        predictionPhysicsScene = predictionScene.GetPhysicsScene2D();

        rbCopy = GameObject.Instantiate(rb.gameObject).GetComponent<Rigidbody2D>();
        SceneManager.MoveGameObjectToScene(rbCopy.gameObject, predictionScene);

    }

    public void CloseSimulation()
    {
        GameObject.Destroy(rbCopy.gameObject);

    }

    public List<Vector2> SimulateImpulse(Vector2 force, float time, int iterations, bool resetVelocity = false)
    {
        rbCopy.transform.position = rb.transform.position;
        if (resetVelocity)
            rbCopy.velocity = Vector2.zero;
        else
            rbCopy.velocity = rb.velocity;

        rbCopy.AddForce(force, ForceMode2D.Impulse);

        float dt = time / iterations;
        List<Vector2> result = new List<Vector2>();

        for (int i = 0; i < iterations; i++)
        {
            predictionPhysicsScene.Simulate(dt);
            result.Add(rbCopy.transform.position - rb.transform.position);
        }

        return result;
    }


}

public struct Rigidbody2DInfo
{
    public Vector2 position;
    public Vector2 velocity;
}

/*

Contenedor de parabolas, calculadas mediante ClassicPredictionSystem

 */
public class PrecalculatedPredictionSystem : ClassicPredictionSystem
{

    private List<List<Vector2>> precalculatedDirections;
    private List<Vector2> impulseForces;
    private int precalculationNumber;
    private float deltaAnglePerIndex;
    private float simulationDeltaTime;
    private int iterations;

    public PrecalculatedPredictionSystem(Rigidbody2D rb, float time, int _iterations, int precalculatedDirectionsCount, float forceMagnitude) : base(rb)
    {
        iterations = _iterations;
        simulationDeltaTime = time / iterations;
        precalculationNumber = precalculatedDirectionsCount;
        deltaAnglePerIndex = (360f / (float)precalculationNumber);
        
        precalculatedDirections = new List<List<Vector2>>();
        impulseForces = new List<Vector2>();

        Vector2 impulse = Vector2.up * forceMagnitude;

        float deltaDegrees = 360.0f / precalculatedDirectionsCount;

        for (int i = 0; i < precalculatedDirectionsCount; i++)
        {
            precalculatedDirections.Add(base.SimulateImpulse(impulse, time, iterations, false));
            impulseForces.Add(impulse);
            impulse = impulse.Rotate(deltaDegrees);
        }

        CloseSimulation();

    }

    public JobyfablePrecalculatedPredictionSystem GetJobyfable()
    {
        var rb = new Rigidbody2DInfo();
        rb.position = this.rb.position;
        rb.velocity = this.rb.velocity;

        return new JobyfablePrecalculatedPredictionSystem(rb, precalculatedDirections, impulseForces, deltaAnglePerIndex, simulationDeltaTime);
    }

    public Vector2 GetForce(int simulationIndex)
    {
        return impulseForces[simulationIndex];
    }

    public int GetSimulationIndex(float degree)
    {
        return (int)(degree / deltaAnglePerIndex);
    }

    public Vector2 ReadSimulationPosition(int simulationIndex, int index)
    {
        return precalculatedDirections[simulationIndex][index] + (rb.velocity * index * simulationDeltaTime) + rb.position;
    }
    public Vector2 ReadSimulationPositionIgnoringVelocity(int simulationIndex, int index)
    {
        return precalculatedDirections[simulationIndex][index] + rb.position;
    }

    public Vector2 ReadLocalSimulationPosition(int simulationIndex, int index)
    {
        return precalculatedDirections[simulationIndex][index] + (rb.velocity * index * simulationDeltaTime);
    }
    public Vector2 ReadLocalSimulationPositionIgnoringVelocity(int simulationIndex, int index)
    {
        return precalculatedDirections[simulationIndex][index];
    }



    public void DrawTrajectory(int simulationIndex)
    {

        Debug.DrawLine(

              rb.position,
              ReadSimulationPosition(simulationIndex, 0)


          );


        for (int i = 1; i < iterations; i++)
        {
            Debug.DrawLine(

                ReadSimulationPosition(simulationIndex, i - 1),
                ReadSimulationPosition(simulationIndex, i)

            );
        }

    }

    public void DrawTrajectory(int simulationIndex, float time)
    {

        Debug.DrawLine(

               rb.position,
               ReadSimulationPosition(simulationIndex, 0),
               Color.red
              

           );


        for (int i = 1; i < iterations; i++)
        {
            Debug.DrawLine(

                ReadSimulationPosition(simulationIndex, i - 1),
                ReadSimulationPosition(simulationIndex, i),
                Color.red,
                time

            );
        }

    }

    public void DrawTrajectoryIgnoringVelocity(int simulationIndex, float drawDuration)
    {

        Debug.DrawLine(

                rb.position,
                ReadSimulationPositionIgnoringVelocity(simulationIndex, 0),
                Color.red,
                drawDuration

            );



        for (int i = 1; i < iterations; i++)
        {
            Debug.DrawLine(

                ReadSimulationPositionIgnoringVelocity(simulationIndex, i - 1),
                ReadSimulationPositionIgnoringVelocity(simulationIndex, i),
                Color.red,
                drawDuration

            );
        }

    }

    public void DrawTrajectoryIgnoringVelocity(int simulationIndex, float drawDuration, Vector2 positionOffset)
    {

        Debug.DrawLine(

               rb.position+ positionOffset,
               ReadSimulationPositionIgnoringVelocity(simulationIndex, 0)+ positionOffset,
               Color.red,
               drawDuration

           );



        for (int i = 1; i < iterations; i++)
        {
            Debug.DrawLine(

                ReadSimulationPositionIgnoringVelocity(simulationIndex, i - 1)+ positionOffset,
                ReadSimulationPositionIgnoringVelocity(simulationIndex, i)+ positionOffset,
                Color.red,
                drawDuration

            );
        }

    }

    public void DrawTrajectoryIgnoringVelocity(int simulationIndex, float time, Vector2 position, Color color)
    {


        Debug.DrawLine(

                position + rb.position,
                position + ReadLocalSimulationPositionIgnoringVelocity(simulationIndex, 0),
                Color.red,
                time

            );



        for (int i = 1; i < iterations; i++)
        {
            Debug.DrawLine(

                position + ReadLocalSimulationPositionIgnoringVelocity(simulationIndex, i - 1),
                position + ReadLocalSimulationPositionIgnoringVelocity(simulationIndex, i),
                color,
                time

            );
        }

    }

}


public class JobyfablePrecalculatedPredictionSystem
{
    /*rb information must be updated every FixedUpdate*/
    public Rigidbody2DInfo rb;
    public List<List<Vector2>> precalculatedDirections;
    private List<Vector2> impulseForces;
    private float deltaAnglePerIndex;
    private float simulationDeltaTime;


    public int GetPrecalculationDirectionsCount()
    {
        return precalculatedDirections.Count;
    }

    public JobyfablePrecalculatedPredictionSystem (Rigidbody2DInfo rb, List<List<Vector2>> precalculatedDirections, List<Vector2> impulseForces, float deltaAnglePerIndex, float simulationDeltaTime)
    {

        this.rb = rb;
        this.precalculatedDirections = precalculatedDirections;
        this.impulseForces = impulseForces;
        this.deltaAnglePerIndex = deltaAnglePerIndex;
        this.simulationDeltaTime = simulationDeltaTime;
    }


public Vector2 GetForce(int simulationIndex)
    {
        return impulseForces[simulationIndex];
    }

    public int GetSimulationIndex(float degree)
    {
        return (int)(degree / deltaAnglePerIndex);
    }

    public Vector2 ReadSimulationPosition(int simulationIndex, int index)
    {
        return precalculatedDirections[simulationIndex][index] + (rb.velocity * index * simulationDeltaTime) + rb.position;
    }
    public Vector2 ReadSimulationPositionIgnoringVelocity(int simulationIndex, int index)
    {
        return precalculatedDirections[simulationIndex][index] + rb.position;
    }

    public Vector2 ReadLocalSimulationPosition(int simulationIndex, int index)
    {
        return precalculatedDirections[simulationIndex][index] + (rb.velocity * index * simulationDeltaTime);
    }
    public Vector2 ReadLocalSimulationPositionIgnoringVelocity(int simulationIndex, int index)
    {
        return precalculatedDirections[simulationIndex][index];
    }




}


