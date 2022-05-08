using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public Transform leftDoor;
    public Transform rightDoor;


    public Transform normalOrigin;
    public Transform normalEnd;

    public BoxCollider2D leftCollider;
    public BoxCollider2D rightCollider;

    Quaternion leftClosedRotation;
    Quaternion rightClosedRotation;
    Quaternion leftOpenedRotation;
    Quaternion rightOpenedRotation;
    Quaternion initialRot;


    Vector2 normal;

    private void Start()
    {
        normal = normalEnd.position - normalOrigin.position;

        normal.Normalize();

        leftOpenedRotation = leftDoor.rotation;
        rightOpenedRotation = rightDoor.rotation;

        leftClosedRotation = leftOpenedRotation * Quaternion.AngleAxis(-90f, Vector3.forward);
        rightClosedRotation = rightOpenedRotation * Quaternion.AngleAxis(90f, Vector3.forward);

        leftCollider.enabled = false;
        rightCollider.enabled = false;

    }

    const float TIME_TO_OPEN_OR_CLOSE = 0.2f;
    const float TIME_TO_START_OPENING = 1f;

    bool closed = false;

    void Open()
    {
        AudioController.instance.sounds.door.Play();
        leftCollider.enabled = false;
        rightCollider.enabled = false;
        StartCoroutine(OpenRoutine());

    }

    void Close()
    {
        if (closed)
            return;

        AudioController.instance.sounds.doorClose.Play();
        closed = true;

        StartCoroutine(CloseRoutine());
    }

    IEnumerator OpenRoutine()
    {
        float timeCounter = 0f;

        do
        {
            timeCounter += Time.deltaTime;

            leftDoor.localRotation = Quaternion.Lerp( leftClosedRotation, leftOpenedRotation, timeCounter/ TIME_TO_OPEN_OR_CLOSE);
            rightDoor.localRotation = Quaternion.Lerp(rightClosedRotation, rightOpenedRotation, timeCounter / TIME_TO_OPEN_OR_CLOSE);
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

            leftDoor.localRotation = Quaternion.Lerp(leftOpenedRotation, leftClosedRotation, timeCounter / TIME_TO_OPEN_OR_CLOSE);
            rightDoor.localRotation = Quaternion.Lerp(rightOpenedRotation, rightClosedRotation, timeCounter / TIME_TO_OPEN_OR_CLOSE);
            yield return null;

        } while (timeCounter < TIME_TO_OPEN_OR_CLOSE);

        leftCollider.enabled = true;
        rightCollider.enabled = true;
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
        Debug.Log("activated");
        var rb = collision.GetComponent<Rigidbody2D>();

        if (rb == null)
            return;

        if(Vector2.Dot(rb.velocity.normalized, normal) > 0f)
        {
            Close();
        }
    }




}
