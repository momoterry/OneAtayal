using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFXSpawner : MonoBehaviour
{
    public GameObject[] FXObjs;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSpawnFX(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight < 0.5f)
            return;

        if (FXObjs[evt.intParameter])
            Instantiate(FXObjs[evt.intParameter], gameObject.transform.position, Quaternion.identity, gameObject.transform);
    }


}
