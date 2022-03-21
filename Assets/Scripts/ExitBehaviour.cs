using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBehaviour : StateMachineBehaviour
{
	private ActorBase actor;
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (actor == null)
		{
			actor = animator.transform.GetComponent<ActorBase>();
		}
		actor.OnAnimationExit();
	}
}
