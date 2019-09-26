using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTEST : MonoBehaviour
{
    public Transform point1, point2;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, point2.rotation);
        animator.SetIKRotation(AvatarIKGoal.RightHand, point1.rotation);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, point2.position);
        animator.SetIKPosition(AvatarIKGoal.RightHand, point1.position);
    }
}
