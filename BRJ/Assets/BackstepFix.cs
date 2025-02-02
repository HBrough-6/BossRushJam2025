using UnityEngine;

public class BackstepFix : StateMachineBehaviour
{
    public Transform myTransform;

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (myTransform == null)
        {
            myTransform = animator.transform.parent;
        }
        Vector3 temp = new Vector3(0, myTransform.rotation.eulerAngles.y + 180, 0);
        myTransform.rotation = Quaternion.Euler(temp);
        Debug.Log("HI");
    }
}
