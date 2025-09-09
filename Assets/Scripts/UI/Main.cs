using System.Collections;
using System.Collections.Generic;
using BlackCat_UI;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.OpenPanel(UIConst.MainPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
