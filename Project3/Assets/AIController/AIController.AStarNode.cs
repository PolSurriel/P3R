using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AIController : MonoBehaviour
{
    public class AStarNode
    {

        public bool secondJumpDone;
        public int directionIndex;
        public int positionIndex;
        public Vector2 position;
        public float coste;
        public float time;


        public AStarNode(Vector2 _pos, bool _secondJumpDone, int _directionIndex, int _positionIndex, float _coste, float _time)
        {

            secondJumpDone = _secondJumpDone;
            directionIndex = _directionIndex;
            positionIndex = _positionIndex;
            position = _pos;

            coste = _coste;
            time = _time;


        }


        public float H(Vector2 goalPosition)
        {


            return (position - goalPosition).magnitude;
        }


        public AStarNode()
        {

        }



    }


}
