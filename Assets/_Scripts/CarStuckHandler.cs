using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarStuckHandler : MonoBehaviour
{
    [SerializeField] bool grounded, canDoUnstuck;
    [SerializeField] float stationaryThresholdSpeed, thresholdAngle, timeToWait, elapsedTime;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField]
    CanvasGroup buttonUI;
    Rigidbody rb;
    public InputActionReference unstuckInput;
    void OnEnable()
    {
        unstuckInput.action.started += DoUnstuck;
    }
    void OnDisable()
    {
        unstuckInput.action.started -= DoUnstuck;
    }

    void OnDestroy()
    {
        unstuckInput.action.started -= DoUnstuck;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, whatIsGround);
        bool isStationary = stationaryThresholdSpeed > rb.linearVelocity.magnitude;
        bool isWrongSideUp = thresholdAngle < Vector3.Angle(Vector3.up, transform.up);
        if(grounded && isStationary && isWrongSideUp) elapsedTime += Time.deltaTime;
        else elapsedTime = 0f;

        if(elapsedTime > timeToWait) 
        {
            buttonUI.alpha = 1;
            canDoUnstuck = true;
        }
        else
        {
            buttonUI.alpha =0;
            canDoUnstuck = false;
        }

    }
    void DoUnstuck(InputAction.CallbackContext context)
    {
        if(canDoUnstuck)
        {
            elapsedTime = 0;
            transform.position = transform.position + Vector3.up * 4f;
            transform.rotation = Quaternion.identity;
            
        }
    }
}
