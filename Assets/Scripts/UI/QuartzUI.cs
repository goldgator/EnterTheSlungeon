using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuartzUI : MonoBehaviour
{
    private Wallet monitoredWallet;

    [SerializeField]
    private TMP_Text voidNum; 
    [SerializeField]
    private TMP_Text timeNum; 
    [SerializeField]
    private TMP_Text spaceNum; 


    // Start is called before the first frame update
    void Start()
    {
        monitoredWallet = Player.Instance.GetWallet();
    }

    // Update is called once per frame
    void Update()
    {
        voidNum.text = monitoredWallet.VoidQuartz.ToString();
        timeNum.text = monitoredWallet.TimeQuartz.ToString();
        spaceNum.text = monitoredWallet.SpaceQuartz.ToString();
    }
}
