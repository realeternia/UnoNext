using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnoGame;

public class Chess : MonoBehaviour, IPointerClickHandler
{
    public int id;
    public string chessName = "9";
    public int playerId; //0-3
    public Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        // 创建材质实例
        Material newMaterial = new Material(rend.sharedMaterial);
        newMaterial.mainTexture = Resources.Load<Texture>("ChessPic/" + chessName);
        rend.material = newMaterial; // 这会为这个渲染器创建一个独立的材质实例
    }

    public void UpdateVal(int cardId)
    {
        id = cardId;
        if(cardId > 0)
            chessName = CardBook.GetCard(id).icon.ToString();
        else
            chessName = "0";
        playerId = -1;
        rend.material.mainTexture = Resources.Load<Texture>("ChessPic/" + chessName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(playerId == 0) // 玩家牌
        {
            GameManager.Instance.OnPlayerSelectCard(id);
        }
        //rend.material.mainTexture = Resources.Load<Texture>("ChessPic/" + UnityEngine.Random.Range(1, 54).ToString());
    }
}
