using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public Transform leftDoor;
    public Transform rightDoor;

    Quaternion leftClosedRotation = Quaternion.identity;
    Quaternion rightClosedRotation = Quaternion.identity;
    Quaternion leftOpenedRotation;
    Quaternion rightOpenedRotation;

    private void Start()
    {
        leftOpenedRotation = leftDoor.rotation;
        rightOpenedRotation = rightDoor.rotation;

        Close();
    }

    const float TIME_TO_OPEN_OR_CLOSE = 0.5f;
    const float TIME_TO_START_OPENING = 1f;

    bool closed = false;

    void Open()
    {
        StartCoroutine(OpenRoutine());

    }

    void Close()
    {
        if (closed)
            return;

        closed = true;

        StartCoroutine(CloseRoutine());
    }

    IEnumerator OpenRoutine()
    {
        float timeCounter = 0f;

        do
        {
            timeCounter += Time.deltaTime;

            leftDoor.rotation = Quaternion.Lerp( leftClosedRotation, leftOpenedRotation, timeCounter/ TIME_TO_OPEN_OR_CLOSE);
            rightDoor.rotation = Quaternion.Lerp(rightClosedRotation, rightOpenedRotation, timeCounter / TIME_TO_OPEN_OR_CLOSE);
            yield return null;

        } while (timeCounter < TIME_TO_OPEN_OR_CLOSE);

        closed = false;

    }
    IEnumerator CloseRoutine()
    {
        float timeCounter = 0f;

        do
        {
            timeCounter += Time.deltaTime;

            leftDoor.rotation = Quaternion.Lerp(leftOpenedRotation, leftClosedRotation, timeCounter / TIME_TO_OPEN_OR_CLOSE);
            rightDoor.rotation = Quaternion.Lerp(rightOpenedRotation, rightClosedRotation, timeCounter / TIME_TO_OPEN_OR_CLOSE);
            yield return null;

        } while (timeCounter < TIME_TO_OPEN_OR_CLOSE);


        timeCounter = 0f;

        do
        {
            timeCounter += Time.deltaTime;

            yield return null;

        } while (timeCounter < TIME_TO_START_OPENING);

        Open();



    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var rb = collision.GetComponent<Rigidbody2D>();

        if(rb.velocity.y > 0f)
        {
            Close();
        }
    }




}
