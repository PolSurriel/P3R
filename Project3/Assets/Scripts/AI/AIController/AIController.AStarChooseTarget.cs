using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AIController : MonoBehaviour
{

    AstarGoal aStarGoal;
    public const float VALID_TARGET_AREA_RADIUS = 10f;
    Vector2 lastTargetLandedPosition;

    /*
     En ocasiones nos puede interesar que un target sea escogido 
     sólamente si el player está debajo o a la derecha o a la izquierda del target.

     Dependiendo del diseño del mapa, habrá tendencias diferentes de elección de target.

    Ejemplo:

    Imagina que:
      -tienes dos paredes:
      -una a la izquierda y otra a la derecha
      -están repletas de targets
      - El paso está bloqueado y sólo se puede salir 
        por un agujero pegado a la pared de la izquierda

    No tendria sentido ir de una pared a la otra. lo más lógico sería:

    Subir por la pared de la izquiera, o si estoy en el de la 
    derecha subir por la derecha para eventualmente cambiar a la izquierda

    Pues el dot constraint sirve para eso. El diseñador marca un vector que restringe
    el ángulo del vecto player --> target a un rango mediante el dot product entre
    el vector player --> target y el vector asignado por el diseñador.

    Entonces el diseñador mediante esto puede hacer, por ejemplo,
    que un target solo se escogido si el vector player --> target es perpendicular
    al vector UP (si su dot es > 0.9 por ejemplo)

    IMPORTANTE: El dot constraint es una PREFERENCIA. Da PRIORIDAD pero no siempre se respeta.

    Ej: Si no hay ningun target q cumpla el dot constraint se omitirá el criterio.
     
     */
    bool DotConstraint(PathTarget target)
    {
        if (!target.useDotConstrainToChoose)
            return true;

        Vector2 toTarget = target.GetEvaluablePosition() - (Vector2)transform.position;
        toTarget.Normalize();

        float dot = Vector2.Dot(toTarget, target.dotConstrain);

        return dot < target.dotConstrainThreshold;
    }

    const float MIN_DIST_TO_CHOOSE_TARGET = GOAL_MIN_DISTANCE * 1.2f;


    /*Entre todos los targets cercanos, decide a cual ir mediante el Astar
     
    Devuelve:

    TRUE: Si la búsqueda fue exitosa

    FALSE: Si la búsqueda NO fue exitosa
     
     */

    bool ChooseTarget(bool useDotConstraint = true)
    {


        // Obtenemos todos los targets
        var targets = FindObjectsOfType<PathTarget>();

        //Generamos una lista de targets validos para elegir uno de ellos al azar.
        var validTargets = new List<PathTarget>();

        // por cada target
        foreach (var target in targets)
        {


            // si lo estamos ignorando, pasamos al siguiente
            if (targetsToIgnore.Contains(target))
                continue;

            // extraemos su posicion
            var targetPos = target.GetEvaluablePosition();

            // comprobamos que no es el target anterior
            if (lastTargetPos != targetPos && lastTargetLandedPosition != targetPos)

                // si está por encima del player O estamos en un backupPlanZone
                // * backupPlanZone: En ocasiones puede interesarnos tener un plan de emergencia.
                // * si el player queda atrapado en una esquina y no hay camino a ningun nodo que esté
                // * por encima de él, nos interesa que pueda considerar como validos los targets que están por debajo.
                // * Por ello, existen triggers con el tag "backupPlanZone" que determinan en qué zonas el player
                // * puede elegir targets por debajo de él.
                // * El nombre NO es descriptivo tipo "canChoosDownTarget" porque se planea desarrollar más
                // * backup plans en el futuro mediante comportamientos reactivos.

            
                if ((!(targetPos.y < transform.position.y + GOAL_MIN_DISTANCE)) || onBackupPlanZone)
                {
                    // El target está los suficientemente cerca
                    float dist = Vector2.Distance(targetPos, transform.position);
                    if (dist < VALID_TARGET_AREA_RADIUS && dist > MIN_DIST_TO_CHOOSE_TARGET)
                    {
                        bool mustUseDotConstraint = useDotConstraint || target.forceDotConstraint;

                        // Si cumple con el dot constrain (revisar el comentario de la función DotConstraint para entender)
                        if (!mustUseDotConstraint || DotConstraint(target))
                        {
                            // entonces lo consideramos como valido
                            validTargets.Add(target);
                            continue;
                        }
                    }
                }

        }

        // si no hemos encontrado caminos
        if (validTargets.Count == 0)
        {
            // en caso de usar el dot contraint 
            if (useDotConstraint)
            {
                // hacemos un try sin el dot constraint.
                return ChooseTarget(false);
            }
            else // si ya hemos probado sin el dot constraint
            {
                // no valid target found
                return false;
            }
        }

        // En caso contrario, elegimos un target random y lo asignamos.

        currentPathTargetObject = validTargets[Random.Range(0, validTargets.Count)];

        var goalPosition = currentPathTargetObject.transform.position;

        if (currentPathTargetObject.useRandomVerticalOffset)
        {
            goalPosition.y += Random.Range(currentPathTargetObject.maxVerticalOffset * -0.5f, currentPathTargetObject.maxVerticalOffset * 0.5f);
        }else
        {
            if (currentPathTargetObject.useRandomHorizontalOffset)
            {
                goalPosition.y += Random.Range(currentPathTargetObject.maxHorizontalOffset * -0.5f, currentPathTargetObject.maxHorizontalOffset * 0.5f);
            }
        }


        aStarGoal = new AstarGoal(currentPathTargetObject.transform.position, currentPathTargetObject.incisionVector, currentPathTargetObject.useIncisionConstrain);
        lastTargetPos = aStarGoal.position;

        // success!
        return true;

    }



}
