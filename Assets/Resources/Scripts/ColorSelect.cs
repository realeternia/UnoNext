using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelect : MonoBehaviour
{
    public int checkId;
    public Button buttonRed;
    public Button buttonBlue;
    public Button buttonGreen;
    public Button buttonYellow;
    
    private string[] datas = {"", "Red", "Green", "Yellow", "Cyan"};
    public event System.Action<int> OnColorSelected;

    // Start is called before the first frame update
    void Start()
    {
        if (buttonRed != null)
            buttonRed.onClick.AddListener(() => GameManager.Instance.OnSelectColor(checkId, 1));
        if (buttonGreen != null)
            buttonGreen.onClick.AddListener(() => GameManager.Instance.OnSelectColor(checkId, 2));
        if (buttonYellow != null)
            buttonYellow.onClick.AddListener(() => GameManager.Instance.OnSelectColor(checkId, 3));
        if (buttonBlue != null)
            buttonBlue.onClick.AddListener(() => GameManager.Instance.OnSelectColor(checkId, 4));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
