using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnoGame;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private List<int> deckCards;
    public Player[] players;        
    private int round;

    public Chess[] cardPlayedPos;
    private int symbol;
    private bool reverse;
    private int bonus;
    private bool jump;
    private bool onTurn;

    public Button buttonAdd;
    public Image ImgClock;
    public Image SymbolPanel;

    public Sprite spriteC;
    public Sprite spriteCR;
    private string[] datas = {"", "Red", "Green", "Yellow", "Cyan"};

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
        symbol = 0;
        onTurn = false;
        // buttonGet.Visible = true;
        // button1.Hide();
        // buttonStart.Show();

        buttonAdd.onClick.AddListener(OnButtonAddClicked);
        StartCoroutine(DelayedUpdate());

        players[0].BeginRound();
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator DelayedUpdate()
    {
        while (true) // 模拟 Update 的循环
        {    
            yield return new WaitForSeconds(2f);
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
            int checkId = players[id].CheckCardAI(cardPlayedPos[0].id, ref symbol, bonus > 0);
            Debug.Log("ai CheckCardAI deckId:"+cardPlayedPos[0].id + " checkId:"+checkId);
            UpdateSymbolPanelColor();
            Card cardPlayed = CardBook.GetCard(checkId);
            bool hasGetCard = false;
            if (bonus > 0)
            {
                Card lastDeckCard = CardBook.GetCard(cardPlayedPos[0].id);
                if (cardPlayed.point == lastDeckCard.point || (cardPlayed.point == 24 &&lastDeckCard.point==21))
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

            if(checkId>0)
            {
                PlaySound("Sounds/pat");
                AddLastCard(id, checkId);
             //   AddLog(string.Format("{0}出了{1}", players[id].Name, cd.name),cd.point<=10?"White":"Lime");
            }
            else if (!hasGetCard)
            {
                PlaySound("Sounds/book");
                players[id].AddCard(GetCard());
             //   AddLog(string.Format("{0}补牌", players[id].Name), "Gray");
            }

            if(players[id].cards.Count == 1)
                PlaySound(id == 0 ? "Sounds/uno" : "Sounds/uno2", 9);

            AddRound();
        }
        onTurn = false;
    }

    public void OnPlayerSelectCard(int checkId)
    {
        if ((round % 4) == 0)
        {
            onTurn = true;
            var hasCard = players[0].CheckCardPlayer(cardPlayedPos[0].id, checkId, ref symbol);
            UpdateSymbolPanelColor();
            if (hasCard)
            {
                if(bonus > 0)
                {
                    Card pickCard = CardBook.GetCard(checkId);
                    Card lastDeckCard = CardBook.GetCard(cardPlayedPos[0].id);
                    if (pickCard.point == lastDeckCard.point || (pickCard.point == 24 && lastDeckCard.point == 21))
                    {
                    }
                    else
                    {
                    //  AddLog(string.Format("{0}吞下{1}张牌", players[0].Name, bonus), "Yellow");
                        for (int i = 0; i < bonus; i++)
                            players[0].AddCard(GetCard());
                        bonus = 0;
                    }
                }

                {
                    PlaySound("Sounds/pat");
                    AddLastCard(round % 4, checkId);
                //   AddLog(string.Format("{0}出了{1}", players[0].Name, cd.name), cd.point <= 10 ? "White" : "Lime");
                //  doubleBuffedPanel1.Invalidate();
                }

                if(players[0].cards.Count == 1)
                    PlaySound("Sounds/uno", 9);

                AddRound();
            }

            onTurn = false;
        }
    }
    
    private void OnButtonAddClicked()
    {
        // 玩家摸牌
        if ((round % 4) == 0)
        {
            onTurn = true;
            if (bonus > 0)
            {
                for (int i = 0; i < bonus; i++)
                    players[0].AddCard(GetCard());
                bonus = 0;
            }
            PlaySound("Sounds/book");
            players[0].AddCard(GetCard());
            AddRound();
            onTurn = false;
        }
    }

    private void AddRound()
    {
        foreach (Player player in players)
            player.EndRound();

        int add = 1;
        if(jump)
            add = 2;
        jump = false;
        round = !reverse ? (round + add) : (round - add);
        players[round % 4].BeginRound();
        Debug.Log("round:"+round);
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
        cardPlayedPos[3].UpdateVal(cardPlayedPos[2].id);
        cardPlayedPos[2].UpdateVal(cardPlayedPos[1].id);
        cardPlayedPos[1].UpdateVal(cardPlayedPos[0].id);
        cardPlayedPos[0].UpdateVal(cid);

        Card cd = CardBook.GetCard(cid);
        if (cd.point == 22)
        {
            reverse = !reverse;
            if (reverse)
                ImgClock.sprite = spriteCR;
            else
                ImgClock.sprite = spriteC;
            PlaySound("Sounds/wind", 6);
        }
        if (cd.point == 20)
        {
            jump = true;
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

    // 静态变量记录上次播放路径和 clip
    string lastPath = "";
    AudioClip lastClip = null;

    private int lastSoundPriority = -1;
    private float lastSoundTime = 0f;

    public void PlaySound(string path, int prioty = 3)
    {
        float currentTime = Time.time;
        // 如果当前优先级低于上一次且时间间隔小于1秒，则跳过播放
        if (prioty < lastSoundPriority && currentTime - lastSoundTime < 1f)
        {
            return;
        }

        // 更新上次播放信息
        lastSoundPriority = prioty;
        lastSoundTime = currentTime;
    
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (lastPath != path)
        {
            lastPath = path;
            lastClip = Resources.Load<AudioClip>(path);
            if (lastClip != null)
            {
                audioSource.clip = lastClip;
            }
        }

        if (audioSource.clip != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }
    }    
    private void UpdateSymbolPanelColor()
    {
        if (symbol >= 0 && symbol < datas.Length)
        {
            string colorName = datas[symbol];
            if (!string.IsNullOrEmpty(colorName))
            {
                switch (colorName.ToLower())
                {
                    case "red":
                        SymbolPanel.color = Color.red;
                        break;
                    case "green":
                        SymbolPanel.color = Color.green;
                        break;
                    case "yellow":
                        SymbolPanel.color = Color.yellow;
                        break;
                    case "cyan":
                        SymbolPanel.color = Color.cyan;
                        break;
                    default:
                        // 未知颜色
                        break;
                }
            }
        }
    }
}
