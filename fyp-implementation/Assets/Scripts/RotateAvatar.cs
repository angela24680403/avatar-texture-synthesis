using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAvatar : MonoBehaviour
{
    private static RotateAvatar instance;
    private bool isRotating = false;
    private void Awake()
    {
        instance = this;
    }


    IEnumerator RotateCoroutine(GameObject avatar, float angle)
    {
        if (!isRotating)
        {
            isRotating = true;

            float targetAngle = avatar.transform.rotation.eulerAngles.y + angle;
            Debug.Log(targetAngle);

            if (targetAngle == 360f)
            {
                targetAngle = targetAngle - 360f;
            }
            while (avatar.transform.rotation.eulerAngles.y < targetAngle)
            {
                // Adjust the rotation speed as needed
                float rotationSpeed = 90f; // 90 degrees per second
                float rotationAmount = rotationSpeed * Time.deltaTime;

                avatar.transform.Rotate(new Vector3(0, 1, 0), rotationAmount);
                yield return null;
            }

            // Snap the rotation to exactly 45 degrees
            avatar.transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            isRotating = false;
        }
    }
    void Rotate(GameObject avatar, float angle)
    {
        StartCoroutine(RotateCoroutine(avatar, angle));
    }

    public static void RotateAvatar_Static(GameObject avatar, float angle)
    {
        instance.Rotate(avatar, angle);
    }
}
