using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerFPS : Agent {
    private Rigidbody rigidbody;
    private float currentSpeed;
    private float jumpForce = 500;
    private Transform cameraMain;
    private Vector3 cameraPosition;
    private bool grounded;
    private Vector2 planarMovement;
    private int maxSpeed = 5;
    private Vector3 slowdownValue;
    private float deaccelerationSpeed = 15.0f;
	private float accelerationSpeed = 50000.0f;

    protected new void Awake(){
        base.Awake();
		rigidbody = GetComponent<Rigidbody>();
		cameraMain = Camera.main.transform;
	}

    protected override void Start() {
        base.Start();
    }

    private void FixedUpdate() {
        CheckMovement();
    }

    private void CheckMovement(){
		if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
            if(movingState == MovingState.Stop)
                movingState = MovingState.Walking;
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 _moveHorizontal = transform.right * h;
            Vector3 _moveVertical = transform.forward * v;
            Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized;
            movingDelta = new Vector3(h, 0, v);
            transform.Translate(_velocity * GetMoveSpeedFromCurrentState() * Time.deltaTime, Space.World);
        }
        else {
            movingState = MovingState.Stop;
        }
}

    protected override void PlayWalkingSound() {
        
    }

    public override void Die() {
        
    }
}
