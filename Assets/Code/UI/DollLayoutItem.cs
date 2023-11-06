using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class DollLayoutItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image dollIcon;

    protected RectTransform myRect;
    protected Vector2 initIconPos;

    protected RectTransform movingRootRT;
    protected Transform originalRoot;
    protected Vector2 originalLocalPos;

    public struct InitData
    {
        public Doll doll;
        public DollLayoutUIBase menuDL;
    }


    private void Awake()
    {
        myRect = GetComponent<RectTransform>();
        initIconPos = dollIcon.rectTransform.localPosition;
    }

    public void Init(InitData _data)
    {
        dollIcon.sprite = _data.doll.icon;
        movingRootRT = _data.menuDL.topRoot;
        originalRoot = transform.parent;
        originalLocalPos = myRect.localPosition;
    }

    public void OnPointerDown(PointerEventData data)
    {
        //print("..DollLayoutItem Point Down!!");
        //dollIcon.transform.SetParent(movingRootRT.transform);

        transform.SetParent(movingRootRT.transform);
    }

    public void OnPointerUp(PointerEventData data)
    {
        //print("..DollLayoutItem Point Up!!");
        //dollIcon.transform.SetParent(myRect.transform);
        transform.SetParent(originalRoot);
        myRect.localPosition = originalLocalPos;

        //dollIcon.rectTransform.localPosition = initIconPos;
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(movingRootRT, data.position, data.enterEventCamera, out pos);
        //print("..Drag Rect Pos " + pos);
        //dollIcon.rectTransform.localPosition = pos;
        myRect.localPosition = pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
