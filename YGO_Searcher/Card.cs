using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGO_Searcher
{
    [Flags]
    public enum CardType
    {
        MONSTER = 1 << 0,
        SPELL = 1 << 1,
        TRAP = 1 << 2
    }

    [Flags]
    public enum MonsterCardType
    {
        NORMAL = 1 << 0,
        EFFECT = 1 << 1,
        RITUAL = 1 << 2,
        FUSION = 1 << 3,
        SYNCHRO = 1 << 4,
        XYZ = 1 << 5,
        LINK = 1 << 6
    }

    [Flags]
    public enum MonsterType
    {
        AQUA = 1 << 0,
        BEAST = 1 << 1,
        BEAST_WARRIOR = 1 << 2,
        CREATOR_GOD = 1 << 3,
        CYBERSE = 1 << 4,
        DINOSAUR = 1 << 5,
        DIVINE_BEAST = 1 << 6,
        DRAGON = 1 << 7,
        FAIRY = 1 << 8,
        FIEND = 1 << 9,
        FISH = 1 << 10,
        INSECT = 1 << 11,
        MACHINE = 1 << 12,
        PLANT = 1 << 13,
        PSYCHIC = 1 << 14,
        PYRO = 1 << 15,
        REPTILE = 1 << 16,
        ROCK = 1 << 17,
        SEA_SERPENT = 1 << 18,
        SPELLCASTER = 1 << 19,
        THUNDER = 1 << 20,
        WARRIOR = 1 << 21,
        WINGED_BEAST = 1 << 22,
        WYRM = 1 << 23,
        ZOMBIE = 1 << 24
    }

    [Flags]
    public enum Monster2ndType
    {
        EFFECT = 1 << 0,
        FLIP = 1 << 1,
        GEMINI = 1 << 2,
        PENDULUM = 1 << 3,
        SPIRIT = 1 << 4,
        TOON = 1 << 5,
        TUNER = 1 << 6,
        UNION = 1 << 7
    }

    [Flags]
    public enum MonsterAttribute
    {
        LIGHT = 1 << 0,
        DARK = 1 << 1,
        DIVINE = 1 << 2,
        EARTH = 1 << 3,
        FIRE = 1 << 4,
        WATER = 1 << 5,
        WIND = 1 << 6
    }

    [Flags]
    public enum SpellType
    {
        NORMAL = 1 << 0,
        FIELD = 1 << 1,
        EQUIP = 1 << 2,
        CONTINUOUS = 1 << 3,
        QUICK_PLAY = 1 << 4,
        RITUAL = 1 << 5
    }

    [Flags]
    public enum TrapType
    {
        NORMAL = 1 << 0,
        CONTINUOUS = 1 << 1,
        COUNTER = 1 << 2
    }

    public enum DeckPart
    {
        MAIN_DECK,
        EXTRA_DECK
    }

    [Serializable]
    public class Card
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public int Limitation = 3;

        public CardType Type = 0;
        public DeckPart DeckPart { get; }

        MonsterCardType MonsterCardType = 0;
        MonsterType MonsterType = 0;
        Monster2ndType Monster2ndType = 0;
        MonsterAttribute MonsterAttribute = 0;
        int Level = -1;
        int Atk = -1;
        int Def = -1;
        int PendulumScale = -1;

        SpellType SpellType = 0;

        TrapType TrapType = 0;

        public string ImgUrl { get; set; }
        public string ImgUrlSmall { get; set; }

        //Misc info qui peut être utile pour le tri
        string Archetype = "";

        public Card()
        {
            this.Id = "";
            this.Name = "";
        }
        public Card(JToken Token, bool UseGoatFormat)
        {
            try
            {
                Id = Token.Value<string>("id");
                Name = Token.Value<string>("name");
                Description = Token.Value<string>("desc");
                Limitation = Helper.GetLimitationByString(Token.Value<string>("ban_tcg"));
                if (UseGoatFormat)
                    Limitation = Helper.GetLimitationByString(Token.Value<string>("ban_goat"));

                Type = Helper.GetCardTypeByString(Token.Value<string>("type"));
                DeckPart = DeckPart.MAIN_DECK;

                switch (Type)
                {
                    case CardType.MONSTER:
                        MonsterCardType = Helper.GetMonsterCardTypeByString(Token.Value<string>("type"));
                        if (MonsterCardType == MonsterCardType.FUSION
                            || MonsterCardType == MonsterCardType.SYNCHRO
                            || MonsterCardType == MonsterCardType.XYZ
                            || MonsterCardType == MonsterCardType.LINK)
                        {
                            DeckPart = DeckPart.EXTRA_DECK;
                        }
                        MonsterType = Helper.GetMonsterTypeByString(Token.Value<string>("race"));
                        Monster2ndType = Helper.GetMonster2ndCardTypeByString(Token.Value<string>("type"));
                        MonsterAttribute = Helper.GetMonsterAttributeByString(Token.Value<string>("attribute"));
                        Atk = Token.Value<int>("atk");
                        if (MonsterCardType != MonsterCardType.LINK)
                        {
                            Level = Token.Value<int>("level");
                            Def = Token.Value<int>("def");
                            if (Monster2ndType == Monster2ndType.PENDULUM) {
                                PendulumScale = Token.Value<int>("scale");
                            }
                        }
                        else
                        {
                            Level = Token.Value<int>("linkval");
                        }
                        break;
                    case CardType.SPELL:
                        SpellType = Helper.GetSpellTypeByString(Token.Value<string>("race"));
                        break;
                    case CardType.TRAP:
                        TrapType = Helper.GetTrapTypeByString(Token.Value<string>("race"));
                        break;
                    default:
                        break;
                }

                this.ImgUrl = Token.Value<string>("image_url");
                this.ImgUrlSmall = Token.Value<string>("image_url_small");
                this.Archetype = Token.Value<string>("archetype");
            }
            catch (Exception e)
            {
            }
        }

        public Card(string Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        public string GetCardTypeString()
        {
            switch (Type)
            {
                case CardType.MONSTER:
                    return (Helper.ToString(MonsterCardType, MonsterType, Monster2ndType, MonsterAttribute));
                case CardType.SPELL:
                    return (Helper.ToString(SpellType));
                case CardType.TRAP:
                    return (Helper.ToString(TrapType));
                default:
                    return ("");
            }
        }

        public string GetMonsterStatString()
        {
            string ToReturn = "";

            if (!Type.HasFlag(CardType.MONSTER)) {
                return ("");
            }

            if (MonsterCardType.HasFlag(MonsterCardType.XYZ)) {
                ToReturn = "Rank ";
            } else if (MonsterCardType.HasFlag(MonsterCardType.LINK)) {
                ToReturn = "Rating ";
            } else {
                ToReturn = "Level ";
            }
            ToReturn += Level.ToString();
            ToReturn += " - ATK/" + Atk.ToString();

            if (!MonsterCardType.HasFlag(MonsterCardType.LINK)) {
                ToReturn += " DEF/" + Def.ToString();
            }

            if (Monster2ndType.HasFlag(Monster2ndType.PENDULUM)) {
                ToReturn += " (Scale : " + PendulumScale.ToString() + ')';
            }

            return (ToReturn);
        }

        public bool CheckCardCriteria(string UserInput,
            bool titleOnly,
            bool descriptionOnly,
            bool exactWords,
            bool SearchArchetype,
            CardType ChosenCardType,
            MonsterCardType ChosenMonsterCardType,
            MonsterAttribute ChosenMonsterAttribute,
            MonsterType ChosenMonsterType,
            Monster2ndType ChosenMonster2ndType,
            SpellType ChosenSpellType,
            TrapType ChosenTrapType,
            int LvlMin, int LvlMax,
            int AtkMin, int AtkMax,
            int DefMin, int DefMax,
            int PendulumScaleMin, int PendulumScaleMax,
            int CardLimitation)
        {
            bool ToReturn = ((ChosenCardType & Type) != 0);

            if (SearchArchetype && Archetype != null && ToReturn)
                ToReturn |= Helper.CheckString(Archetype, UserInput);

            if (titleOnly && !descriptionOnly)
                ToReturn &= Helper.CheckString(Name, UserInput);
            else if (descriptionOnly && !titleOnly)
                ToReturn &= Helper.CheckString(Description, UserInput);
            else
                ToReturn &= (Helper.CheckString(Name, UserInput) || Helper.CheckString(Description, UserInput));

            switch (Type)
            {
                case CardType.MONSTER:
                    ToReturn &= ((ChosenMonsterCardType & MonsterCardType) != 0);
                    ToReturn &= ((ChosenMonsterAttribute & MonsterAttribute) != 0);
                    ToReturn &= ((ChosenMonsterType & MonsterType) != 0);
                    ToReturn &= ((ChosenMonster2ndType & Monster2ndType) != 0 || Monster2ndType == 0);
                    if (LvlMin >= 0)
                        ToReturn &= (LvlMin <= Level);
                    if (LvlMax >= 0)
                        ToReturn &= (Level <= LvlMax);
                    if (AtkMin >= 0)
                        ToReturn &= (AtkMin <= Atk);
                    if (AtkMax >= 0)
                        ToReturn &= (Atk <= AtkMax);
                    if (DefMin >= 0)
                        ToReturn &= (DefMin <= Def);
                    if (DefMax >= 0)
                        ToReturn &= (Def <= DefMax);
                    if (Monster2ndType.HasFlag(Monster2ndType.PENDULUM) && PendulumScaleMin >= 0)
                        ToReturn &= (PendulumScaleMin <= PendulumScale);
                    if (Monster2ndType.HasFlag(Monster2ndType.PENDULUM) && PendulumScaleMax >= 0)
                        ToReturn &= (PendulumScale <= PendulumScaleMax);
                    break;
                case CardType.SPELL:
                    ToReturn &= ((ChosenSpellType & SpellType) != 0);
                    break;
                case CardType.TRAP:
                    ToReturn &= ((ChosenTrapType & TrapType) != 0);
                    break;
                default:
                    return (false);
            }
            if (CardLimitation >= 0)
                ToReturn &= (Limitation == CardLimitation);
            return (ToReturn);
        }

        public bool CanAddCardToDeck(List<Card> Deck)
        {
            int SameCard = 0;

            foreach (var card in Deck)
            {
                if (card.Name == Name)
                    SameCard++;
            }

            if (SameCard >= Limitation)
                return (false);

            return (true);
        }
    }
}
