using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParallel : bullet_base
{
    public GameObject singleBulletRef;
    public int bulletNum = 3;
    public float stepWidth = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 paralVec = Vector3.Cross(targetDir, Vector3.up).normalized;

        float shift = stepWidth * (float)(bulletNum - 1) * -0.5f;

        for (int i = 0; i < bulletNum; i++)
        {
            Vector3 pos = transform.position + paralVec * shift;
            GameObject newObj = BattleSystem.SpawnGameObj(singleBulletRef, pos);
            if (newObj)
            {
                bullet_base newBullet = newObj.GetComponent<bullet_base>();
                if (newBullet)
                {

                    newBullet.InitValue(group, baseDamage, targetDir, targetObj);
                }
            }
            shift += stepWidth;
        }

        Destroy(gameObject);
    }

}
