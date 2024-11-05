using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DollSkillShield : DollSkillBase
{
    public GameObject wall;

    protected NavMeshAgent dollNav;
    protected int originalPriority;
    protected Vector3 myPosition = new();

    // Start is called before the first frame update
    void Start()
    {
        dollNav = doll.GetComponent<NavMeshAgent>();
        if (dollNav)
            originalPriority = dollNav.avoidancePriority;
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
            dollNav.avoidancePriority = 10;
        }
        else
        {
            dollNav.avoidancePriority = originalPriority;
        }

        if (wall)
        {
            wall.SetActive(active);
        }
    }
}
