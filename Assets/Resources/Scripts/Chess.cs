using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chess : MonoBehaviour, IPointerClickHandler
{
    public int id;
    public string chessName = "9";
    public Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        // 创建材质实例
        Material newMaterial = new Material(rend.sharedMaterial);
        newMaterial.mainTexture = Resources.Load<Texture>("ChessPic/" + chessName);
        rend.material = newMaterial; // 这会为这个渲染器创建一个独立的材质实例
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //rend.material.mainTexture = Resources.Load<Texture>("ChessPic/" + UnityEngine.Random.Range(1, 54).ToString());
        Debug.Log("Scene Obj Click");
    }
}
