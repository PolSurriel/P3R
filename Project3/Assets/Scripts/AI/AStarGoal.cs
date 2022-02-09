using UnityEngine;

public class AstarGoal
{
    public bool useIncisionConstrain;
    public Vector2 position;
    public Vector2 incisionDirection;

    public AstarGoal(Vector2 position, Vector2 incisionDirection, bool useIncisionConstrain)
    {
        this.position = position;
        this.incisionDirection = incisionDirection;
        this.useIncisionConstrain = useIncisionConstrain;
    }

}
