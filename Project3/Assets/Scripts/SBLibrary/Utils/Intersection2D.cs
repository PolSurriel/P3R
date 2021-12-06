using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


namespace SurrealBoost
{
    namespace Utils
    {
        public class Intersection2D
        {

            public struct IntersectionResult
            {
                public bool result;
                public Vector2 intersectionPoint;
            }

            public static IntersectionResult lineLine(Types.Line line1, Types.Line line2)
            {

                // calculate the distance to intersection point
                float uA = ((line2.pointB.x - line2.pointA.x) * (line1.pointA.y - line2.pointA.y) - (line2.pointB.y - line2.pointA.y) * (line1.pointA.x - line2.pointA.x)) / ((line2.pointB.y - line2.pointA.y) * (line1.pointB.x - line1.pointA.x) - (line2.pointB.x - line2.pointA.x) * (line1.pointB.y - line1.pointA.y));
                float uB = ((line1.pointB.x - line1.pointA.x) * (line1.pointA.y - line2.pointA.y) - (line1.pointB.y - line1.pointA.y) * (line1.pointA.x - line2.pointA.x)) / ((line2.pointB.y - line2.pointA.y) * (line1.pointB.x - line1.pointA.x) - (line2.pointB.x - line2.pointA.x) * (line1.pointB.y - line1.pointA.y));

                // if uA and uB are between 0-1, lines are colliding
                if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
                {
                    float intersectionX = line1.pointA.x + (uA * (line1.pointB.x - line1.pointA.x));
                    float intersectionY = line1.pointA.y + (uA * (line1.pointB.y - line1.pointA.y));

                    return new IntersectionResult() { 
                        result = true,
                        intersectionPoint = new Vector2(intersectionX, intersectionY)
                    };

                }
                return new IntersectionResult()  { result = false };
            }


            // LINE/CIRCLE
            public static Vector2 ClosestLineCircle(Vector2 point1, Vector2 point2, Vector2 circlePosition, float radius)
            {

                // get length of the line
                float distX = point1.x - point2.x;
                float distY = point1.y - point2.y;
                float len = Mathf.Sqrt((distX * distX) + (distY * distY));

                // get dot product of the line and circle
                float dot = (((circlePosition.x - point1.x) * (point2.x - point1.x)) + ((circlePosition.y - point1.y) * (point2.y - point1.y))) / Mathf.Pow(len, 2);

                // find the closest point on the line
                float closestX = point1.x + (dot * (point2.x - point1.x));
                float closestY = point1.y + (dot * (point2.y - point1.y));

                return new Vector2(closestX, closestY);
            }

            // LINE/CIRCLE
            public static bool LineCircle(Vector2 point1, Vector2 point2, Vector2 circlePosition, float radius)
            {

                // is either end INSIDE the circle?
                // if so, return true immediately
                bool inside1 = pointCircle(point1, circlePosition, radius);
                bool inside2 = pointCircle(point2, circlePosition, radius);
                if (inside1 || inside2) return true;

                // get length of the line
                float distX = point1.x - point2.x;
                float distY = point1.y - point2.y;
                float len = Mathf.Sqrt((distX * distX) + (distY * distY));

                // get dot product of the line and circle
                float dot = (((circlePosition.x - point1.x) * (point2.x - point1.x)) + ((circlePosition.y - point1.y) * (point2.y - point1.y))) / Mathf.Pow(len, 2);

                // find the closest point on the line
                float closestX = point1.x + (dot * (point2.x - point1.x));
                float closestY = point1.y + (dot * (point2.y - point1.y));

                // is this point actually on the line segment?
                // if so keep going, but if not, return false
                bool onSegment = linePoint(point1, point2, new Vector2(closestX, closestY));
                if (!onSegment) return false;

           
                // get distance to closest point
                distX = closestX - circlePosition.x;
                distY = closestY - circlePosition.y;
                float distance = Mathf.Sqrt((distX * distX) + (distY * distY));

                if (distance <= radius)
                {
                    return true;
                }
                return false;
            }


            // POINT/CIRCLE
            public static bool pointCircle(Vector2 point, Vector2 circlePosition, float radius)
            {

                // get distance between the point and circle's center
                // using the Pythagorean Theorem
                float distX = point.x - circlePosition.x;
                float distY = point.y - circlePosition.y;
                float distance = Mathf.Sqrt((distX * distX) + (distY * distY));

                // if the distance is less than the circle's
                // radius the point is inside!
                if (distance <= radius)
                {
                    return true;
                }
                return false;
            }


            // LINE/POINT
            public static bool linePoint(Vector2 linePoint1, Vector2 linePoint2, Vector2 point)
            {

                // get distance from the point to the two ends of the line
                float d1 = Vector2.Distance(point, linePoint1);
                float d2 = Vector2.Distance(point, linePoint2);

                // get the length of the line
                float lineLen = Vector2.Distance(linePoint1, linePoint2);

                // since floats are so minutely accurate, add
                // a little buffer zone that will give collision
                float buffer = 0.01f;    // higher # = less accurate

                // if the two distances are equal to the line's
                // length, the point is on the line!
                // note we use the buffer here to give a range,
                // rather than one #
                if (d1 + d2 >= lineLen - buffer && d1 + d2 <= lineLen + buffer)
                {
                    return true;
                }
                return false;
            }
        }
    }
}

