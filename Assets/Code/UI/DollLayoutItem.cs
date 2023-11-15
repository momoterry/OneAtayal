using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DollLayoutItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image dollIcon;
    public Image outLine;

    protected RectTransform myRect;
    protected Vector2 initIconPos;

    protected DollLayoutUIBase myMenu;
    protected RectTransform movingRootRT;
    protected Transform originalRoot;
    protected Vector2 originalLocalPos;

    [System.NonSerialized]
    public Doll myDoll;
    [System.NonSerialized]
    public int myGroup;
    [System.NonSerialized]
    public int myIndex;

    public class InitData
    {
        public Doll doll;
        public DollLayoutUIBase menuDL;
        public int group;
        public int index;
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
        myMenu = _data.menuDL;
        myGroup = _data.group;
        myIndex = _data.index;
        myDoll = _data.doll;

        originalRoot = transform.parent;
        originalLocalPos = myRect.localPosition;
    }

    public void OnPointerDown(PointerEventData data)
    {
        transform.SetParent(movingRootRT.transform);
        foreach (Image im in GetComponentsInChildren<Image>())
        {
            im.raycastTarget = false;
        }
        transform.localScale = Vector3.one * 1.5f;

        myMenu.RegisterDragItem(this);
    }

    public void OnPointerUp(PointerEventData data)
    {
        transform.SetParent(originalRoot);
        myRect.localPosition = originalLocalPos;

        foreach (Image im in GetComponentsInChildren<Image>())
        {
            im.raycastTarget = true;
        }
        transform.localScale = Vector3.one;

        myMenu.UnRegisterDragItem(this);
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(movingRootRT, data.position, data.enterEventCamera, out pos);
        myRect.localPosition = pos;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        //print("....PointerEnter !!");
        //outLine.gameObject.SetActive(true);
        //myMenu.OnItemPointerEnter(this);
    }

    public void OnPointerExit(PointerEventData data)
    {
        //print("....Exit !!");
        //outLine.gameObject.SetActive(false);
        //myMenu.OnItemPointerExit(this);
    }

    public void ShowOutline(bool isOn)
    {
        outLine.gameObject.SetActive(isOn);
    }

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

}
