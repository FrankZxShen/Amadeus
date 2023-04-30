using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIdleState : StateMachineBehaviour
{
    public int numberofstate=6;
    public float minnormtime=0f;
    public float maxnormtime=5f;

    protected float m_RandomNormTime;
    readonly int m_HashRandomIdle=Animator.StringToHash("RandomIdle");

    override public void OnStateEnter(Animator animator,AnimatorStateInfo stateInfo,int layerIndex)
    {
        m_RandomNormTime=Random.Range(minnormtime,maxnormtime);
    }
    override public void OnStateUpdate(Animator animator,AnimatorStateInfo stateInfo,int layerIndex)
    {
        if(animator.IsInTransition(0)&&animator.GetCurrentAnimatorStateInfo(0).fullPathHash==stateInfo.fullPathHash)
        {
            animator.SetInteger(m_HashRandomIdle,-1);
        }
        if(stateInfo.normalizedTime>m_RandomNormTime&&!animator.IsInTransition(0))
        {
            animator.SetInteger(m_HashRandomIdle,Random.Range(0,numberofstate));
        }
    }
}
