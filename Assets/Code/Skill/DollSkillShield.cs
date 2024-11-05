using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DollSkillShield : DollSkillBase
{
    public GameObject wall;
    public float defAdd = 20.0f;

    protected NavMeshAgent dollNav;
    protected int originalPriority;
    protected Vector3 myPosition = new();
    protected HitBody dollHitBody;
    protected float originalDamageRatio;

    // Start is called before the first frame update
    void Start()
    {
        dollNav = doll.GetComponent<NavMeshAgent>();
        if (dollNav)
            originalPriority = dollNav.avoidancePriority;
        dollHitBody = GetComponent<HitBody>();
        if (dollHitBody)
            originalDamageRatio = dollHitBody.DamageRatio;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            transform.position = myPosition;
        }
    }

    public override void OnStartSkill(bool active = true)
    {
        base.OnStartSkill(active);

        if (active)
        {
            myPosition = transform.position;
            if (dollNav)
                dollNav.avoidancePriority = 10;
            if (dollHitBody)
                dollHitBody.DamageRatio = originalDamageRatio - (defAdd * 0.01f);
        }
        else
        {
            if (dollNav)
                dollNav.avoidancePriority = originalPriority;
            if (dollHitBody)
                dollHitBody.DamageRatio = originalDamageRatio;
        }

        if (wall)
        {
            wall.SetActive(active);
        }
    }
}
