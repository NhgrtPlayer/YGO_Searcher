using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;

namespace YGO_Searcher
{
    public partial class MainWindow : Window
    {
        bool TitleOnly = false;
        bool DescriptionOnly = false;
        bool ExactWords = false;
        bool SortAlpha = false;
        bool SearchArchetype = true;
        CardType ChosenCardType = CardType.MONSTER | CardType.SPELL | CardType.TRAP;
        MonsterCardType ChosenMonsterCardType = MonsterCardType.EFFECT;
        MonsterAttribute ChosenMonsterAttribute = MonsterAttribute.LIGHT;
        MonsterType ChosenMonsterType = MonsterType.DRAGON;
        Monster2ndType ChosenMonster2ndType = Monster2ndType.EFFECT;
        SpellType ChosenSpellType = SpellType.NORMAL;
        TrapType ChosenTrapType = TrapType.NORMAL;
        int LvlMin = -1, LvlMax = -1,
            AtkMin = -1, AtkMax = -1,
            DefMin = -1, DefMax = -1,
            PendulumScaleMin = -1, PendulumScaleMax = -1,
            CardLimitation = -1;

        Connection Co;

        List<Card> Cards;

        List<Card> ShownCards;
        Card SelectedCard;

        public MainWindow()
        {
            InitializeComponent();
            Co = new Connection();
            Cards = new List<Card>();
            ShownCards = new List<Card>();
            SelectedCard = new Card();

            if (File.Exists("cards.bin"))
                LoadCardsFromFile(null, null);

            ResetFilters(null, null);
            UpdateFilters(null, null);

            UpdateShownCards(Cards.GetRange(0, 15));
        }

        private void CheckTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchCards(sender, e);
            }
        }

        private void GetCardsFromDB(object sender, EventArgs e)
        {
            RequestProgressionWindow window = new RequestProgressionWindow();

            IProgress<double> progressPercentage = new Progress<double>(percentCompleted =>
            {
                window.SetStatusPercent(percentCompleted);
            });

            IProgress<string> progressStatus = new Progress<string>(status =>
            {
                window.AddStatusText(status);
            });

            Cards.Clear();
            Task.Run(async () =>
            {
                await Co.RequestAllCardsAsync(progressPercentage, progressStatus);
                Cards = Co.GetCardsFromAnswer(progressPercentage, progressStatus);
                progressStatus.Report("Updating UI...");
                progressPercentage.Report(0);

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    UpdateShownCards(Cards.GetRange(0, 15));
                    progressPercentage.Report(100);
                    progressStatus.Report("Updating UI : OK !");
                    window.Close();
                }));

                SaveCardsInFile(sender, e);

            });
            window.ShowDialog();
        }

        private void SaveCardsInFile(object sender, EventArgs e)
        {
            Serializer.Save("cards.bin", Cards);
        }

        private void LoadCardsFromFile(object sender, EventArgs e)
        {
            Cards = Serializer.Load<List<Card>>("cards.bin");

            UpdateShownCards(Cards.GetRange(0, 15));
        }

        private void ResetFilters(object sender, RoutedEventArgs e)
        {
            CardType_ComboBox.SelectedIndex = 0;
            MonsterCardType_ComboBox.SelectedIndex = 0;
            MonsterType_ComboBox.SelectedIndex = 0;
            Monster2ndType_ComboBox.SelectedIndex = 0;
            SpellType_ComboBox.SelectedIndex = 0;
            TrapType_ComboBox.SelectedIndex = 0;
            Limit_ComboBox.SelectedIndex = 0;
            MonsterAttribute_ComboBox.SelectedIndex = 0;
            LvlMin_TextBox.Text = "";
            LvlMax_TextBox.Text = "";
            AtkMin_TextBox.Text = "";
            AtkMax_TextBox.Text = "";
            DefMin_TextBox.Text = "";
            DefMax_TextBox.Text = "";
            PendulumScalesMin_TextBox.Text = "";
            PendulumScalesMax_TextBox.Text = "";

            UpdateFilters(sender, e);
        }

        private void SearchCards(object sender, RoutedEventArgs e)
        {
            UpdateFilters(sender, e);

            List<Card> Result = Cards;
            /*
            string toShow = "";

            toShow += "TitleOnly : " + TitleOnly.ToString() + '\n';
            toShow += "DescriptionOnly : " + DescriptionOnly.ToString() + '\n';
            toShow += "ExactWords : " + ExactWords.ToString() + '\n';
            toShow += "ChosenCardType : " + ChosenCardType.ToString() + '\n';
            toShow += "ChosenMonsterCardType : " + ChosenMonsterCardType.ToString() + '\n';
            toShow += "ChosenMonsterAttribute : " + ChosenMonsterAttribute.ToString() + '\n';
            toShow += "ChosenMonsterType : " + ChosenMonsterType.ToString() + '\n';
            toShow += "ChosenMonster2ndType : " + ChosenMonster2ndType.ToString() + '\n';
            toShow += "ChosenSpellType : " + ChosenSpellType.ToString() + '\n';
            toShow += "ChosenTrapType : " + ChosenTrapType.ToString() + '\n';
            toShow += "LvlMin : " + LvlMin.ToString() + '\n';
            toShow += "LvlMax : " + LvlMax.ToString() + '\n';
            toShow += "AtkMin : " + AtkMin.ToString() + '\n';
            toShow += "AtkMax : " + AtkMax.ToString() + '\n';
            toShow += "DefMin : " + DefMin.ToString() + '\n';
            toShow += "DefMax : " + DefMax.ToString() + '\n';
            toShow += "CardLimitation : " + CardLimitation.ToString() + '\n';
            toShow += "SortAlpha : " + SortAlpha + '\n';
            MessageBox.Show(toShow);
            */

            Result = Result.FindAll(card => card.CheckCardCriteria(UserInput.Text,
                    TitleOnly,
                    DescriptionOnly,
                    ExactWords,
                    SearchArchetype,
                    ChosenCardType,
                    ChosenMonsterCardType,
                    ChosenMonsterAttribute,
                    ChosenMonsterType,
                    ChosenMonster2ndType,
                    ChosenSpellType,
                    ChosenTrapType,
                    LvlMin, LvlMax,
                    AtkMin, AtkMax,
                    DefMin, DefMax,
                    PendulumScaleMin, PendulumScaleMax,
                    CardLimitation));

            if (SortAlpha)
            {
                Result.Sort((a, b) => (a.Name.CompareTo(b.Name)));
            }
            UpdateShownCards(Result);
        }

        private void UpdateFilters(object sender, EventArgs e)
        {
            TitleOnly = TitleOnly_CheckBox.IsChecked.Value;
            DescriptionOnly = DescriptionOnly_CheckBox.IsChecked.Value;
            ExactWords = ExactWords_CheckBox.IsChecked.Value;
            SortAlpha = (Sorting_ComboBox.SelectedIndex == 0);
            SearchArchetype = SearchArchetype_CheckBox.IsChecked.Value;

            ChosenCardType = Helper.GetSelection<CardType>(CardType_ComboBox.SelectedIndex);
            ChosenMonsterCardType = Helper.GetSelection<MonsterCardType>(MonsterCardType_ComboBox.SelectedIndex);
            ChosenMonsterAttribute = Helper.GetSelection<MonsterAttribute>(MonsterAttribute_ComboBox.SelectedIndex);
            ChosenMonsterType = Helper.GetSelection<MonsterType>(MonsterType_ComboBox.SelectedIndex);
            ChosenMonster2ndType = Helper.GetSelection<Monster2ndType>(Monster2ndType_ComboBox.SelectedIndex);
            ChosenSpellType = Helper.GetSelection<SpellType>(SpellType_ComboBox.SelectedIndex);
            ChosenTrapType = Helper.GetSelection<TrapType>(TrapType_ComboBox.SelectedIndex);

            LvlMin = (LvlMin_TextBox.Text == "") ? -1 : int.Parse(LvlMin_TextBox.Text);
            LvlMax = (LvlMax_TextBox.Text == "") ? -1 : int.Parse(LvlMax_TextBox.Text);
            AtkMin = (AtkMin_TextBox.Text == "") ? -1 : int.Parse(AtkMin_TextBox.Text);
            AtkMax = (AtkMax_TextBox.Text == "") ? -1 : int.Parse(AtkMax_TextBox.Text);
            DefMin = (DefMin_TextBox.Text == "") ? -1 : int.Parse(DefMin_TextBox.Text);
            DefMax = (DefMax_TextBox.Text == "") ? -1 : int.Parse(DefMax_TextBox.Text);
            PendulumScaleMin = (PendulumScalesMin_TextBox.Text == "") ? -1 : int.Parse(PendulumScalesMin_TextBox.Text);
            PendulumScaleMax = (PendulumScalesMax_TextBox.Text == "") ? -1 : int.Parse(PendulumScalesMax_TextBox.Text);

            CardLimitation = Limit_ComboBox.SelectedIndex - 1;

            MonsterCardType_ComboBox.Visibility = Helper.CardTypeIsOfType(ChosenCardType, CardType.MONSTER) ? Visibility.Visible : Visibility.Collapsed;
            MonsterType_WrapPanel.Visibility = Helper.CardTypeIsOfType(ChosenCardType, CardType.MONSTER) ? Visibility.Visible : Visibility.Collapsed;
            MonsterAttributes_WrapPanel.Visibility = Helper.CardTypeIsOfType(ChosenCardType, CardType.MONSTER) ? Visibility.Visible : Visibility.Collapsed;
            SpellType_WrapPanel.Visibility = Helper.CardTypeIsOfType(ChosenCardType, CardType.SPELL) ? Visibility.Visible : Visibility.Collapsed;
            TrapType_WrapPanel.Visibility = Helper.CardTypeIsOfType(ChosenCardType, CardType.TRAP) ? Visibility.Visible : Visibility.Collapsed;
            PendulumScales_WrapPanel.Visibility = ChosenMonster2ndType.HasFlag(Monster2ndType.PENDULUM) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateShownCards(List<Card> NewShownCards)
        {
            ShownCards = NewShownCards;
            CardSearch_ListView.ItemsSource = ShownCards;
            if (CardSearch_ListView.Items.Count > 0)
                CardSearch_ListView.ScrollIntoView(CardSearch_ListView.Items[0]);
        }

        private void UpdateSelectedCard(object sender, MouseEventArgs e)
        {
            if (sender == null || (sender as Grid) == null)
            {
                return;
            }
            SelectedCard = (sender as Grid)?.DataContext as Card;
            CardPreview.DataContext = SelectedCard;
            //CardPreview.UpdateLayout();
        }



    } // END OF MainWindow


    // CONVERTER CLASSES
    public class CardTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((value as Card)?.GetCardTypeString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class MonsterStatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((value as Card)?.GetMonsterStatString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class MonsterStatVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || (value as Card) == null || !(value as Card).Type.HasFlag(CardType.MONSTER))
                return (Visibility.Collapsed);
            return (Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class MonsterLimitVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || (value as Card) == null || (value as Card).Limitation != 3)
                return (Visibility.Collapsed);
            return (Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
