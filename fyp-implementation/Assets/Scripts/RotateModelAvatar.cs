using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateModelAvatar : MonoBehaviour
{
    private static RotateModelAvatar instance;
    private bool isRotating = false;
    private void Awake()
    {
        instance = this;
    }


    IEnumerator RotateCoroutine()
    {
        if (!isRotating)
        {
            isRotating = true;

            float targetAngle = transform.rotation.eulerAngles.y + 45f;
            Debug.Log(targetAngle);

            while (transform.rotation.eulerAngles.y < targetAngle)
            {
                // Adjust the rotation speed as needed
                float rotationSpeed = 90f; // 90 degrees per second
                float rotationAmount = rotationSpeed * Time.deltaTime;

                transform.Rotate(new Vector3(0, 1, 0), rotationAmount);
                yield return null;
            }

            // Snap the rotation to exactly 45 degrees
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            isRotating = false;
        }
    }
    void Rotate()
    {
        StartCoroutine(RotateCoroutine());
    }

    public static void RotateAvatar_Static()
    {
        instance.Rotate();
    }
}
