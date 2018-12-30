using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {

    //Drzewo stanów

    public enum Task
    {
        idle, move, follow, chase, attack
    }

    const string ANIMATOR_SPEED = "Speed",
        ANIMATOR_ALIVE = "Alive",
        ANIMATOR_SHOOT = "Shoot";

    public bool isAlive { get { return hp > 0; } }

    //zeby miec dostep do listy tworzymy gettera tego samego typu i nazwie
    public static List<ISelectable> SelectableUnits { get { return selectableUnits; } }
    static List<ISelectable> selectableUnits = new List<ISelectable>();//Lista obiektow do zaznaczenia
    

    [Header("Unit")]
    [SerializeField]
    GameObject hpBarPrefab;
    [SerializeField]
    float hp, hpMax = 100;
    [SerializeField]
    protected float attackDistance = 1,
        attackSpeed = 1,
        stoppingDistance = 1,
        attackDamage = 0;

    public float HealtPercent { get { return hp / hpMax; } }


    protected HealthBar healthBar;//soldier bedzie korzystac

    //udostepniamy go poprzez protected
    protected NavMeshAgent nav;
    public Transform target;
    Animator animator;
    //zmienna przechowujaca wartosc enum'a
    //protected aby inne jednostki mogly z tego korzystac - dziedziczenie
    protected Task task = Task.idle;
    [SerializeField]
    float stoppingDistance = 1;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        hp = hpMax;
        healthBar  = Instantiate(hpBarPrefab, transform).GetComponent<HealthBar>();// zapisujemy referencje
    }


    private void Start () {
		if (this is ISelectable)
        {
            selectableUnits.Add(this as ISelectable);// dodamy jestli nasz obiekt jest selectable to dodamy sobie do listy
           // (this as ISelectable).SetSelected(false);
        }
	}

    private void OnDestroy()// jesli umrze
    {
        if (this is ISelectable)
        {
            selectableUnits.Remove(this as ISelectable);// dodamy jestli nasz obiekt jest selectable to dodamy sobie do listy
        }
    }

    // Update is called once per frame
    void Update () {

        /*
		   */
        if (isAlive)
        {
            switch (task)
            {
                case Task.idle: Idling();
                    break;
                case Task.move: Moving();
                    break;
                case Task.follow: Following();
                    break;
                case Task.chase: Chasing();
                    break;
                case Task.attack: Attacking();
                    break;

            }
        }
        
        Animate();
      
    }

    //virtual - Aby moc je nadpisywac
    protected virtual void Idling ()
    {
        nav.velocity = Vector3.zero;//zerujemy predkosc nav mesha
    }
    protected virtual void Moving()
    {
        //tu sprawdzamy czy dotarlismy
        float distance = Vector3.Magnitude(nav.destination - transform.position);
        if (distance <= stoppingDistance)
        {
            task = Task.idle;
        }
    }
    protected virtual void Following()
    {
        if (target)
        {
            nav.SetDestination(target.position);
        } else
        {
            task = Task.idle;
        }
    }
    protected virtual void Chasing()
    {
        //toDo
    }
    protected virtual void Attacking()
    {
        
        if(target)
        {
            nav.velocity = Vector3.zero;//zerujemy predkosc nav mesha
            transform.LookAt(target);
        }
        else
        {
            task = Task.idle;
        }
    }

    protected virtual void Animate ()
    {
        var speedVector = nav.velocity;
        speedVector.y = 0;
        float speed = speedVector.magnitude;
        animator.SetFloat(ANIMATOR_SPEED, speed);
        animator.SetBool(ANIMATOR_ALIVE, hp>0);
    }

    //public ze wzgledu na wywolywanie z zewatrz
    public virtual void Attack()
    {
        animator.SetTrigger(ANIMATOR_SHOOT);
    }

    public virtual void DealDamage()
    {
        if (target)
        {
            Unit unit = target.GetComponents<Unit>();
            if (unit && unit.isAlive)
            {
                unit.hp -= attackDamage;
            }
        }
    }

}
