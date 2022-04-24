using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace SurrealBoost.GizmosTools
{


    public class Draw2D
    {

        public static void Line(Vector3 p1, Vector3 p2, float width)
        {
            Line(p1, p2, width, Color.white);
        }

        public static void Line(Vector3 p1, Vector3 p2, float width, Color color)
        {

            #if UNITY_EDITOR

            Vector2 p1ToP2 = p2 - p1;
            Vector3 perpendicular = new Vector2(p1ToP2.y, -p1ToP2.x).normalized;


            width *= 0.5f;
            Handles.color = color;
            Handles.DrawSolidRectangleWithOutline( 
                new Vector3[]
                {
                    p1 - perpendicular * width,
                    p1 + perpendicular * width,
                    p2 + perpendicular * width,
                    p2 - perpendicular * width,

                }
                , 
                color, 
                new Color(0f, 0f, 0f, 0f));
            
            #endif

        }

        public static void ArrowedLine(Vector3 start, Vector3 end, float width)
        {
            ArrowedLine(start, end, width, Color.white);
        }

        public static void ArrowedLine(Vector3 start, Vector3 end, float width, Color color)
        {


            const float ratioLineWidthArrowSize = 1.2f;

            Vector2 p1ToP2 = (end - start).normalized;
            Vector3 perpendicular = new Vector2(p1ToP2.y, -p1ToP2.x).normalized * width * ratioLineWidthArrowSize;

            float arrowLen = 2f * width;

            Vector3 arrowStartPoint = (Vector2)end - (p1ToP2 * arrowLen);

            Line(start, arrowStartPoint, width, color);


            #if UNITY_EDITOR
            Handles.DrawAAConvexPolygon(
                    new Vector3[]
                    {
                    end,
                    arrowStartPoint + perpendicular,
                    arrowStartPoint - perpendicular,

                    }
                );
            #endif
        }

    }

}