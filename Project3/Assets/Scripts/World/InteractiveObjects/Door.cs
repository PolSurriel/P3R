using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private readonly Quaternion LEFT_OPENED_ROTATION = Quaternion.Euler(0f, 0f, 90f);
    private readonly Quaternion RIGHT_OPENED_ROTATION = Quaternion.Euler(0f, 0f, -90f);
    private readonly Quaternion LEFT_CLOSED_ROTATION = Quaternion.identity;
    private readonly Quaternion RIGHT_CLOSED_ROTATION = Quaternion.identity;


    public Transform leftDoor;
    public Transform rightDoor;


    public Transform normalOrigin;
    public Transform normalEnd;

    public BoxCollider2D leftCollider;
    public BoxCollider2D rightCollider;

    Vector2 normal;

    private void Start()
    {
        normal = normalEnd.position - normalOrigin.position;

        normal.Normalize();

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

            leftDoor.localRotation = Quaternion.Lerp(LEFT_CLOSED_ROTATION, LEFT_OPENED_ROTATION, timeCounter / TIME_TO_OPEN_OR_CLOSE);
            rightDoor.localRotation = Quaternion.Lerp(RIGHT_CLOSED_ROTATION, RIGHT_OPENED_ROTATION, timeCounter / TIME_TO_OPEN_OR_CLOSE);
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

            leftDoor.localRotation = Quaternion.Lerp(LEFT_OPENED_ROTATION, LEFT_CLOSED_ROTATION, timeCounter / TIME_TO_OPEN_OR_CLOSE);
            rightDoor.localRotation = Quaternion.Lerp(RIGHT_OPENED_ROTATION, RIGHT_CLOSED_ROTATION, timeCounter / TIME_TO_OPEN_OR_CLOSE);
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

        if (Vector2.Dot(rb.velocity.normalized, normal) > 0f)
        {
            Close();
        }
    }




}
