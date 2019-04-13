using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace YGO_Searcher
{
    public static class Serializer
    {
        // Save() & Load() -> .ydk file export
        public static void Save(string filePath, object objToSerialize)
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, objToSerialize);
                }
            }
            catch (IOException)
            {
            }
        }

        public static T Load<T>(string filePath) where T : new()
        {
            T rez = new T();

            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    rez = (T)bin.Deserialize(stream);
                }
            }
            catch (IOException)
            {
            }

            return rez;
        }

        // HEADER BLOCK
        // Reserved byte 0x00
        // Version(1)
        // CARDS BLOCK
        // Main Deck Length + Array of IDs
        // Extra Deck Length + Array of IDs
        // Side Deck Length + Array of IDs
        public static string Serialize(List<Card> Deck)
        {
            using (var ms = new MemoryStream())
            {
                void Write(ulong value)
                {
                    if (value == 0)
                        ms.WriteByte(0);
                    else
                    {
                        var bytes = VarLong.GetBytes((ulong)value);
                        ms.Write(bytes, 0, bytes.Length);
                    }
                }

                ms.WriteByte(0);
                Write(1);

                var MainDeckCards = Deck.Where(card => card.DeckPart == DeckPart.MAIN_DECK).ToList();
                var ExtraDeckCards = Deck.Where(card => card.DeckPart == DeckPart.EXTRA_DECK).ToList();

                Write((ulong) MainDeckCards.Count);
                foreach (var card in MainDeckCards)
                    Write(Convert.ToUInt64(card.Id));

                Write((ulong) ExtraDeckCards.Count);
                foreach (var card in ExtraDeckCards)
                    Write(Convert.ToUInt64(card.Id));

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static List<Card> DeserializeDeckString(string deckString, List<Card> Cards)
        {
            List<Card> Deck = new List<Card>();
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(deckString);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Input is not a valid deck string.", e);
            }
            var offset = 0;
            ulong Read()
            {
                if (offset > bytes.Length)
                    throw new ArgumentException("Input is not a valid deck string.");
                var value = VarLong.ReadNext(bytes.Skip(offset).ToArray(), out var length);
                offset += length;
                return value;
            }

            //Zero byte
            offset++;
            //Version - always 1
            ulong Version = Read();

            void AddCard(ulong? dbfId = null)
            {
                dbfId = dbfId ?? Read();
                Card ToAdd = Helper.GetCardById(dbfId.ToString(), Cards);
                if (ToAdd == null)
                    throw new ArgumentException("Cards in Decks are not valid.");
                Deck.Add(ToAdd);
            }

            var MainDeckCount = (int)Read();
            for (var i = 0; i < MainDeckCount; i++)
                AddCard();

            var ExtraDeckCount = (int)Read();
            for (var i = 0; i < ExtraDeckCount; i++)
                AddCard();

            var SideDeckCount = (int)Read();
            for (var i = 0; i < SideDeckCount; i++)
                AddCard();
            /*for (var i = 0; i < SideDeckCount; i++)
            {
                var dbfId = (int)Read();
                var count = (int)Read();
                AddCard(dbfId, count);
            }*/
            return (Deck);
        }
    }
}
