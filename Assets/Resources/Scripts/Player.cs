using System;
using System.Collections.Generic;
using System.Drawing;
using UnoGame;

namespace UnoGame
{
    class Player
    {
        private int id;
        private string name;
        public List<int> cards;
        private int x;
        private int y;
        private int sel;

        public int ID
        {
            get { return id; }
        }
        public string Name
        {
            get { return name; }
        }

        public Player(int id, string name, int x, int y)
        {
            this.id = id;
            this.name = name;
            this.x = x;
            this.y = y;
            sel = -1;
            cards = new List<int>();
        }

        public void AddCard(int tid)
        {
            cards.Add(tid);
        }

        public int FindBestSymbol()
        {
            int[] marks = new int[4];
            Array.Clear(marks, 0, 4);
            int count = 0;
            foreach (int card in cards)
            {
                Card cd = CardBook.GetCard(card);
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
            if (hasBonus)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    Card cdd = CardBook.GetCard(cid);
                    Card cd = CardBook.GetCard(cards[i]);
                    if ((cdd.point == 21 && cd.point == 24) || cd.point == cdd.point) //+2，+4
                    {
                        if (cd.symble == 5)
                        {
                            symbol = FindBestSymbol();
                        }
                        else
                        {
                            symbol = cd.symble;
                        }
                        rt = cd.id;
                        cards.RemoveAt(i);
                        return rt;
                    }
                }
            }

            for (int i = 0; i < cards.Count; i++)
            {
                Card cdd = CardBook.GetCard(cid);
                Card cd = CardBook.GetCard(cards[i]);
                if (cid==-1 || cards.Count == 1)
                {
                    if (cd.point <=10 && (cid==-1||cd.symble ==cdd.symble))
                    {
                        rt = cd.id;
                        symbol = cd.symble;
                        cards.RemoveAt(i);
                        break;
                    }
                }
                else if (cdd.symble == 5 && cd.symble!=5)
                {
                    if (cd.point==20||cd.point==21||cd.point==22)
                    {
                        continue;
                    }
                    if (cd.symble == symbol)
                    {
                        rt = cd.id;
                        cards.RemoveAt(i);
                        break;
                    }

                }
                else if (cd.symble == 5 || cd.symble == symbol || cd.point == cdd.point)
                {
                    if (cd.symble==5)
                    {
                        symbol = FindBestSymbol();
                    }
                    else
                    {
                        symbol = cd.symble;
                    }
                    rt = cd.id;
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
            Card cd = CardBook.GetCard(cards[sel]);
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
                    ColorForm cf=new ColorForm();
                    cf.ShowDialog();
                    symbol = cf.Color;
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

        public bool CheckMouse(int mx, int my)
        {
            int newsel = -1;
            for (int i = 0; i < cards.Count; i++)
            {
                int xoff = i != cards.Count - 1 ? 32 : 48;
                if (mx>=x+32*i&&mx<x+32*i+xoff&&my>=y&&my<y+80)
                {
                    newsel = i;
                    break;
                } 
            }
            if (newsel != sel)
            {
                sel = newsel;
                return true;
            }
            return false;
        }
    }
}
