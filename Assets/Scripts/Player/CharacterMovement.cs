using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    public float RotateSpeed = 3.0f;


    private CharacterMotor motor;

    // Use this for initialization
    void Awake()
    {
        motor = GetComponent<CharacterMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.Paused) return;

        // Get the input vector from kayboard or analog stick
        Vector3 directionVector = new Vector3(0, 0, Input.GetAxis("Vertical"));

        if (directionVector != Vector3.zero)
        {
            // Get the length of the directon vector and then normalize it
            // Dividing by the length is cheaper than normalizing when we already have the length anyway
            float directionLength = directionVector.magnitude;
            directionVector = directionVector / directionLength;

            // Make sure the length is no bigger than 1
            directionLength = Mathf.Min(1, directionLength);

            // Make the input vector more sensitive towards the extremes and less sensitive in the middle
            // This makes it easier to control slow speeds when using analog sticks
            directionLength = directionLength * directionLength;

            // Multiply the normalized direction vector by the modified length
            directionVector = directionVector * directionLength;
        }

        // Apply the direction to the CharacterMotor
        motor.inputMoveDirection = transform.rotation * directionVector;

        transform.Rotate(0, Input.GetAxis("Horizontal") * RotateSpeed, 0);


        motor.inputJump = Input.GetButtonUp("Jump");
    }
}
