using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;
using ChineseWordGame.Common;
using ChineseWordGame.Questions;
using Microsoft.Phone.Controls;

namespace ChineseWordGame
{
    public partial class MainPage : PhoneApplicationPage
    {
        public string m_fileContent;

        private readonly string[] TopScoreKeys = new string[5]
                                        {
                                            "topScore1",
                                            "topScore2",
                                            "topScore3",
                                            "topScore4",
                                            "topScore5",
                                        };

        private const string QuestionPrefixFormat = "第{0}题：";
        private const string TitleFormat = "关： 至少答对{1}题";
        private const string ReplayTitle = "关： 题目回看模式";
        private const string ReplayErrorTitle = "关： 错题回看模式";
        private const string LastLevelTitle = "关： 最后一关，加油";
        private const string LevelUpTitle = "关： 恭喜过关，请进入下一关";
        private const string TrueAnswerImage = "/PhoneApp1;component/assets/true.jpg";
        private const string FalseAnswerImage = "/PhoneApp1;component/assets/false.png";
        private const string LandscapeImage = "/PhoneApp1;resource1.resx/night.jpg";
        private const string PortraitImage = "/PhoneApp1;component/assets/night_vertical.jpg";
        private const string SnapImage = "/PhoneApp1;component/assets/snap.jpg";

        private const int TotalTime = 100;

        private Questions.CombinedQuestion m_currentQuestion;

        private Button[] m_answerButtons;

        private int m_currentQuestionIndex;

        //private Image[] m_timerImages = null;
        private int m_remainingTime;
        private DispatcherTimer m_timer = new DispatcherTimer();
        private DispatcherTimer m_waitTimer = new DispatcherTimer();
        private Button m_disabledButton = null;

        private Image[] m_scoreImages = null;

        private int m_currentLevel = 0;
        private int m_correctAnswerCount = 0;
        private int[] m_timerIntervalInLevels;

        private int m_currentScore = 0;

        private bool m_isCheckingAnswer;

        public MainPage()
        {
            InitializeComponent();
            HideControls();
            InitLevels();
            InitTimerImages();
            InitScoreImages();
            InitAnswerButtions();
            if (Global.PlayMode == PlayMode.Replay)
            {
                Start.Visibility = Visibility.Collapsed;
                StartReplay();
            }
        }

        private void HideControls()
        {
            Start.Visibility = Visibility.Visible;
            Caption1.Visibility = Visibility.Visible;
            Caption2.Visibility = Visibility.Visible;
            Caption3.Visibility = Visibility.Visible;
            Caption4.Visibility = Visibility.Visible;
            Caption5.Visibility = Visibility.Visible;
            Caption6.Visibility = Visibility.Visible;

            pageTitle1.Visibility = Visibility.Collapsed;
            pageTitle2.Visibility = Visibility.Collapsed;
            pageTitle3.Visibility = Visibility.Collapsed;
            QuestionText.Visibility = Visibility.Collapsed;
            AnswerA.Visibility = Visibility.Collapsed;
            AnswerB.Visibility = Visibility.Collapsed;
            AnswerC.Visibility = Visibility.Collapsed;
            AnswerD.Visibility = Visibility.Collapsed;

            Prev.Visibility = Visibility.Collapsed;
            Next.Visibility = Visibility.Collapsed;
        }

        private void ShowControls()
        {
            Start.Visibility = Visibility.Collapsed;
            Caption1.Visibility = Visibility.Collapsed;
            Caption2.Visibility = Visibility.Collapsed;
            Caption3.Visibility = Visibility.Collapsed;
            Caption4.Visibility = Visibility.Collapsed;
            Caption5.Visibility = Visibility.Collapsed;
            Caption6.Visibility = Visibility.Collapsed;

            pageTitle1.Visibility = Visibility.Visible;
            pageTitle2.Visibility = Visibility.Visible;
            pageTitle3.Visibility = Visibility.Visible;
            QuestionText.Visibility = Visibility.Visible;
            AnswerA.Visibility = Visibility.Visible;
            AnswerB.Visibility = Visibility.Visible;
            AnswerC.Visibility = Visibility.Visible;
            AnswerD.Visibility = Visibility.Visible;
        }


        private void GotoNextLevel()
        {
            ResetScoreImages();
            m_currentLevel++;
            m_correctAnswerCount = 0;
            m_currentQuestionIndex = 0;
            pageTitle2.Text = Constants.ChineseNumbers[m_currentLevel - 1];
            if (m_currentLevel < Constants.TotalLevelCount)
            {
                pageTitle3.Text = string.Format(TitleFormat, m_currentLevel, m_currentLevel + Constants.BaseBarForEachLevel - 1);
            }
            else
            {
                pageTitle3.Text = LastLevelTitle;
            }
            ShowNextQuestion();
        }

        private void ResetScoreImages()
        {
            for (int i = 0; i < m_scoreImages.Count(); i++)
            {
                m_scoreImages[i].Visibility = Visibility.Collapsed;
            }
        }

        private void ResetTimer()
        {
            //for(int i = 0; i < m_timerImages.Count(); i++)
            //{
            //    m_timerImages[i].Visibility = Visibility.Visible; 
            //}
            Progress.Value = 0;
            m_timer.Interval = TimeSpan.FromMilliseconds(m_timerIntervalInLevels[m_currentLevel - 1]);
            m_remainingTime = TotalTime;
            m_timer.Start();
        }

        private void OnTimer(object sender, object e)
        {
            m_remainingTime--;
            Progress.Value++;
            //m_timerImages[m_timerImages.Count() - 1 - m_remainingTime].Visibility = Visibility.Collapsed; 
            if (m_remainingTime == 0)
            {
                m_timer.Stop();
                CheckAnswer(-1);
            }
        }

        private void OnWaitTimer(object sender, object e)
        {
            m_waitTimer.Stop();
            if (m_disabledButton != null)
                m_disabledButton.Foreground = new SolidColorBrush(Colors.White);

            if (m_currentQuestionIndex < Constants.QuestionPerLevel)
            {
                ShowNextQuestion();
                m_isCheckingAnswer = false;
            }
            else
            {
                if (m_currentLevel < Constants.TotalLevelCount && m_correctAnswerCount >= Constants.BaseBarForEachLevel + m_currentLevel - 1)
                {
                    levelUp.Visibility = Visibility.Visible;
                    pageTitle3.Text = LevelUpTitle;
                }
                else
                {
                    SaveCurrentScore();
                    m_isCheckingAnswer = false;
                    NavigationService.Navigate(new Uri("/EndPage.xaml", UriKind.Relative));
                }
            }
        }

        private void InitLevels()
        {
            Questions.QuestionManager.Instance.Refresh();
            m_timerIntervalInLevels = new int[Constants.TotalLevelCount];
            m_timerIntervalInLevels[0] = 200;
            m_timerIntervalInLevels[1] = 160;
            m_timerIntervalInLevels[2] = 120;
            m_timerIntervalInLevels[3] = 100;
            m_timerIntervalInLevels[4] = 80;
            m_timerIntervalInLevels[5] = 60;
            m_timerIntervalInLevels[6] = 50;
            m_timerIntervalInLevels[7] = 40;

        }

        private void InitAnswerButtions()
        {
            m_answerButtons = new Button[4];
            m_answerButtons[0] = AnswerA;
            m_answerButtons[1] = AnswerB;
            m_answerButtons[2] = AnswerC;
            m_answerButtons[3] = AnswerD;
        }

        private void InitTimerImages()
        {
            m_timer.Interval = TimeSpan.FromMilliseconds(50);
            m_timer.Tick += OnTimer;
            m_timer.Stop();

            m_waitTimer.Tick += OnWaitTimer;
            m_waitTimer.Stop();

            Progress.Maximum = 100;

        }

        private void InitScoreImages()
        {
            m_scoreImages = new Image[Constants.QuestionPerLevel];
            m_scoreImages[0] = Score1;
            m_scoreImages[1] = Score2;
            m_scoreImages[2] = Score3;
            m_scoreImages[3] = Score4;
            m_scoreImages[4] = Score5;
            m_scoreImages[5] = Score6;
            m_scoreImages[6] = Score7;
            m_scoreImages[7] = Score8;
            m_scoreImages[8] = Score9;
            m_scoreImages[9] = Score10;
        }


        private void ShowNextQuestion()
        {
            m_currentQuestion = Questions.QuestionManager.Instance.GetRandomCombinedQuestion();

            m_currentQuestionIndex++;
            QuestionText.Text = string.Format(QuestionPrefixFormat, m_currentQuestionIndex) + m_currentQuestion.Text;
            AnswerA.Content = "A. " + m_currentQuestion.Answers[0];
            AnswerB.Content = "B. " + m_currentQuestion.Answers[1];
            AnswerC.Content = "C. " + m_currentQuestion.Answers[2];
            AnswerD.Content = "D. " + m_currentQuestion.Answers[3];

            ResetTimer();
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if(Global.PlayMode == PlayMode.Replay)
                return;

            int result = -1;
            switch ((sender as Button).Name)
            {
                case "AnswerA":
                    result = 0;
                    break;
                case "AnswerB":
                    result = 1;
                    break;
                case "AnswerC":
                    result = 2;
                    break;
                case "AnswerD":
                    result = 3;
                    break;
            }
            CheckAnswer(result);
        }

        private void SetAnswer(int result)
        {
            if (result == m_currentQuestion.CorrectAnswer)
            {
                score.Text = Global.QuestionAndAnswers[(m_currentLevel - 1) * Constants.QuestionPerLevel + m_currentQuestionIndex - 1].CurrentScore.ToString();
                Utilities.SetImage(m_scoreImages[m_currentQuestionIndex - 1], Resource1._true);
            }
            else
            {
                Utilities.SetImage(m_scoreImages[m_currentQuestionIndex - 1], Resource1._false);
            }

            for (int i = 0; i < m_answerButtons.Count(); i++)
            {
                m_answerButtons[i].Foreground = new SolidColorBrush(Colors.White);
            }
            m_answerButtons[m_currentQuestion.CorrectAnswer].Foreground = new SolidColorBrush(Colors.Orange);
            m_answerButtons[m_currentQuestion.CorrectAnswer].Content = "☆." + m_currentQuestion.Answers[m_currentQuestion.CorrectAnswer];
            if (result != m_currentQuestion.CorrectAnswer)
            {
                m_answerButtons[result].Foreground = new SolidColorBrush(Colors.DarkGray);
                m_answerButtons[result].Content = "△." + m_currentQuestion.Answers[result];
            }
        }

        private void CheckAnswer(int result)
        {
            if (m_isCheckingAnswer == true)
                return;

            m_timer.Stop();
            m_isCheckingAnswer = true;
            int delayms;

            if (result == m_currentQuestion.CorrectAnswer)
            {
                delayms = 500;
                m_correctAnswerCount++;
                m_currentScore += Constants.BaseScoreOfQuestion + Constants.BaseScoreOfQuestion * (m_currentLevel - 1) * 5 / 10;
                //score.Text = "2000";
                score.Text = m_currentScore.ToString();
                Utilities.SetImage(m_scoreImages[m_currentQuestionIndex - 1], Resource1._true);
            }
            else
            {
                delayms = 2000;
                Utilities.SetImage(m_scoreImages[m_currentQuestionIndex - 1], Resource1._false);
            }
            Global.QuestionAndAnswers.Add(
                new QuestionHistory()
                    {
                        Question = m_currentQuestion,
                        Answer = result,
                        CurrentScore = m_currentScore,
                    });

            m_answerButtons[m_currentQuestion.CorrectAnswer].Foreground = new SolidColorBrush(Colors.Orange);
            m_answerButtons[m_currentQuestion.CorrectAnswer].Content = "☆." + m_currentQuestion.Answers[m_currentQuestion.CorrectAnswer];
            m_disabledButton = m_answerButtons[m_currentQuestion.CorrectAnswer];
            m_waitTimer.Interval = TimeSpan.FromMilliseconds(delayms);
            m_waitTimer.Start();

        }

        private void AddOrSetSettingValue(string key, object value)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains(key))
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key] = value;
            }

        }

        private int[] LoadTopScores()
        {
            int[] topScores = new int[5];
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            for (int i = 0; i < TopScoreKeys.Count(); i++)
            {
                if (settings.Contains(TopScoreKeys[i]))
                {
                    topScores[i] = Convert.ToInt32(settings[TopScoreKeys[i]]);
                }
                else
                {
                    settings.Add(TopScoreKeys[i], 0);
                    topScores[i] = 0;
                }
            }
            settings.Save();
            return topScores;
        }

        private void InsertTopScore(int position, int score)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            for (int i = TopScoreKeys.Count() - 1; i > position; i--)
            {
                settings[TopScoreKeys[i]] = settings[TopScoreKeys[i - 1]];
            }
            settings[TopScoreKeys[position]] = score;
            settings.Save();
        }

        private void SaveCurrentScore()
        {
            AddOrSetSettingValue("currentScore", m_currentScore);
            AddOrSetSettingValue("currentLevel", m_currentLevel);

            var topScores = LoadTopScores();
            for (int i = 0; i < topScores.Count(); i++)
            {
                if (m_currentScore > topScores[i])
                {
                    InsertTopScore(i, m_currentScore);
                    AddOrSetSettingValue("currentRank", i+1);
                    break;
                }
            }
        }

        private void levelUp_Click(object sender, RoutedEventArgs e)
        {
            if (Global.PlayMode == PlayMode.Normal)
            {
                levelUp.Visibility = Visibility.Collapsed;
                GotoNextLevel();
                m_isCheckingAnswer = false;
            }
            else
            {
                NavigationService.Navigate(new Uri("/EndPage.xaml", UriKind.Relative));
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Global.PlayMode = PlayMode.Normal;
            Global.QuestionAndAnswers = new List<QuestionHistory>();
            ShowControls();
            GotoNextLevel();
        }

        public void StartReplay()
        {
            ShowControls();
            ReplayQuestionTo(0);
            if(Global.ReplayMode == ReplayMode.All)
                pageTitle3.Text = ReplayTitle;
            else
            {
                pageTitle3.Text = ReplayErrorTitle;
            }
            levelUp.Content = "返回";
            levelUp.Visibility = Visibility.Visible;
            var nextImgBrush = new ImageBrush();
            Utilities.SetImage(nextImgBrush, Resource1.next);
            Next.Background = nextImgBrush;
            var preImageBrush = new ImageBrush();
            Utilities.SetImage(preImageBrush, Resource1.prev);
            Prev.Background = preImageBrush;
            Next.Visibility = Visibility.Visible;
            Prev.Visibility = Visibility.Visible;
        }

        private void AdjustAnswerControls(double imageHeight, double imageWidth, double buttonHeight, double buttonWidth)
        {
            double perception = imageWidth / imageHeight;
            if (perception < 0.6)
            {
                double textLeft = imageWidth < QuestionText.Width ? 0 : (imageWidth - QuestionText.Width) / 2;
                Canvas.SetLeft(QuestionText, textLeft);
                Canvas.SetTop(QuestionText, imageHeight * 0.25);

                Canvas.SetLeft(AnswerA, 0);
                Canvas.SetTop(AnswerA, imageHeight * 0.32);

                Canvas.SetLeft(AnswerC, 0);
                Canvas.SetTop(AnswerC, imageHeight * 0.32 + buttonHeight);

                Canvas.SetLeft(AnswerB, buttonWidth);
                Canvas.SetTop(AnswerB, imageHeight * 0.32);

                Canvas.SetLeft(AnswerD, buttonWidth);
                Canvas.SetTop(AnswerD, imageHeight * 0.32 + buttonHeight);
            }
            else if (perception < 1)
            {
                double textLeft = imageWidth < QuestionText.Width ? 0 : (imageWidth - QuestionText.Width) / 2;
                Canvas.SetLeft(QuestionText, textLeft);
                Canvas.SetTop(QuestionText, imageHeight * 0.3);

                Canvas.SetLeft(AnswerA, 0);
                Canvas.SetTop(AnswerA, imageHeight * 0.4);

                Canvas.SetLeft(AnswerC, 0);
                Canvas.SetTop(AnswerC, imageHeight * 0.4 + buttonHeight + 30);

                Canvas.SetLeft(AnswerB, buttonWidth);
                Canvas.SetTop(AnswerB, imageHeight * 0.4);

                Canvas.SetLeft(AnswerD, buttonWidth);
                Canvas.SetTop(AnswerD, imageHeight * 0.4 + buttonHeight + 30);
            }
            else if (perception < 1.4) //fill view screen
            {
                Canvas.SetLeft(QuestionText, (ImgBorder.Width - QuestionText.Width) / 2);
                Canvas.SetTop(QuestionText, imageHeight * 0.15);

                Canvas.SetLeft(AnswerA, imageWidth * 0.32);
                Canvas.SetTop(AnswerA, imageHeight * 0.42);

                Canvas.SetLeft(AnswerC, imageWidth * 0.32);
                Canvas.SetTop(AnswerC, imageHeight * 0.42 + buttonHeight);

                Canvas.SetLeft(AnswerB, imageWidth * 0.32 + buttonWidth);
                Canvas.SetTop(AnswerB, imageHeight * 0.42);

                Canvas.SetLeft(AnswerD, imageWidth * 0.32 + buttonWidth);
                Canvas.SetTop(AnswerD, imageHeight * 0.42 + buttonHeight);
            }
            else
            {
                Canvas.SetLeft(QuestionText, (ImgBorder.Width - QuestionText.Width) / 2);
                Canvas.SetTop(QuestionText, imageHeight * 0.2);

                Canvas.SetLeft(AnswerA, imageWidth * 0.32);
                Canvas.SetTop(AnswerA, imageHeight * 0.55);

                Canvas.SetLeft(AnswerC, imageWidth * 0.32);
                Canvas.SetTop(AnswerC, imageHeight * 0.55 + buttonHeight);

                Canvas.SetLeft(AnswerB, imageWidth * 0.32 + buttonWidth);
                Canvas.SetTop(AnswerB, imageHeight * 0.55);

                Canvas.SetLeft(AnswerD, imageWidth * 0.32 + buttonWidth);
                Canvas.SetTop(AnswerD, imageHeight * 0.55 + buttonHeight);
            }

        }

        private void AdjustHiddenControls(double imageWidth, double imageHeight)
        {
            double perception = imageWidth / imageHeight;
            if (perception < 0.6)
            {
                Canvas.SetLeft(levelUp, imageWidth * 0.65);
                Canvas.SetTop(levelUp, imageHeight * 0.6 - levelUp.Height);

                Canvas.SetLeft(Start, imageWidth * 0.22 - Start.Width / 2);
                Canvas.SetTop(Start, imageHeight * 0.2 - Start.Height / 2);

                Canvas.SetLeft(Caption1, imageWidth * 0.55);
                Canvas.SetTop(Caption1, imageHeight * 0.02);
                Canvas.SetLeft(Caption2, imageWidth * 0.7);
                Canvas.SetTop(Caption2, imageHeight * 0.15);
                Canvas.SetLeft(Caption3, imageWidth * 0.5);
                Canvas.SetTop(Caption3, imageHeight * 0.25);
                Canvas.SetLeft(Caption4, imageWidth * 0.62);
                Canvas.SetTop(Caption4, imageHeight * 0.42);
                Canvas.SetLeft(Caption5, imageWidth * 0.45);
                Canvas.SetTop(Caption5, imageHeight * 0.55);
                Canvas.SetLeft(Caption6, imageWidth * 0.72);
                Canvas.SetTop(Caption6, imageHeight * 0.62);
            }
            if (perception < 1) //fill view screen
            {
                Canvas.SetLeft(levelUp, imageWidth * 0.5);
                Canvas.SetTop(levelUp, imageHeight * 0.6 - levelUp.Height);

                Canvas.SetLeft(Start, imageWidth * 0.2 - Start.Width / 2);
                Canvas.SetTop(Start, imageHeight * 0.6 - Start.Height / 2);

                Canvas.SetLeft(Caption1, imageWidth * 0.55);
                Canvas.SetTop(Caption1, imageHeight * 0.02);
                Canvas.SetLeft(Caption2, imageWidth * 0.7);
                Canvas.SetTop(Caption2, imageHeight * 0.15);
                Canvas.SetLeft(Caption3, imageWidth * 0.5);
                Canvas.SetTop(Caption3, imageHeight * 0.25);
                Canvas.SetLeft(Caption4, imageWidth * 0.62);
                Canvas.SetTop(Caption4, imageHeight * 0.42);
                Canvas.SetLeft(Caption5, imageWidth * 0.45);
                Canvas.SetTop(Caption5, imageHeight * 0.55);
                Canvas.SetLeft(Caption6, imageWidth * 0.72);
                Canvas.SetTop(Caption6, imageHeight * 0.62);
            }
            if (perception < 1.4) //fill view screen
            {
                Canvas.SetLeft(levelUp, imageWidth * 0.75);
                Canvas.SetTop(levelUp, imageHeight * 0.8 - levelUp.Height);

                Canvas.SetLeft(Start, imageWidth * 0.2 - Start.Width / 2);
                Canvas.SetTop(Start, imageHeight * 0.6 - Start.Height / 2);

                Canvas.SetLeft(Caption1, imageWidth * 0.55);
                Canvas.SetTop(Caption1, imageHeight * 0.02);
                Canvas.SetLeft(Caption2, imageWidth * 0.7);
                Canvas.SetTop(Caption2, imageHeight * 0.15);
                Canvas.SetLeft(Caption3, imageWidth * 0.5);
                Canvas.SetTop(Caption3, imageHeight * 0.25);
                Canvas.SetLeft(Caption4, imageWidth * 0.62);
                Canvas.SetTop(Caption4, imageHeight * 0.42);
                Canvas.SetLeft(Caption5, imageWidth * 0.45);
                Canvas.SetTop(Caption5, imageHeight * 0.55);
                Canvas.SetLeft(Caption6, imageWidth * 0.72);
                Canvas.SetTop(Caption6, imageHeight * 0.62);
            }
            else if (perception < 1.6)
            {
                Canvas.SetLeft(levelUp, imageWidth * 0.8);
                Canvas.SetTop(levelUp, imageHeight * 0.9 - levelUp.Height);

                Canvas.SetLeft(Start, imageWidth * 0.2 - Start.Width / 2);
                Canvas.SetTop(Start, imageHeight * 0.72 - Start.Height / 2);

                Canvas.SetLeft(Caption1, imageWidth * 0.55);
                Canvas.SetTop(Caption1, imageHeight * 0.02);
                Canvas.SetLeft(Caption2, imageWidth * 0.7);
                Canvas.SetTop(Caption2, imageHeight * 0.2);
                Canvas.SetLeft(Caption3, imageWidth * 0.5);
                Canvas.SetTop(Caption3, imageHeight * 0.35);
                Canvas.SetLeft(Caption4, imageWidth * 0.62);
                Canvas.SetTop(Caption4, imageHeight * 0.52);
                Canvas.SetLeft(Caption5, imageWidth * 0.48);
                Canvas.SetTop(Caption5, imageHeight * 0.65);
                Canvas.SetLeft(Caption6, imageWidth * 0.72);
                Canvas.SetTop(Caption6, imageHeight * 0.72);
            }
            else
            {
                Canvas.SetLeft(levelUp, imageWidth * 0.8);
                Canvas.SetTop(levelUp, imageHeight * 0.9 - levelUp.Height);

                Canvas.SetLeft(Start, imageWidth * 0.2 - Start.Width / 2);
                Canvas.SetTop(Start, imageHeight * 0.78 - Start.Height / 2);

                Canvas.SetLeft(Caption1, imageWidth * 0.55);
                Canvas.SetTop(Caption1, imageHeight * 0.02);
                Canvas.SetLeft(Caption2, imageWidth * 0.7);
                Canvas.SetTop(Caption2, imageHeight * 0.2);
                Canvas.SetLeft(Caption3, imageWidth * 0.5);            

                Canvas.SetTop(Caption3, imageHeight * 0.35);
                Canvas.SetLeft(Caption4, imageWidth * 0.62);
                Canvas.SetTop(Caption4, imageHeight * 0.52);
                Canvas.SetLeft(Caption5, imageWidth * 0.48);
                Canvas.SetTop(Caption5, imageHeight * 0.65);
                Canvas.SetLeft(Caption6, imageWidth * 0.72);
                Canvas.SetTop(Caption6, imageHeight * 0.72);
            }
        }

        private void AdjustControlsForFullScreenLandscape()
        {
            Utilities.SetImage(backgroundImg, Resource1.night);
            
            //Adjust title bar
            score.Width = this.ActualWidth / 12;
            scoreTitle.Width = this.ActualWidth / 10;
            //title.Height = 50;

            //Adjust progress bar
            Progress.Width = this.ActualWidth - score.Width;
            //Progress.Height = 48;

            //Adjust image
            ImgBorder.Width = Progress.Width;
            //Next = ImgBorder.Width - scoreTitle.Width - Next.ActualWidth;

            double imageHeight = this.ActualHeight - Progress.Height; //TODO bottomscore.height
            double imageWidth = ImgBorder.Width;

            double buttonHeight = imageHeight * 0.12;
            double buttonWidth = ImgBorder.Width * 0.25;

            AdjustAnswerControls(imageHeight, imageWidth, buttonHeight, buttonWidth);

            AdjustHiddenControls(imageWidth, imageHeight);

        }

        private void MainPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustControlsForFullScreenLandscape();
            //switch (Windows.UI.ViewManagement.ApplicationView.Value)
            //{
            //    case Windows.UI.ViewManagement.ApplicationViewState.Filled:
            //        VisualStateManager.GoToState(this, "Fill", false);
            //        AdjustControlsForFullScreenLandscape();
            //        break;
            //    case Windows.UI.ViewManagement.ApplicationViewState.FullScreenLandscape:
            //        VisualStateManager.GoToState(this, "FullScreenLandscape", false);
            //        AdjustControlsForFullScreenLandscape();
            //        break;
            //    case Windows.UI.ViewManagement.ApplicationViewState.Snapped:
            //        VisualStateManager.GoToState(this, "Snapped", false);
            //        AdjustControlsForSnapped();
            //        break;
            //    case Windows.UI.ViewManagement.ApplicationViewState.FullScreenPortrait:
            //        VisualStateManager.GoToState(this, "FullScreenPortrait", false);
            //        AdjustControlsForFullScreenPortrait();
            //        break;
            //    default:
            //        break;
            //}
        }

        private void ReplayQuestionTo(int questionIndex)
        {
            m_currentLevel = questionIndex / Constants.QuestionPerLevel + 1;
            m_currentQuestionIndex = questionIndex % Constants.QuestionPerLevel + 1;
            ResetScoreImages();
            pageTitle2.Text = Constants.ChineseNumbers[m_currentLevel - 1];

            for (int j = 0; j < m_currentQuestionIndex - 1; j++)
            {
                int index = (m_currentLevel - 1) * Constants.QuestionPerLevel + j;
                if (Global.QuestionAndAnswers[index].Answer == Global.QuestionAndAnswers[index].Question.CorrectAnswer)
                {
                    Utilities.SetImage(m_scoreImages[j], Resource1._true);
                }
                else
                {
                    Utilities.SetImage(m_scoreImages[j], Resource1._false);
                }
            }
            m_currentQuestion = Global.QuestionAndAnswers[questionIndex].Question;
            QuestionText.Text = string.Format(QuestionPrefixFormat, m_currentQuestionIndex) +
                                m_currentQuestion.Text;
            AnswerA.Content = "A. " + m_currentQuestion.Answers[0];
            AnswerB.Content = "B. " + m_currentQuestion.Answers[1];
            AnswerC.Content = "C. " + m_currentQuestion.Answers[2];
            AnswerD.Content = "D. " + m_currentQuestion.Answers[3];

            score.Text = Global.QuestionAndAnswers[questionIndex].CurrentScore.ToString();
            SetAnswer(Global.QuestionAndAnswers[questionIndex].Answer);
            
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = (m_currentLevel - 1)*Constants.QuestionPerLevel + m_currentQuestionIndex;
                 i < Global.QuestionAndAnswers.Count;
                 i++)
            {
                if (Global.ReplayMode == ReplayMode.All || 
                    (Global.ReplayMode == ReplayMode.Errors && Global.QuestionAndAnswers[i].Answer != Global.QuestionAndAnswers[i].Question.CorrectAnswer))
                {
                    ReplayQuestionTo(i);
                    break;
                }
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = (m_currentLevel - 1) * Constants.QuestionPerLevel + m_currentQuestionIndex - 2;
                 i >= 0;
                 i--)
            {
                if (Global.ReplayMode == ReplayMode.All ||
                    (Global.ReplayMode == ReplayMode.Errors && Global.QuestionAndAnswers[i].Answer != Global.QuestionAndAnswers[i].Question.CorrectAnswer))
                {
                    ReplayQuestionTo(i);
                    break;
                }
            }

        }

    }
}