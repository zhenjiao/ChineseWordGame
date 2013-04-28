using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChineseWordGame.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ChineseWordGame
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class EndPage : ChineseWordGame.Common.LayoutAwarePage
    {
        private const string scoreTitleFormat = "得到：{0}分";
        private const string levelTitleFormat = "完成：{0}关";
        private const string TopScoreFormat = "第{0}名：  {1}";

        public EndPage()
        {
            this.InitializeComponent();
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            int curLevel = Convert.ToInt32(roamingSettings.Values["currentLevel"]);
            int curScore = Convert.ToInt32(roamingSettings.Values["currentScore"]);
            score.Text = string.Format(scoreTitleFormat, curScore);
            level.Text = string.Format(levelTitleFormat, curLevel);
            currentLevel.Maximum = Constants.TotalLevelCount * 10;
            currentScore.Maximum = Constants.MaxScore;
            currentLevel.Value = 0;
            currentScore.Value = 0;
            RenderScoreBar(curScore);
            RenderLevelBar(curLevel);
            //currentLevel.Value = Convert.ToDouble(roamingSettings.Values["currentLevel"]);
            //passedLevel.Text = currentLevel.Value.ToString();
            //rank.Text = ScoreToPerformanceText((int)roamingSettings.Values["currentScore"]);
            //RenderPerformanceText();
            ListTop5Scores();
        }

        private async void RenderLevelBar(int level)
        {
            for (int i = 0; i < level * 10; i++)
            {
                currentLevel.Value++;
                await Task.Delay(100);
            }
        }

        private async void RenderScoreBar(int score)
        {
            for (int i = 0; i < score/5; i++)
            {
                currentScore.Value += 5;
                await Task.Delay(20);
            }

        }

        private void ListTop5Scores()
        {
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            top1.Text = string.Format(TopScoreFormat, Constants.ChineseNumbers[0], roamingSettings.Values["topScore1"]);
            top2.Text = string.Format(TopScoreFormat, Constants.ChineseNumbers[1], roamingSettings.Values["topScore2"]);
            top3.Text = string.Format(TopScoreFormat, Constants.ChineseNumbers[2], roamingSettings.Values["topScore3"]);
            top4.Text = string.Format(TopScoreFormat, Constants.ChineseNumbers[3], roamingSettings.Values["topScore4"]);
            top5.Text = string.Format(TopScoreFormat, Constants.ChineseNumbers[4], roamingSettings.Values["topScore5"]);
            
        }

        private string ScoreToPerformanceText(int score)
        {
            if (score < 150)
                return "错别字大王";
            else if (score < 300)
                return "基本合格，继续努力";
            else if (score < 600)
                return "表现不错，继续加油";
            else if (score < 1000)
                return "语文高手";
            else
                return "超级高手";
        }

        //private async void RenderPerformanceText()
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        (rank.RenderTransform as CompositeTransform).SkewY += 0.3;
        //        await Task.Delay(100);
        //    }
        //    for (int i = 0; i < 10; i++)
        //    {
        //        (rank.RenderTransform as CompositeTransform).SkewY -= 0.3;
        //        await Task.Delay(100);
        //    }
        //}

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (!rootFrame.Navigate(typeof(MainPage), null))
            {
                throw new Exception("Failed to load main page");
            }

        }

        private void AdjustControls()
        {
            bool isSnapped = ApplicationView.Value == ApplicationViewState.Snapped;

            if(isSnapped)
            {
                listTitle.Visibility = Visibility.Collapsed;
                top1.Visibility = Visibility.Collapsed;
                top2.Visibility = Visibility.Collapsed;
                top3.Visibility = Visibility.Collapsed;
                top4.Visibility = Visibility.Collapsed;
                top5.Visibility = Visibility.Collapsed;
                currentLevel.Visibility = Visibility.Collapsed;
                currentScore.Visibility = Visibility.Collapsed;
                PlayAgain.Visibility = Visibility.Collapsed;

                root.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                listTitle.Visibility = Visibility.Visible;
                top1.Visibility = Visibility.Visible;
                top2.Visibility = Visibility.Visible;
                top3.Visibility = Visibility.Visible;
                top4.Visibility = Visibility.Visible;
                top5.Visibility = Visibility.Visible;
                currentLevel.Visibility = Visibility.Visible;
                currentScore.Visibility = Visibility.Visible;
                PlayAgain.Visibility = Visibility.Visible;

                var top = this.ActualHeight / 20;
                var bottom = this.ActualHeight / 20;
                var left = this.ActualWidth / 20;
                var right = this.ActualWidth / 20;
                var margin = new Thickness(left, top, right, bottom);
                root.Margin = margin;

                currentScore.Width = (this.ActualWidth - left - right) / 2;
                currentLevel.Width = currentScore.Width / 2;
            }

        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            switch (Windows.UI.ViewManagement.ApplicationView.Value)
            {
                case Windows.UI.ViewManagement.ApplicationViewState.Filled:
                    VisualStateManager.GoToState(this, "Fill", false);
                    break;
                case Windows.UI.ViewManagement.ApplicationViewState.FullScreenLandscape:
                    VisualStateManager.GoToState(this, "FullScreenLandscape", false);
                    break;
                case Windows.UI.ViewManagement.ApplicationViewState.Snapped:
                    VisualStateManager.GoToState(this, "Snapped", false);
                    break;
                case Windows.UI.ViewManagement.ApplicationViewState.FullScreenPortrait:
                    VisualStateManager.GoToState(this, "FullScreenPortrait", false);
                    break;
                default:
                    break;
            }
            AdjustControls();

        }
    }
}
