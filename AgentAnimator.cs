using UnityEngine;

public class AgentAnimator : MonoBehaviour {
    private Animator animator;
    private Vector3 previousDir = Vector3.zero;
    private float previousRotY = 0;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void SetMovingAnim(MovingState movingState, Vector3 movingDelta) {
        animator.SetInteger("MovingState", (int)movingState);
        movingDelta.y = 0;
        movingDelta = movingDelta.normalized;
        Vector3 currentDir = movingDelta;
        previousDir = Vector3.Lerp(previousDir, currentDir, 10 * Time.deltaTime);
        animator.SetFloat("XDelta", previousDir.x);
        animator.SetFloat("YDelta", previousDir.z);
    }

    public void SetTurningAnim(Vector3 rotation) {
        float angle = rotation.y - previousRotY;
        if(angle < 0.05f && angle > -0.05f) {
            animator.SetTrigger("StopTurn");
        }
        else if(angle > 0) {
            animator.SetTrigger("TurnRight");
        }
        else {
            animator.SetTrigger("TurnLeft");
        }
        previousRotY = rotation.y;
    }

    public void SetAimAnim(bool isAiming) {
        animator.SetBool("IsAiming", isAiming);
    }

    private int GetDirectionNumFromMovingAngle(float angle) {
        int directionNum = 0;
        if (angle >= -22.5f && angle < 22.5f)
            directionNum = 3; // Right
        else if (angle >= 22.5f && angle < 67.5f)
            directionNum = 2; // Front-Right
        else if (angle >= 67.5f && angle < 112.5f)
            directionNum = 1; // Front
        else if (angle >= 112.5f && angle < 157.5f)
            directionNum = 8;  // Front-Left
        else if (angle >= 157.5f || angle < -157.5f)
            directionNum = 7;  // Left
        else if (angle >= -157.5f && angle < -112.5f)
            directionNum = 6;  // Back-Left
        else if (angle >= -112.5f && angle < -67.5f)
            directionNum = 5;  // Back
        else
            directionNum = 4;  // Back-Right
        return directionNum;
    }
}
