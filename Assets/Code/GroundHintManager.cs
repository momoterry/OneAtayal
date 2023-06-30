using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHintManager : MonoBehaviour
{
    public Sprite SquareSprite;

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

    public void ShowSquareHint( Vector3 vCenter, Vector3 vDir, Vector2 size, float duration )
    {
        GameObject o = new GameObject("_GroundHint");
        SpriteRenderer sr = o.AddComponent<SpriteRenderer>();
        FlashFX fx = o.AddComponent<FlashFX>();
        fx.LifeTime = duration;
        sr.sprite = SquareSprite;
        sr.sortingLayerName = "GroundEffect";
        sr.color = new Color(1.0f, 0, 0, 0.5f);
        o.transform.position = vCenter;
        o.transform.rotation = Quaternion.Euler(90.0f, Vector3.SignedAngle(Vector3.back, vDir, Vector3.up), 0);
        o.transform.localScale = new Vector3(size.x, size.y, 1.0f);
    }
}
