using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMultiShoot : bullet_base
{
    public GameObject singleBulletRef;
    public float initDis = 0.5f;

    public float intAngle = -90.0f;
    public float endAngle = 90.0f;
    public bool endAngleIncluded = true;
    public int bulletNum = 5;

    // Start is called before the first frame update
    void Start()
    {
        float angleStep = 0;
        int stepNum = bulletNum;
        if (endAngleIncluded)
            stepNum -= 1;
        if (bulletNum > 1)
            angleStep = (endAngle - intAngle) / stepNum;

        float currAngle = intAngle;
        for (int i=0; i<bulletNum; i++)
        {
#if XZ_PLAN
            Quaternion rM = Quaternion.AngleAxis(currAngle, Vector3.up);
            Quaternion rm = Quaternion.Euler(90, 0, 0);
#else
            Quaternion rM = Quaternion.AngleAxis(currAngle*Mathf.Deg2Rad, Vector3.forward);
            Quaternion rm = Quaternion.identity;
#endif
            Vector3 shootTo = rM * targetDir;
            Vector3 shootPoint = transform.position + shootTo * initDis;

            GameObject newObj = Instantiate(singleBulletRef, shootPoint, rm, null);
            if (newObj)
            {
                bullet_base newBullet = newObj.GetComponent<bullet_base>();
                if (newBullet)
                {

                    newBullet.InitValue(group, baseDamage, shootTo, targetObj);
                }
            }
            currAngle += angleStep;
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
