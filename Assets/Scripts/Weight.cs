using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Weight : MonoBehaviour {

    Text weighttext;

    public Image bagBar;

    float startW;

    float W;

    //private void Start()
    //{
    //    bagBar = GetComponent<Image>();
    //}
    public void Awake()
    {
        weighttext = GetComponent<Text>();
        
    }
    public void Update()
    {
        
        weighttext.text = "Weight: " + Player.instance.current_weight + "/" + Player.instance.CAPACITY ;
        bagBar.fillAmount = (float)Player.instance.current_weight / (float)Player.instance.CAPACITY ;
    }

}
