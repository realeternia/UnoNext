using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnoGame;

public class GameManager : MonoBehaviour
{    
    private List<int> deckCards;
    public Player[] players;        
    private bool isStart;
    private int round;
    private List<int> lastCard;
    private int symbol;
    private bool reverse;
    private int bonus;
    private bool onTurn;

    public Button buttonAdd;

    // Start is called before the first frame update
    void Start()
    {
        InitCards();
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
        // buttonGet.Visible = true;
        // button1.Hide();
        // buttonStart.Show();

        buttonAdd.onClick.AddListener(OnButtonAddClicked);
        StartCoroutine(DelayedUpdate());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator DelayedUpdate()
    {
        while (true) // 模拟 Update 的循环
        {
            // 你的逻辑代码
            doWork();

            // 等待 1 秒（不阻塞主线程）
            yield return new WaitForSeconds(1f);
        }
    }

    private void InitCards()
    {
        deckCards = new List<int>();
        for (int i = 1; i < 53; i++)
        {
            deckCards.Add(i);
            deckCards.Add(i);
        }
        for (int i = 0; i < 4; i++)
        {
            deckCards.Add(53);
            deckCards.Add(54);
        }
        deckCards = RandomShuffle.Process(deckCards.ToArray());
    }

    private void OnButtonAddClicked()
    {
        foreach (Player player in players)
        {
            player.AddCard(GetCard());
        }
    }

    private void doWork()
    {
        foreach (Player player in players)
        {
            if (player.cards.Count==0)
            {
              //  AddLog(string.Format("恭喜{0}获得胜利", player.Name), "Red");
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
                 //   AddLog(string.Format("{0}吞下{1}张牌", players[id].Name, bonus), "Yellow");
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
        if (deckCards.Count==0)
        {
            InitCards();
        }
        int cid = deckCards[0];
        deckCards.RemoveAt(0);
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
