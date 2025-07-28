using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;
using UnoGame;

namespace UnoGame
{
    internal class CardBook
    {
        private static Dictionary<int, Card> cards;
        private static Dictionary<int, Texture> cardImages;

        static CardBook()
        {
            Load();
        }

        static public Card GetCard(int id)
        {
            if (cards.ContainsKey(id))
            {
                return cards[id];    
            }
            return new Card();
        }

        static public Texture GetCardImage(int id)
        {
            if (!cardImages.ContainsKey(id))
            {
                cardImages.Add(id, Resources.Load<Texture>("ChessPic/" + id));
            }
            return cardImages[id];
        }

        private static void Load()
        {
            cards = new Dictionary<int, Card>();
            cardImages = new Dictionary<int, Texture>();
            TextAsset csvAsset = Resources.Load<TextAsset>("Scripts/cardCfg");
            using (MemoryStream stream = new MemoryStream(csvAsset.bytes))
            {
                CsvDataReader dr = new CsvDataReader(stream);
                while (dr.Read())
                {
                    Card item = new Card();
                    item.id = int.Parse(dr["id"]);
                    item.name = dr["name"];
                    item.point = int.Parse(dr["point"]);
                    item.symble = int.Parse(dr["symble"]);
                    item.icon = int.Parse(dr["icon"]);

                    cards.Add(item.id, item);
                }
                dr.Close();
            }
        }
    }
}
