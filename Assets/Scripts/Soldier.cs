
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit, ISelectable
{

    [Header("Soldier")]
    [Range(0, .3f), SerializeField]
    float shootDuration = 0;
    [SerializeField]
    ParticleSystem muzzleEffect, impactEffect;
    [SerializeField]
    LayerMask shootingLayerMask;


    public void SetSelected(bool selected)
    {
        healthBar.gameObject.SetActive(selected);
    }

    void Command(Vector3 destination)
    {
        nav.SetDestination(destination);
        task = Task.move;
        target = null;
    }

    void Command(Soldier soldierToFollow)
    {
        target = soldierToFollow.transform;
        task = Task.follow;
    }

    void Command(Dragon dragonToKill)
    {

    }

    public override void DealDamage ()
    {
        if (Shoot())
        {
            base.DealDamage();
        }
       
    }

    bool Shoot()
    {
        Vector3 start = muzzleEffect.transform.position;
        Vector3 direction = transform.forward;
        RaycastHit hit;

        if(Physics.Raycast(start, direction, out hit, attackDistance,shootingLayerMask))
        {
            var unit = hit.collider.gameObject.GetComponents<unit>();
        }
    }
}
