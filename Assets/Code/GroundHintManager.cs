using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHintManager : MonoBehaviour
{
    //public Sprite SquareSprite;
    public GameObject SquareHintRef;

    static private GroundHintManager instance;
    static public GroundHintManager GetInstance() { return instance; }

    private void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 GroundHintManager 存在 ");
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowSquareHint(Vector3 vCenter, Vector3 vDir, Vector2 size, float duration)
    {
        GameObject so = Instantiate(SquareHintRef, vCenter, Quaternion.Euler(90.0f, Vector3.SignedAngle(Vector3.back, vDir, Vector3.up), 0));
        //so.transform.localScale = new Vector3(size.x, size.y, 1.0f);
        GroundHintSquare gs = so.GetComponent<GroundHintSquare>();
        gs.SetSize(size.x, size.y);
        if (duration >= 0)
        {
            FlashFX ff = so.AddComponent<FlashFX>();
            ff.LifeTime = duration;
        }
    }

    public void ShowSquareHint(Vector3 vCenter, Vector3 vDir, Vector2 size, float duration, Color color)
    {
        GameObject so = Instantiate(SquareHintRef, vCenter, Quaternion.Euler(90.0f, Vector3.SignedAngle(Vector3.back, vDir, Vector3.up), 0));
        GroundHintSquare gs = so.GetComponent<GroundHintSquare>();
        gs.SetSize(size.x, size.y);
        if (duration >= 0) {
            FlashFX ff = so.AddComponent<FlashFX>();
            ff.LifeTime = duration;
        }
        foreach (SpriteRenderer sr in so.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = color;
        }
    }
}
