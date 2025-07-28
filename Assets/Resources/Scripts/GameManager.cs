using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnoGame;

public class GameManager : MonoBehaviour
{    
    private List<int> cards;
    private Player[] players;        
    private bool isStart;
    private int round;
    private List<int> lastCard;
    private int symbol;
    private bool reverse;
    private int bonus;
    private bool onTurn;

    // Start is called before the first frame update
    void Start()
    {
        InitCards();
        players = new Player[4];
        players[0] = new Player(0, "宝宝", 300, 550);
        players[1] = new Player(1, "张三疯", 30, 300);
        players[2] = new Player(2, "贝贝", 300, 30);
        players[3] = new Player(3, "雀儿", 750, 300);
        for (int i = 0; i < 7; i++)
        {
            foreach (Player player in players)
            {
                player.AddCard(GetCard());                    
            }
        }
        round = 10000;
        isStart = true;
        lastCard = new List<int>();
        symbol = 0;
        onTurn = false;
        buttonGet.Visible = true;
        button1.Hide();
        buttonStart.Show();
    }

    // Update is called once per frame
    void Update()
    {
        doWork();
        Thread.Sleep(1000);
    }

    private void InitCards()
    {
        cards = new List<int>();
        for (int i = 1; i < 53; i++)
        {
            cards.Add(i);
            cards.Add(i);
        }
        for (int i = 0; i < 4; i++)
        {
            cards.Add(53);
            cards.Add(54);
        }
        cards = RandomShuffle.Process(cards.ToArray());
    }

    private void doWork(object o)
    {
        foreach (Player player in players)
        {
            if (player.cards.Count==0)
            {
                AddLog(string.Format("恭喜{0}获得胜利", player.Name), "Red");
                return;
            }
        }

        onTurn = true;
        int id = (round%4);
        if (id != 0)
        {
            int cid = players[id].CheckCard(lastCard.Count == 0 ? -1 : lastCard[lastCard.Count - 1], ref symbol, bonus > 0);
            Card cd = CardBook.GetCard(cid);
            bool hasGetCard = false;
            if (bonus > 0)
            {
                Card ccd = CardBook.GetCard(lastCard[lastCard.Count - 1]);
                if (cd.point == ccd.point || (cd.point == 24 &&ccd.point==21))
                {
                }
                else
                {
                    AddLog(string.Format("{0}吞下{1}张牌", players[id].Name, bonus), "Yellow");
                    for (int i = 0; i < bonus; i++)
                    {
                        players[id].AddCard(GetCard());
                    }
                    bonus = 0;
                    hasGetCard = true;
                }
            }

            round = !reverse ? (round + 1) : (round - 1);
            if(cid>0)
            {
                AddLastCard(round % 4, cid);
             //   AddLog(string.Format("{0}出了{1}", players[id].Name, cd.name),cd.point<=10?"White":"Lime");
            }
            else if (!hasGetCard)
            {
                players[id].AddCard(GetCard());
             //   AddLog(string.Format("{0}补牌", players[id].Name), "Gray");
            }
        }
        onTurn = false;
    }


    private int GetCard()
    {
        if (cards.Count==0)
        {
            InitCards();
        }
        int cid = cards[0];
        cards.RemoveAt(0);
        return cid;
    }

    private void AddLastCard(int id, int cid)
    {
        lastCard.Add(cid);
        if (lastCard.Count>3)
        {
            lastCard.RemoveAt(0);
        }
        Card cd = CardBook.GetCard(cid);
        if (cd.point == 22)
        {
            round = reverse ? round + 2 : round - 2;
            reverse = !reverse;
        }
        if (cd.point == 20)
        {
            round = reverse ? round - 1 : round + 1;
        }
        
        if (cd.point == 21)
        {
            bonus += 2;
        }
        if (cd.point == 24)
        {
            bonus += 4;
        }
    }


    public void AddLog(string s)
    {

    }
}
