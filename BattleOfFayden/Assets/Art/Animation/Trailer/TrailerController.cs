using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrailerController : MonoBehaviour
{
    [Header("APE")]
    public GameObject ape;
    public Animator apeAnimator;

    [Header("Pangolin")]
    public GameObject pangolin;
    public Animator pangolinAnimator;

    GameObject destinationPoint;
    NavMeshAgent agent;


	void Start ()
    {
        apeAnimator = ape.GetComponent<Animator>();
        pangolinAnimator = pangolin.GetComponent<Animator>();

        //destinationPoint = GameObject.Find("Destination");

        //agent = pangolin.GetComponent<NavMeshAgent>();

        //agent.destination = destinationPoint.transform.position;

    }
    
	void Update ()
    {
		if(agent.hasPath)
        {
            pangolinAnimator.SetBool("isWalking", true);
        }else
        {
            pangolinAnimator.SetBool("isWalking", false);
        }
	}

    /// <summary>
    ///             APE
    /// </summary>

    void ApeAnimationSwitchTrue()
    {
        apeAnimator.SetBool("isReady", true);
    }

    void ApeAnimationSwitchFalse()
    {
        apeAnimator.SetBool("isReady", false);
    }

    void ApeAnimationStop()
    {
        apeAnimator.enabled = false;
    }

    /// <summary>
    ///             Pangolin
    /// </summary>

    void PangolinAnimationSwitchTrue()
    {
        pangolinAnimator.SetBool("isReady", true);
    }

    void PangolinAnimationSwitchFalse()
    {
        pangolinAnimator.SetBool("isReady", false);
    }

    void PangolinAnimationStop()
    {
        pangolinAnimator.enabled = false;
    }
    
    void PangolinAnimationAttachTrue()
    {
        pangolinAnimator.SetBool("Attack", true);
    }

    void PangolinAnimationAttachFalse()
    {
        pangolinAnimator.SetBool("Attack", false);
    }
}
