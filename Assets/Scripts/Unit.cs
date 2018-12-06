using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {

    const string ANIMATOR_SPEED = "Speed",
        ANIMATOR_ALIVE = "Alive",
        ANIMATOR_SHOOT = "Shoot";

    [SerializeField]
    float hp, hpMax = 100;

    public float HealtPercent { get { return hp / hpMax; } }
    [SerializeField]
    GameObject hpBarPrefab;

    NavMeshAgent nav;
    public Transform target;
    Animator animator;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        hp = hpMax;
        Instantiate(hpBarPrefab, transform);
    }


    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		   if(target)
        {
            nav.SetDestination(target.position);
        }
        Animate();
      
    }

    protected virtual void Animate ()
    {
        var speedVector = nav.velocity;
        speedVector.y = 0;
        float speed = speedVector.magnitude;
        animator.SetFloat(ANIMATOR_SPEED, speed);
        animator.SetBool(ANIMATOR_ALIVE, hp>0);
    }
}
