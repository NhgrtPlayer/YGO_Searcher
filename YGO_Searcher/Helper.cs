using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGO_Searcher
{
    class Helper
    {

        public static T GetSelection<T>(int selectedIndex) where T : IConvertible
        { // Index depuis une TextBox vers l'enum (T) Correspond
            if (selectedIndex > 0)
            {
                return ((T)(object) (1 << (selectedIndex - 1)));
            }
            int Nb = Enum.GetNames(typeof(T)).Length;
            return ((T)(object) ((1 << Nb) - 1));
        }

        public static bool CardTypeIsOfType(CardType CardType, CardType TypeToCheck, bool Exclusive = true)
        {
            if (Exclusive)
            {
                return (CardType == TypeToCheck);
            }
            return ((CardType & TypeToCheck) != 0);
        }

        /*
public static CardType GetCardTypeSelection(int CardTypeSelectedIndex)
{
   if (CardTypeSelectedIndex > 0)
   {
       return ((CardType)(1 << (CardTypeSelectedIndex - 1)));
   }
   int Nb = Enum.GetNames(typeof(CardType)).Length;
   return (CardType)((1 << Nb) - 1);
}

public static MonsterCardType GetMonsterCardTypeSelection(int MonsterCardTypeSelectedIndex)
{
   if (MonsterCardTypeSelectedIndex > 0)
   {
       return ((MonsterCardType)(1 << (MonsterCardTypeSelectedIndex - 1)));
   }
   int Nb = Enum.GetNames(typeof(MonsterCardType)).Length;
   return (MonsterCardType)((1 << Nb) - 1);
}

public static MonsterAttribute GetMonsterAttributeSelection(int selectedIndex)
{
   if (selectedIndex > 0)
   {
       return ((MonsterAttribute)(1 << (selectedIndex - 1)));
   }
   int Nb = Enum.GetNames(typeof(MonsterAttribute)).Length;
   return (MonsterAttribute)((1 << Nb) - 1);
}

public static MonsterType GetMonsterTypeSelection(int selectedIndex)
{
   if (selectedIndex > 0)
   {
       return ((MonsterType)(1 << (selectedIndex - 1)));
   }
   int Nb = Enum.GetNames(typeof(MonsterType)).Length;
   return (MonsterType)((1 << Nb) - 1);
}

public static Monster2ndType GetMonster2ndTypeSelection(int selectedIndex)
{
   if (selectedIndex > 0)
   {
       return ((Monster2ndType)(1 << (selectedIndex - 1)));
   }
   int Nb = Enum.GetNames(typeof(Monster2ndType)).Length;
   return (Monster2ndType)((1 << Nb) - 1);
}

public static SpellType GetSpellTypeSelection(int SpellTypeSelectedIndex)
{
   if (SpellTypeSelectedIndex > 0)
   {
       return ((SpellType)(1 << (SpellTypeSelectedIndex - 1)));
   }
   int Nb = Enum.GetNames(typeof(SpellType)).Length;
   return (SpellType)((1 << Nb) - 1);
}

public static TrapType GetTrapTypeSelection(int TrapTypeSelectedIndex)
{
   if (TrapTypeSelectedIndex > 0)
   {
       return ((TrapType)(1 << (TrapTypeSelectedIndex - 1)));
   }
   int Nb = Enum.GetNames(typeof(TrapType)).Length;
   return (TrapType)((1 << Nb) - 1);
}
*/
        public static int GetLimitationByString(string Str)
        {
            if (Str == null)
                return (3);
            if (Str == "Banned")
                return (0);
            if (Str == "Limited")
                return (1);
            if (Str == "Semi-Limited")
                return (2);
            return (3);
        }

        public static CardType GetCardTypeByString(string Str)
        {
            if (Str.Contains("Spell"))
                return (CardType.SPELL);
            if (Str.Contains("Trap"))
                return (CardType.TRAP);
            return (CardType.MONSTER);
        }

        public static MonsterCardType GetMonsterCardTypeByString(string Str)
        {
            if (Str.Contains("Effect"))
                return (MonsterCardType.EFFECT);
            if (Str.Contains("Normal"))
                return (MonsterCardType.NORMAL);
            if (Str.Contains("Ritual"))
                return (MonsterCardType.RITUAL);
            if (Str.Contains("Fusion"))
                return (MonsterCardType.FUSION);
            if (Str.Contains("Synchro"))
                return (MonsterCardType.SYNCHRO);
            if (Str.Contains("XYZ"))
                return (MonsterCardType.XYZ);
            if (Str.Contains("Link"))
                return (MonsterCardType.LINK);
            return (MonsterCardType.EFFECT);
        }

        public static MonsterType GetMonsterTypeByString(string Str)
        {
            try
            {
                Enum.TryParse(Str.Replace('-', '_').Replace(' ', '_').ToUpper(), out MonsterType ToReturn);
                return (ToReturn);
            }
            catch (Exception e)
            {
                return (0);
            }
            /*
             * if (Str == "Aqua")
                return (MonsterType.AQUA);
            if (Str == "Beast")
                return (MonsterType.BEAST);
            if (Str == "Beast-Warrior")
                return (MonsterType.BEAST_WARRIOR);
            if (Str == "Creator-God")
                return (MonsterType.CREATOR_GOD);
            if (Str == "Cyberse")
                return (MonsterType.CYBERSE);
            if (Str == "Dinosaur")
                return (MonsterType.DINOSAUR);
            if (Str == "Divine-Beast")
                return (MonsterType.DIVINE_BEAST);
            if (Str == "Dragon")
                return (MonsterType.DRAGON);
            if (Str == "Fairy")
                return (MonsterType.FAIRY);
            if (Str == "Fiend")
                return (MonsterType.FIEND);
            if (Str == "Fish")
                return (MonsterType.FISH);
            if (Str == "Insect")
                return (MonsterType.INSECT);
            if (Str == "Machine")
                return (MonsterType.MACHINE);
            if (Str == "Plant")
                return (MonsterType.PLANT);
            if (Str == "Psychic")
                return (MonsterType.PSYCHIC);
            if (Str == "Pyro")
                return (MonsterType.PYRO);
            if (Str == "Reptile")
                return (MonsterType.REPTILE);
            if (Str == "Rock")
                return (MonsterType.ROCK);
            if (Str == "Sea Serpent")
                return (MonsterType.SEA_SERPENT);
            if (Str == "Spellcaster")
                return (MonsterType.SPELLCASTER);
            if (Str == "Thunder")
                return (MonsterType.THUNDER);
            if (Str == "Warrior")
                return (MonsterType.WARRIOR);
            if (Str == "Winged Beast")
                return (MonsterType.WINGED_BEAST);
            return (0);
            */
        }

        public static Monster2ndType GetMonster2ndCardTypeByString(string Str)
        {
            Monster2ndType ToReturn = 0;

            if (Str.Contains("Tuner"))
                ToReturn |= Monster2ndType.TUNER;
            if (Str.Contains("Spirit"))
                ToReturn |= Monster2ndType.SPIRIT;
            if (Str.Contains("Flip"))
            {
                ToReturn |= Monster2ndType.FLIP;
                ToReturn |= Monster2ndType.EFFECT;
            }
            if (Str.Contains("Toon"))
                ToReturn |= Monster2ndType.TOON;
            if (Str.Contains("Union"))
                ToReturn |= Monster2ndType.UNION;
            if (Str.Contains("Gemini"))
            {
                ToReturn |= Monster2ndType.GEMINI;
                ToReturn |= Monster2ndType.EFFECT;
            }
            if (Str.Contains("Effect"))
                ToReturn |= Monster2ndType.EFFECT;
            if (Str.Contains("Pendulum"))
                ToReturn |= Monster2ndType.PENDULUM;

            return (ToReturn);
        }

        public static MonsterAttribute GetMonsterAttributeByString(string Str)
        {
            try
            {
                Enum.TryParse(Str.ToUpper(), out MonsterAttribute ToReturn);
                return (ToReturn);
            }
            catch (Exception)
            {
                return (0);
            }
        }

        public static SpellType GetSpellTypeByString(string Str)
        {
            if (Str == "Field")
                return (SpellType.FIELD);
            if (Str == "Equip")
                return (SpellType.EQUIP);
            if (Str == "Continuous")
                return (SpellType.CONTINUOUS);
            if (Str == "Quick-Play")
                return (SpellType.QUICK_PLAY);
            if (Str == "Ritual")
                return (SpellType.RITUAL);
            return (SpellType.NORMAL);
        }

        public static TrapType GetTrapTypeByString(string Str)
        {
            if (Str == "Counter")
                return (TrapType.COUNTER);
            if (Str == "Continuous")
                return (TrapType.CONTINUOUS);
            return (TrapType.NORMAL);
        }

        public static string ToString(MonsterCardType MonsterCardType, MonsterType MonsterType, Monster2ndType Monster2ndType, MonsterAttribute MonsterAttribute)
        {
            string ToReturn = "";

            if (Enum.IsDefined(MonsterAttribute.GetType(), MonsterAttribute))
                ToReturn += '[' + MonsterAttribute.ToString() + "] ";

            switch (MonsterType)
            {
                case MonsterType.AQUA:
                    ToReturn += "Aqua";
                    break;
                case MonsterType.BEAST:
                    ToReturn += "Beast";
                    break;
                case MonsterType.BEAST_WARRIOR:
                    ToReturn += "Beast-Warrior";
                    break;
                case MonsterType.CREATOR_GOD:
                    ToReturn += "Creator-God";
                    break;
                case MonsterType.CYBERSE:
                    ToReturn += "Cyberse";
                    break;
                case MonsterType.DINOSAUR:
                    ToReturn += "Dinosaur";
                    break;
                case MonsterType.DIVINE_BEAST:
                    ToReturn += "Divine-Beast";
                    break;
                case MonsterType.DRAGON:
                    ToReturn += "Dragon";
                    break;
                case MonsterType.FAIRY:
                    ToReturn += "Fairy";
                    break;
                case MonsterType.FIEND:
                    ToReturn += "Fiend";
                    break;
                case MonsterType.FISH:
                    ToReturn += "Fish";
                    break;
                case MonsterType.INSECT:
                    ToReturn += "Insect";
                    break;
                case MonsterType.MACHINE:
                    ToReturn += "Machine";
                    break;
                case MonsterType.PLANT:
                    ToReturn += "Plant";
                    break;
                case MonsterType.PSYCHIC:
                    ToReturn += "Psychic";
                    break;
                case MonsterType.PYRO:
                    ToReturn += "Pyro";
                    break;
                case MonsterType.REPTILE:
                    ToReturn += "Reptile";
                    break;
                case MonsterType.ROCK:
                    ToReturn += "Rock";
                    break;
                case MonsterType.SEA_SERPENT:
                    ToReturn += "Sea Serpent";
                    break;
                case MonsterType.SPELLCASTER:
                    ToReturn += "Spellcaster";
                    break;
                case MonsterType.THUNDER:
                    ToReturn += "Thunder";
                    break;
                case MonsterType.WARRIOR:
                    ToReturn += "Warrior";
                    break;
                case MonsterType.WINGED_BEAST:
                    ToReturn += "Winged-Beast";
                    break;
                case MonsterType.WYRM:
                    ToReturn += "Wyrm";
                    break;
                case MonsterType.ZOMBIE:
                    ToReturn += "Zombie";
                    break;
                default:
                    return ("");
            }

            switch (Monster2ndType)
            {
                case Monster2ndType.FLIP:
                    ToReturn += "/Flip";
                    break;
                case Monster2ndType.GEMINI:
                    ToReturn += "/Gemini";
                    break;
                case Monster2ndType.PENDULUM:
                    ToReturn += "/Pendulum";
                    break;
                case Monster2ndType.SPIRIT:
                    ToReturn += "/Spirit";
                    break;
                case Monster2ndType.TOON:
                    ToReturn += "/Toon";
                    break;
                case Monster2ndType.TUNER:
                    ToReturn += "/Tuner";
                    break;
                case Monster2ndType.UNION:
                    ToReturn += "/Union";
                    break;
            }

            switch (MonsterCardType)
            {
                case MonsterCardType.NORMAL:
                    ToReturn += "/Normal";
                    break;
                case MonsterCardType.EFFECT:
                    ToReturn += "/Effect";
                    break;
                case MonsterCardType.RITUAL:
                    ToReturn += "/Ritual";
                    break;
                case MonsterCardType.FUSION:
                    ToReturn += "/Fusion";
                    break;
                case MonsterCardType.SYNCHRO:
                    ToReturn += "/Synchro";
                    break;
                case MonsterCardType.XYZ:
                    ToReturn += "/Xyz";
                    break;
                case MonsterCardType.LINK:
                    ToReturn += "/Link";
                    break;
            }

            return (ToReturn);
        }

        public static string ToString(SpellType Spell)
        {
            switch (Spell)
            {
                case SpellType.NORMAL:
                    return ("Normal Spell Card");
                case SpellType.FIELD:
                    return ("Field Spell Card");
                case SpellType.EQUIP:
                    return ("Equipement Spell Card");
                case SpellType.CONTINUOUS:
                    return ("Continuous Spell Card");
                case SpellType.QUICK_PLAY:
                    return ("Quick-Play Spell Card");
                case SpellType.RITUAL:
                    return ("Ritual Spell Card");
                default:
                    return ("Spell Card");
            }
        }

        public static string ToString(TrapType Trap)
        {
            switch (Trap)
            {
                case TrapType.NORMAL:
                    return ("Normal Trap Card");
                case TrapType.CONTINUOUS:
                    return ("Continuous Trap Card");
                case TrapType.COUNTER:
                    return ("Counter Trap Card");
                default:
                    return ("Trap Card");
            }
        }

        public static bool CheckString(string Str, string Substr)
        {
            return (Str.ToLower().Contains(Substr.ToLower()));
        }

        public static void ExportDeck(List<Card> Deck, string FilePath)
        {
            string Text = "";
            List<string> MainDeckIds = new List<string>();
            List<string> ExtraDeckIds = new List<string>();

            foreach (var card in Deck)
            {
                if (card.Id != null && card.DeckPart == DeckPart.MAIN_DECK)
                    MainDeckIds.Add(card.Id.ToString());
                else if (card.Id != null && card.DeckPart == DeckPart.EXTRA_DECK)
                    ExtraDeckIds.Add(card.Id.ToString());
            }

            Text += "#created by ...\n";
            Text += "#main\n";
            foreach (var cardId in MainDeckIds)
            {
                Text += cardId + '\n';
            }
            Text += "#extra\n";
            foreach (var cardId in ExtraDeckIds)
            {
                Text += cardId + '\n';
            }
            Text += "!side\n";
            File.WriteAllText(FilePath, Text);
        }

        public static bool IsGoatFormat(string SetTagsStr)
        {
            if (SetTagsStr == null)
                return (false);

            List<string> SetTags = SetTagsStr.Split(',').ToList();
            List<string> GoatSetTags = new List<string>();
            GoatSetTags.Add("LOB");
            GoatSetTags.Add("MRD");
            GoatSetTags.Add("SRL");
            GoatSetTags.Add("MRL");
            GoatSetTags.Add("PSV");
            GoatSetTags.Add("LON");
            GoatSetTags.Add("LOD");
            GoatSetTags.Add("PGD");
            GoatSetTags.Add("MFC");
            GoatSetTags.Add("DCR");
            GoatSetTags.Add("IOC");
            GoatSetTags.Add("AST");
            GoatSetTags.Add("SOD");
            GoatSetTags.Add("RDS");
            GoatSetTags.Add("FET");
            GoatSetTags.Add("DB1");
            GoatSetTags.Add("DB2");
            GoatSetTags.Add("DR1");
            GoatSetTags.Add("TP1");
            GoatSetTags.Add("TP2");
            GoatSetTags.Add("TP3");
            GoatSetTags.Add("TP4");
            GoatSetTags.Add("TP5");
            GoatSetTags.Add("TP6");
            GoatSetTags.Add("BPT");
            GoatSetTags.Add("BPT");
            GoatSetTags.Add("CT1");
            GoatSetTags.Add("DDS");
            GoatSetTags.Add("FMR");
            GoatSetTags.Add("EDS");
            GoatSetTags.Add("DOR");
            GoatSetTags.Add("SDD");
            GoatSetTags.Add("PCY");
            GoatSetTags.Add("TFK");
            GoatSetTags.Add("TSC");
            GoatSetTags.Add("WC4");
            GoatSetTags.Add("DOD");
            GoatSetTags.Add("PCK");
            GoatSetTags.Add("ROD");
            GoatSetTags.Add("PCJ");
            GoatSetTags.Add("DBT");
            GoatSetTags.Add("CMC");
            GoatSetTags.Add("WC5");
            GoatSetTags.Add("MP1");
            GoatSetTags.Add("MOV");
            GoatSetTags.Add("SP1-EN001");
            GoatSetTags.Add("SP1-EN002");
            GoatSetTags.Add("SDY");
            GoatSetTags.Add("SDK");
            GoatSetTags.Add("SDJ");
            GoatSetTags.Add("SDP");
            GoatSetTags.Add("SYE");
            GoatSetTags.Add("SKE");
            GoatSetTags.Add("SD1");
            GoatSetTags.Add("SD2");
            GoatSetTags.Add("SD3");
            GoatSetTags.Add("SD4");

            foreach (var SetTag in SetTags)
            {
                foreach (var GoatSetTag in GoatSetTags)
                {
                    if ((SetTag.Length > GoatSetTag.Length && SetTag.StartsWith(GoatSetTag) && SetTag[GoatSetTag.Length] == '-') || SetTag == GoatSetTag)
                        return (true);
                }
            }

            return (false);
        }
    }
}
