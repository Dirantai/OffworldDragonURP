using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class character : MonoBehaviour
{

    public Transform SOI;
    public Transform body;
    public Transform head;
    public Rigidbody controller;
    public float gravity;
    public float Hsensitivity;
    public float Vsensitivity;
    public float movementForce;
    public float maxSpeed;
    public float slowdownMultiplier;
    public InputActionAsset inputs;
    public float snapHeight;
    
    float angle = 0;
    Vector2 fakeDelta;

    // Start is called before the first frame update
    void Start()
    {
        inputs?.actionMaps[0].Enable();
    }

    void Update(){
        Vector3 direction = transform.position - SOI.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = transform.position - SOI.position;
        Vector3 gravityVector = -direction.normalized * gravity;

        Vector3 hitPos = Vector3.zero;
        float distance = 0;

        if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hitInfo, 10)){
            distance = hitInfo.distance - 1;
            hitPos = hitInfo.point;
        }

        if(hitPos != Vector3.zero){
            Debug.Log(distance);
            if (distance > snapHeight){
                controller.AddForce(gravityVector);
            }
        }else{
            controller.AddForce(gravityVector);
        }

        Vector3 forwardsForce = HandleForce(body.forward, inputs["Thrust"].ReadValue<float>(), movementForce, maxSpeed);
        Vector3 sidewaysForce = HandleForce(body.right, inputs["Lateral Thrust"].ReadValue<float>(), movementForce, maxSpeed);
        Vector3 finalVector = forwardsForce + sidewaysForce;

        fakeDelta += inputs["Mouse Delta"].ReadValue<Vector2>();
        fakeDelta = Vector2.ClampMagnitude(fakeDelta, 1);
        fakeDelta = Vector2.Lerp(fakeDelta, Vector2.zero, 10 * Time.deltaTime);
        body.Rotate(0, inputs["Yaw"].ReadValue<float>() + fakeDelta.x * Hsensitivity, 0);

        angle -= (fakeDelta.y + inputs["Pitch"].ReadValue<float>()) * Vsensitivity;
        angle = Mathf.Clamp(angle, -85, 85);
        head.localRotation = Quaternion.Euler(angle, 0, 0);

        controller.AddForce(finalVector);
    }

    Vector3 HandleForce(Vector3 shipAxisDirection, float playerInput, float force, float maxValue)
    {
        float velocityDotProduct = Vector3.Dot(shipAxisDirection, controller.velocity);
        float localPlayerInput = playerInput * 2;

        if (Mathf.Abs(velocityDotProduct) > maxValue)
        {
            if(localPlayerInput > 0 && velocityDotProduct > 0 || localPlayerInput < 0 && velocityDotProduct < 0)
            {
                localPlayerInput = 0;
            }
        }
    
        float inputDirection = localPlayerInput + Mathf.Clamp(-(velocityDotProduct * Mathf.Clamp(slowdownMultiplier, 0, 1)), -1, 1);

        return shipAxisDirection * Mathf.Clamp(inputDirection, -1, 1) * force;
    }
}
