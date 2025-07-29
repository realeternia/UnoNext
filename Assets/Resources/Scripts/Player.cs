using System;
using System.Collections.Generic;
using System.Drawing;
using UnoGame;
using UnityEngine;
using Unity.VisualScripting;

namespace UnoGame
{
    public class Player : MonoBehaviour
    {
        public int id;
        public string playerName;
        public List<GameObject> cards = new List<GameObject>();
        public GameObject baseMent;
        public PlayerInfo playerInfo;
        public bool isUpDown;
        public bool isAi;

        public int ID
        {
            get { return id; }
        }
        public string PlayerName
        {
            get { return playerName; }
        }

        private void Start()
        {
            playerInfo.playerNameText.text = playerName;
        }

        public void BeginRound()
        {
            playerInfo.isOnTurn = true;
        }
        public void EndRound()
        {
            playerInfo.isOnTurn = false;
        }

        public void AddCard(int tid)
        {
            // 从Prefab创建一个ChessObj对象
            GameObject chessObj = Instantiate(Resources.Load<GameObject>("Prefabs/ChessObj"));
            float chessWidth = chessObj.GetComponent<Renderer>().bounds.size.x;
            chessObj.transform.SetParent(baseMent.transform, false);

            // 获取Chess组件
            Chess chess = chessObj.GetComponent<Chess>();
            if (chess != null)
            {
                // 设置Chess组件的属性
                chess.id = tid;
                chess.playerId = id;
                // if(isAi)
                //     chess.chessName = "0"; // ai牌不显示
                // else
                    chess.chessName = CardBook.GetCard(tid).icon.ToString();
            }

            //保存到一个队列里
            cards.Add(chessObj);

            if (!isUpDown)
                chessObj.transform.Rotate(0, 90, 0);
            UpdateCards();
        }

        private void RemoveCard(int cardId)
        {
            var index = cards.FindIndex(x => x.GetComponent<Chess>().id == cardId);
            if (index != -1)
            {
                Destroy(cards[index]);
                cards.RemoveAt(index);
            }
            UpdateCards();
        }

        private void UpdateCards()
        {
            var chessWidth = 10;
            //将所有cards物件，水平平放在baseMent上，一排放置
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].transform.SetParent(GameObject.Find("Units").transform, false);
                // 根据baseMent的宽度和缩放计算卡片坐标
                // 根据isUpDown决定是沿X轴还是Z轴排列卡片
                Vector3 cardPosition;
                if (isUpDown)
                {
                    // Z轴排列
                    float baseMentDepth = baseMent.transform.localScale.z;
                    float totalCardDepth = cards.Count * chessWidth;
                    float startZ = -baseMentDepth / 2 + chessWidth / 2;
                    float spacing = cards.Count > 1 ? (baseMentDepth - totalCardDepth) / (cards.Count - 1) : 0;
                    if(spacing > 2)
                        spacing = 2;
                    cardPosition = new Vector3(0, 0, startZ + i * (chessWidth + spacing));
                }
                else
                {
                    // X轴排列
                    float baseMentWidth = baseMent.transform.localScale.z;
                    float totalCardWidth = cards.Count * chessWidth;
                    float startX = -baseMentWidth / 2 + chessWidth / 2;
                    float spacing = cards.Count > 1 ? (baseMentWidth - totalCardWidth) / (cards.Count - 1) : 0;
                    if(spacing > 2)
                        spacing = 2;
                    cardPosition = new Vector3(startX + i * (chessWidth + spacing), 0, 0);
                }
                // 添加baseMent的世界坐标偏移
                cardPosition += baseMent.transform.position + new Vector3(0, i*0.1f, 0);
                cards[i].transform.position = cardPosition;

            }
        }

        public int FindBestSymbol()
        {
            int[] marks = new int[4];
            Array.Clear(marks, 0, 4);
            int count = 0;
            foreach (var cardObj in cards)
            {
                Card cd = CardBook.GetCard(cardObj.GetComponent<Chess>().id);
                if (cd.symble!=5)
                {
                    marks[cd.symble - 1]++;
                    count++;
                }
            }
            if (count>1)
            {
                for (int i = 0; i < marks.Length; i++)
                {
                    if (marks[i] >= count * 2 / 5)
                    {
                        return i + 1;
                    }
                }
            }
            int rt = UnityEngine.Random.Range(1, 4);
            return rt;
        }

        public int CheckCardAI(int deckCardId, ref int symbol,bool hasBonus)
        {
            int rt = -1;
            Card deckCard = CardBook.GetCard(deckCardId);
            if (hasBonus)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    Card pickCard = CardBook.GetCard(cards[i].GetComponent<Chess>().id);
                    if ((deckCard.point == 21 && pickCard.point == 24) || pickCard.point == deckCard.point) //+2，+4
                    {
                        if (pickCard.symble == 5)
                        {
                            symbol = FindBestSymbol();
                        }
                        else
                        {
                            symbol = pickCard.symble;
                        }
                        rt = pickCard.id;
                        RemoveCard(pickCard.id);
                        return rt;
                    }
                }
            }

            for (int i = 0; i < cards.Count; i++)
            {
                int sml = symbol;
                Card pickCard = CardBook.GetCard(cards[i].GetComponent<Chess>().id);
                if (CheckCard(deckCardId, pickCard.id, ref sml))
                {
                    rt = pickCard.id;
                    RemoveCard(pickCard.id);
                    symbol = sml;
                    break;
                }
            }
            return rt;
        }

        public bool CheckCardPlayer(int cid, int checkCardId, ref int symbol)
        {
            int sml = symbol;
            if (CheckCard(cid, checkCardId, ref sml))
            {
                RemoveCard(checkCardId);
                symbol = sml;
                return true;
            }
            return false;
        }

        private bool CheckCard(int deckCardId, int checkCardId, ref int symbol)
        {
            Card deckCard = CardBook.GetCard(deckCardId);
            Card checkCard = CardBook.GetCard(checkCardId);
            if (deckCardId == -1 || cards.Count == 1)
            {
                if (checkCard.point <= 10 && (deckCardId == -1 || checkCard.symble == deckCard.symble))
                {
                    symbol = checkCard.symble;
                    return true;
                }
            }
            else if (deckCard.symble == 5 && checkCard.symble != 5)
            {
                if (checkCard.point == 20 || checkCard.point == 21 || checkCard.point == 22)
                {
                    return false;
                }
                if (checkCard.symble == symbol)
                {
                    return true;
                }
            }
            else if (checkCard.symble == 5 || checkCard.symble == symbol || checkCard.point == deckCard.point)
            {
                if (checkCard.symble == 5)
                {
                    symbol = FindBestSymbol(); //ai情况
                    // ColorForm cf=new ColorForm();
                    // cf.ShowDialog();
                    // symbol = cf.Color;
                }
                else
                {
                    symbol = checkCard.symble;
                }
                return true;
            }
            return false;
        }


    }
}
