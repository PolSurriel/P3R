using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AIController : MonoBehaviour
{
    public class AStarNode
    {
        public int iterationsSincePortalCrossed;
        public bool secondJumpDone;
        public int directionIndex;
        public int positionIndex;
        public Vector2 portalSense = Vector2.one;
        public bool swapXY;
        public Vector2 portalNormal;
        public Vector2 origin;

        private Vector2 m_position;
        public Vector2 position
        {
            get { return m_position + portalOffset; }
            set { m_position = value; }
        }

        public float coste;
        public float time;
        public Vector2 portalOffset;

        public float H(Vector2 goalPosition) { return (position - goalPosition).magnitude;}

        public AStarNode() {}
        public AStarNode(Vector2 _pos, bool _secondJumpDone, int _directionIndex, int _positionIndex, float _coste, float _time, int _immidiatPortal)
        {
            iterationsSincePortalCrossed = _immidiatPortal;
            secondJumpDone = _secondJumpDone;
            directionIndex = _directionIndex;
            positionIndex = _positionIndex;
            position = _pos;

            coste = _coste;
            time = _time;

        }


#if UNITY_EDITOR
        public delegate void IfChoosenDoONGizmos();
        public List<IfChoosenDoONGizmos> ifChoosenDoOnGizmos = new List<IfChoosenDoONGizmos>();
        public List<IfChoosenDoONGizmos> ifChoosenAndCurrentDoOnGizmos = new List<IfChoosenDoONGizmos>();
#endif

    }


}
