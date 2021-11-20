﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AIController : MonoBehaviour
{
    public class AStarNode
    {
        public int immidiatPortal;
        public bool secondJumpDone;
        public int directionIndex;
        public int positionIndex;

        private Vector2 m_position;
        public Vector2 portalSense = Vector2.one;

        public Vector2 position
        {
            get { return m_position + portalOffset; }
            set { m_position = value; }
        }

        public float coste;
        public float time;

        public Vector2 portalOffset;


        public AStarNode(Vector2 _pos, bool _secondJumpDone, int _directionIndex, int _positionIndex, float _coste, float _time, int _immidiatPortal)
        {

            immidiatPortal = _immidiatPortal;
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