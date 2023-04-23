using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WriteKeyboard : MonoBehaviour
{
    public GameObject txtWrittenGO;
    private TMPro.TextMeshProUGUI textShown;
  

    // Start is called before the first frame update
    void Start()
    {
        textShown = txtWrittenGO.GetComponent<TMPro.TextMeshProUGUI>();
    
    }


    public void clickKey(string value)
    {
        //Debug.Log("click = " + value);
        textShown.text = textShown.text + value;

    }
    
   public void clickDeleteKey()
    {
        //Debug.Log("click delete ");
        if(textShown.text.Length > 0)
        {
            textShown.text = textShown.text.Substring(0, textShown.text.Length - 1);

        }
    }

}
