using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollCardMenu : MonoBehaviour
{
    [SerializeField]
    protected DollCard theCard;

    public void ShowOneDollCard(DollInstance di)
    {
        theCard.SetupCard(di);
        theCard.gameObject.SetActive(true);
    }

    public void OnCloseDollCard()
    {
        theCard.gameObject.SetActive(false);
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
