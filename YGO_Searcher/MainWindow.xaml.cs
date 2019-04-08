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
using Microsoft.Win32;

namespace YGO_Searcher
{
    public partial class MainWindow : Window
    {
        bool TitleOnly = false;
        bool DescriptionOnly = false;
        bool ExactWords = false;
        bool SortAlpha = false;
        bool SearchArchetype = true;
        bool UseGOATFormat = false;
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

        List<Card> Deck;

        public MainWindow()
        {
            InitializeComponent();
            Co = new Connection();
            Cards = new List<Card>();
            ShownCards = new List<Card>();
            SelectedCard = new Card();
            Deck = new List<Card>();

            if ((!UseGOATFormat && File.Exists("cards.bin")) || (UseGOATFormat && File.Exists("cards_goat.bin")))
                LoadCardsFromFile(null, null);

            ResetFilters(null, null);
            UpdateFilters(null, null);

            UpdateShownCards(Cards.GetRange(0, 15));

            UpdateDeck();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Deck_ListView.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription("DeckPart"));
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
                Cards = Co.GetCardsFromAnswer(progressPercentage, progressStatus, UseGOATFormat);
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
            if (UseGOATFormat)
                Serializer.Save("cards_goat.bin", Cards);
            else
                Serializer.Save("cards.bin", Cards);
        }

        private void LoadCardsFromFile(object sender, EventArgs e)
        {
            if (UseGOATFormat)
                Cards = Serializer.Load<List<Card>>("cards_goat.bin");
            else
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

            if (sender != null)
                UpdateFilters(sender, e);
        }

        private void SearchCards(object sender, RoutedEventArgs e)
        {
            UpdateFilters(sender, e);

            List<Card> Result = Cards;

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
                Result.Sort((a, b) => (a.Name.CompareTo(b.Name)));

            UpdateShownCards(Result);
        }

        private void UpdateFilters(object sender, EventArgs e)
        {
            TitleOnly = TitleOnly_CheckBox.IsChecked.Value;
            DescriptionOnly = DescriptionOnly_CheckBox.IsChecked.Value;
            ExactWords = ExactWords_CheckBox.IsChecked.Value;
            SortAlpha = (Sorting_ComboBox.SelectedIndex == 0);
            SearchArchetype = SearchArchetype_CheckBox.IsChecked.Value;
            UseGOATFormat = UseGoatFormat.IsChecked;

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

            if (CardType_ComboBox.SelectedIndex == 0)
                ResetFilters(null, null);
        }

        private void UpdateShownCards(List<Card> NewShownCards)
        {
            ShownCards = NewShownCards;
            CardSearch_ListView.ItemsSource = ShownCards;
            if (CardSearch_ListView.Items.Count > 0)
                CardSearch_ListView.ScrollIntoView(CardSearch_ListView.Items[0]);
        }

        private void UpdateGoatFormat_Click(object sender, RoutedEventArgs e)
        {
            UpdateFilters(sender, e);

            if ((!UseGOATFormat && File.Exists("cards.bin")) || (UseGOATFormat && File.Exists("cards_goat.bin")))
                LoadCardsFromFile(null, null);

            SearchCards(sender, e);
        }

        private void ActualizeCards(object sender, EventArgs e)
        {
            SearchCards(sender, null);
        }

        private void UpdateSelectedCard(object sender, MouseEventArgs e)
        {
            if (sender == null) {
                return;
            }
            SelectedCard = (sender as Grid)?.DataContext as Card;
            if (SelectedCard == null) {
                SelectedCard = (sender as DockPanel)?.DataContext as Card;
            }
            CardPreview.DataContext = SelectedCard;
        }

        private void AddCardToDeck(object sender, MouseEventArgs e)
        {
            if (sender == null) {
                return;
            }
            if (((sender as Grid)?.DataContext as Card).CanAddCardToDeck(Deck))
                Deck.Add((sender as Grid)?.DataContext as Card);

            UpdateDeck();
        }

        private void RemoveCardFromDeck(object sender, MouseEventArgs e)
        {
            if (sender == null) {
                return;
            }
            var CardToRemove = (sender as DockPanel)?.DataContext as Card;
            Deck.Remove(CardToRemove);
            UpdateDeck();
        }

        private void UpdateDeck()
        {
            Deck_ListView.ItemsSource = Deck;
            Deck_ListView.Items.Refresh();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Deck_ListView.ItemsSource);
            view.Refresh();
        }

        private void SortDeck_Button_Click(object sender, RoutedEventArgs e)
        {
            //Deck.Sort((a, b) => (a.Name.CompareTo(b.Name)));
            //Deck.Sort((a, b) => (a.Type.CompareTo(b.Type)));
            Deck.Sort((a, b) => (a.CompareTo(b)));
            UpdateDeck();
        }

        private void ClearDeck_Button_Click(object sender, RoutedEventArgs e)
        {
            Deck.Clear();
            UpdateDeck();
        }

        private void ExportDeck_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "YGOPro Deck file (*.ydk)|*.ydk";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (saveFileDialog.ShowDialog() == true)
                Helper.ExportDeck(Deck, saveFileDialog.FileName); // TODO
        }

        private void About_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            About.ShowAboutWindow();
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

    public class CardLimitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || (value as Card) == null)
                return ("/Icons/Limitation/Forbidden.png");
            switch ((value as Card).Limitation)
            {
                case 1:
                    return ("/Icons/Limitation/Limited.png");
                case 2:
                    return ("/Icons/Limitation/Semi-Limited.png");
                default:
                    return ("/Icons/Limitation/Forbidden.png");
                    // On retourne ça même pour les cartes à x3 car on va le cacher pour ces derniers
                    // Si on retourne un truc vide, le programme aime pas ça au runtime
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class CardLimitVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || (value as Card) == null || (value as Card).Limitation >= 3)
                return (Visibility.Collapsed);
            return (Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
