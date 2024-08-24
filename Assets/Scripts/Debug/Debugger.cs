using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Debugging{
public class Debugger : MonoBehaviour
{
    private string input = "";
    private bool isReading;
    private GameObject debugCanvas;

    public delegate void DebugCommand(string input);
    public DebugCommand debugCommand;

    void Start(){
        debugCanvas = this.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return) && isReading){
            debugCommand(input.Trim());
            isReading = false;
            input = "";
            toggleUIVisible(false);
        }

        else if(Input.GetKeyUp(KeyCode.Return) && !isReading){
            input = "";
            isReading = true;
            toggleUIVisible(true);
        }

        if(Input.anyKey && isReading){
            input += Input.inputString;
            updateText();
        }

    }

    public void updateText(){
        TextMeshProUGUI tmpro = debugCanvas.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        print(debugCanvas.transform.GetChild(0));
        tmpro.SetText(input);
    }

    public void toggleUIVisible(bool visibility){
        debugCanvas.SetActive(visibility);
    }
}
}