using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using ChineseWordGame.Common;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ChineseWordGame
{
    public partial class EndPage : PhoneApplicationPage
    {
        private const string scoreTitleFormat = "得到：{0}分";
        private const string levelTitleFormat = "完成：{0}关";
        private const string TopScoreFormat = "第{0}名：  {1}";
        private int m_currentScore;
        private int m_currentLevel;
        private int m_remainingScoreTime;
        private int m_remainingLevelTime;
        private DispatcherTimer m_scoreTimer = new DispatcherTimer();
        private DispatcherTimer m_levelTimer = new DispatcherTimer();

        public EndPage()
        {
            this.InitializeComponent();
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            m_currentLevel = Convert.ToInt32(settings["currentLevel"]);
            m_currentScore = Convert.ToInt32(settings["currentScore"]);
            score.Text = string.Format(scoreTitleFormat, m_currentScore);
            level.Text = string.Format(levelTitleFormat, m_currentLevel);
            currentLevel.Maximum = Constants.TotalLevelCount * 10;
            currentScore.Maximum = Constants.MaxScore;
            currentLevel.Value = 0;
            currentScore.Value = 0;
            RenderScoreBar(m_currentScore);
            RenderLevelBar(m_currentLevel);
            ListTop5Scores();
        }

        private void OnLevelTimer(object sender, object e)
        {
            m_remainingLevelTime--;
            currentLevel.Value++;
            if (m_remainingLevelTime == 0)
            {
                m_levelTimer.Stop();
            }
        }

        private void OnScoreTimer(object sender, object e)
        {
            m_remainingScoreTime--;
            currentScore.Value += 5;
            if (m_remainingScoreTime == 0)
            {
                m_scoreTimer.Stop();
            }
        }


        private void RenderLevelBar(int curLevel)
        {
            if (curLevel > 0)
            {
                m_remainingLevelTime = curLevel*10;
                m_levelTimer.Interval = TimeSpan.FromMilliseconds(100);
                m_levelTimer.Tick += OnLevelTimer;
                m_levelTimer.Start();
            }
        }

        private void RenderScoreBar(int curScore)
        {
            if (curScore > 0)
            {
                m_remainingScoreTime = curScore/5;
                m_scoreTimer.Interval = TimeSpan.FromMilliseconds(20);
                m_scoreTimer.Tick += OnScoreTimer;
                m_scoreTimer.Start();
            }

        }

        private void ListTop5Scores()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            top1.Text = string.Format(TopScoreFormat, Constants.ChineseNumbers[0], settings["topScore1"]);
            top2.Text = string.Format(TopScoreFormat, Constants.ChineseNumbers[1], settings["topScore2"]);
            top3.Text = string.Format(TopScoreFormat, Constants.ChineseNumbers[2], settings["topScore3"]);
            top4.Text = string.Format(TopScoreFormat, Constants.ChineseNumbers[3], settings["topScore4"]);
            top5.Text = string.Format(TopScoreFormat, Constants.ChineseNumbers[4], settings["topScore5"]);
            
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

        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void AdjustControls()
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

        private void EndPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustControls();

        }

        private void EndPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            //currentLevel.Value = Convert.ToDouble(settings["currentLevel"]);
            //passedLevel.Text = currentLevel.Value.ToString();
            //rank.Text = ScoreToPerformanceText((int)settings["currentScore"]);
            //RenderPerformanceText();

        }
    }
}