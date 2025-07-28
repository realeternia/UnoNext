using System;
using System.Collections.Generic;
using System.Drawing;
using UnoGame;
using UnityEngine;

namespace UnoGame
{
    public class Player : MonoBehaviour
    {
        public float cardScale = 1f;   // 卡片缩放比例
    
        private int id;
        private string playerName;
        public List<GameObject> cards;
        private GameObject baseMent;
        private int sel;
        private bool isUpDown;

        public int ID
        {
            get { return id; }
        }
        public string PlayerName
        {
            get { return playerName; }
        }

        public Player(int id, string name, bool upDown, GameObject region)
        {
            this.id = id;
            playerName = name;
            isUpDown = upDown;
            baseMent = region;
            sel = -1;
            cards = new List<GameObject>();
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
                chess.chessName = CardBook.GetCard(tid).icon.ToString();
            }

            //保存到一个队列里
            cards.Add(chessObj);

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
                    float totalCardDepth = cards.Count * chessWidth * cardScale;
                    float startZ = -baseMentDepth / 2 + chessWidth * cardScale / 2;
                    float spacing = cards.Count > 1 ? (baseMentDepth - totalCardDepth) / (cards.Count - 1) : 0;
                    cardPosition = new Vector3(0, 0, startZ + i * (chessWidth * cardScale + spacing));
                }
                else
                {
                    // X轴排列
                    float baseMentWidth = baseMent.transform.localScale.x;
                    float totalCardWidth = cards.Count * chessWidth * cardScale;
                    float startX = -baseMentWidth / 2 + chessWidth * cardScale / 2;
                    float spacing = cards.Count > 1 ? (baseMentWidth - totalCardWidth) / (cards.Count - 1) : 0;
                    cardPosition = new Vector3(startX + i * (chessWidth * cardScale + spacing), 0, 0);
                }
                // 添加baseMent的世界坐标偏移
                cardPosition += baseMent.transform.position;
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

        public int CheckCard(int cid, ref int symbol,bool hasBonus)
        {
            int rt = -1;
            Card comprCard = CardBook.GetCard(cid);
            if (hasBonus)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    Card pickCard = CardBook.GetCard(cards[i].GetComponent<Chess>().id);
                    if ((comprCard.point == 21 && pickCard.point == 24) || pickCard.point == comprCard.point) //+2，+4
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
                        cards.RemoveAt(i);
                        return rt;
                    }
                }
            }

            for (int i = 0; i < cards.Count; i++)
            {
                Card pickCard = CardBook.GetCard(cards[i].GetComponent<Chess>().id);
                if (cid==-1 || cards.Count == 1)
                {
                    if (pickCard.point <=10 && (cid==-1||pickCard.symble ==comprCard.symble))
                    {
                        rt = pickCard.id;
                        symbol = pickCard.symble;
                        cards.RemoveAt(i);
                        break;
                    }
                }
                else if (comprCard.symble == 5 && pickCard.symble!=5)
                {
                    if (pickCard.point==20||pickCard.point==21||pickCard.point==22)
                    {
                        continue;
                    }
                    if (pickCard.symble == symbol)
                    {
                        rt = pickCard.id;
                        cards.RemoveAt(i);
                        break;
                    }

                }
                else if (pickCard.symble == 5 || pickCard.symble == symbol || pickCard.point == comprCard.point)
                {
                    if (pickCard.symble==5)
                    {
                        symbol = FindBestSymbol();
                    }
                    else
                    {
                        symbol = pickCard.symble;
                    }
                    rt = pickCard.id;
                    cards.RemoveAt(i);
                    break;
                }
            }
            return rt;
        }

        public int CheckSelectCard(int cid, ref int symbol)
        {
            if (sel==-1)
            {
                return -1;
            }

            Card cdd = CardBook.GetCard(cid);
            Card cd = CardBook.GetCard(cards[sel].GetComponent<Chess>().id);
            if (cid == -1 || cards.Count == 1)
            {
                if (cd.point <= 10 && (cid == -1 || cd.symble == cdd.symble))
                {
                    cards.RemoveAt(sel);
                    sel = -1;
                    symbol = cd.symble;
                    return cd.id;
                }
            }
            else if (cdd.symble == 5 && cd.symble != 5)
            {
                if (cd.point == 20 || cd.point == 21 || cd.point == 22)
                {
                    return -1;
                }
                if (cd.symble == symbol)
                {
                    cards.RemoveAt(sel);
                    sel = -1;
                    return cd.id;
                }
            }
            else if (cd.symble == 5 || cd.symble == symbol || cd.point == cdd.point)
            {
                if (cd.symble == 5)
                {
                    // ColorForm cf=new ColorForm();
                    // cf.ShowDialog();
                    // symbol = cf.Color;
                }
                else
                {
                    symbol = cd.symble;
                }
                cards.RemoveAt(sel);
                sel = -1;
                return cd.id;
            }
            return -1;
        }

    }
}
