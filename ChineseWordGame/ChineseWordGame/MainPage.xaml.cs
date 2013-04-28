using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChineseWordGame.Questions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;
using ChineseWordGame.Common;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ChineseWordGame
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : ChineseWordGame.Common.LayoutAwarePage
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

        private const string QuestionPrefixFormat = "第{0}题： ";
        private const string TitleFormat = "关： 至少答对{1}题";
        private const string LastLevelTitle = "关： 最后一关，加油";
        private const string LevelUpTitle = "关： 恭喜过关，请进入下一关";
        private const string TrueAnswerImage = "ms-appx:///assets/true.jpg";
        private const string FalseAnswerImage = "ms-appx:///assets/false.png";
        private const string LandscapeImage = "ms-appx:///assets/night.jpg";
        private const string PortraitImage = "ms-appx:///assets/night_vertical.jpg";
        private const string SnapImage = "ms-appx:///assets/snap.jpg";

        private const int TotalTime = 100;

        private Questions.CombinedQuestion m_currentQuestion;
        
        private Button[] m_answerButtons;

        private int m_currentQuestionIndex;

        //private Image[] m_timerImages = null;
        private int m_remainingTime;
        private DispatcherTimer m_timer = new DispatcherTimer();

        private Image[] m_scoreImages = null;

        private int m_currentLevel = 0;
        private int m_correctAnswerCount = 0;
        private int[] m_timerIntervalInLevels;

        private int m_currentScore = 0;

        private bool m_isCheckingAnswer;

        public MainPage()
        {
            this.InitializeComponent();
            HideControls();
            InitLevels();
            InitTimerImages();
            InitScoreImages();
            InitAnswerButtions();
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
            pageTitle2.Text = Constants.ChineseNumbers[m_currentLevel-1];
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
            if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                //In snapped view, it is paused
                return;
            }
            m_remainingTime--;
            Progress.Value++;
            //m_timerImages[m_timerImages.Count() - 1 - m_remainingTime].Visibility = Visibility.Collapsed; 
            if(m_remainingTime == 0)
            {
                m_timer.Stop();
                CheckAnswer(-1);
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

            Progress.Maximum = 100;

        }

        private void InitScoreImages()
        {
            m_scoreImages = new Image[10];
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
            if (pageState != null && pageState.ContainsKey("isTimerRunning") && Convert.ToInt32(pageState["isTimerRunning"]) == 1)
            {
                m_timer.Start();
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            if(m_timer.IsEnabled)
            {
                m_timer.Stop();
                pageState["isTimerRunning"] = 1;
            }
            else
            {
                pageState["isTimerRunning"] = 0;
            }
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


        private async void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
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

        private async void CheckAnswer(int result)
        {
            if (m_isCheckingAnswer == true)
                return;

            m_timer.Stop();
            m_isCheckingAnswer = true;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.DecodePixelHeight = 100;
            bitmapImage.DecodePixelWidth = 100;
            string imageUri = null;
            int delayms;

            if (result == m_currentQuestion.CorrectAnswer)
            {
                delayms = 500;
                imageUri = TrueAnswerImage;
                m_correctAnswerCount++;
                m_currentScore += Constants.BaseScoreOfQuestion + Constants.BaseScoreOfQuestion * (m_currentLevel - 1)*5/10;
                score.Text = m_currentScore.ToString();
            }
            else
            {
                delayms = 2000;
                imageUri = FalseAnswerImage;
            }
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(imageUri));
            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                // Set the image source to the selected bitmap

                await bitmapImage.SetSourceAsync(fileStream);
                m_scoreImages[m_currentQuestionIndex - 1].Source = bitmapImage;
                m_scoreImages[m_currentQuestionIndex - 1].Visibility = Visibility.Visible;
            }
            m_answerButtons[m_currentQuestion.CorrectAnswer].IsEnabled = false;
            await Task.Delay(delayms);
            m_answerButtons[m_currentQuestion.CorrectAnswer].IsEnabled = true;

            if (m_currentQuestionIndex < 10)
            {
                ShowNextQuestion();
            }
            else
            {
                if(m_currentLevel < Constants.TotalLevelCount && m_correctAnswerCount >= Constants.BaseBarForEachLevel + m_currentLevel - 1)
                {
                    levelUp.Visibility = Visibility.Visible;
                    pageTitle3.Text = LevelUpTitle;
                    return;
                }
                else
                {
                    SaveCurrentScore();
                    Frame rootFrame = Window.Current.Content as Frame;
                    if (!rootFrame.Navigate(typeof(EndPage), null))
                    {
                        throw new Exception("Failed to create end page");
                    }
                }
            }
            m_isCheckingAnswer = false;
        }

        private int[] LoadTopScores()
        {
            int[] topScores = new int[5];
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;

            for(int i = 0; i < TopScoreKeys.Count(); i++)
            {
                if (roamingSettings.Values.ContainsKey(TopScoreKeys[i]))
                {
                    topScores[i] = Convert.ToInt32(roamingSettings.Values[TopScoreKeys[i]]);
                }
                else
                {
                    roamingSettings.Values[TopScoreKeys[i]] = 0;
                    topScores[i] = 0;
                }
            }
            return topScores;
        }

        private void InsertTopScore(int position, int score)
        {
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;

            for(int i = TopScoreKeys.Count() - 1; i > position; i--)
            {
                roamingSettings.Values[TopScoreKeys[i]] = roamingSettings.Values[TopScoreKeys[i-1]];
            }
            roamingSettings.Values[TopScoreKeys[position]] = score;
        }

        private void SaveCurrentScore()
        {
            ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["currentScore"] = m_currentScore;
            roamingSettings.Values["currentLevel"] = m_currentLevel;
            roamingSettings.Values.Remove("currentRank");

            var topScores = LoadTopScores();
            for(int i = 0; i < topScores.Count(); i++)
            {
                if(m_currentScore > topScores[i])
                {
                    InsertTopScore(i, m_currentScore);
                    roamingSettings.Values["currentRank"] = i + 1;
                    break;
                }
            }
        }

        private void levelUp_Click(object sender, RoutedEventArgs e)
        {
            levelUp.Visibility = Visibility.Collapsed;
            GotoNextLevel();
            m_isCheckingAnswer = false;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            ShowControls();
            GotoNextLevel();
        }

        private void AdjustControlsForFullScreenLandscape()
        {
            SetBackgroundImage(backgroundImg, LandscapeImage);

            //Adjust margin
            var top = this.ActualHeight / 20;
            var bottom = this.ActualHeight / 20;
            var left = this.ActualWidth / 20;
            var right = this.ActualWidth / 20;
            var margin = new Thickness(left, top, right, bottom);
            root.Margin = margin;

            //Adjust title bar
            score.Width = this.ActualWidth / 12;
            scoreTitle.Width = this.ActualWidth / 10;
            title.Height = 70;

            //Adjust progress bar
            Progress.Width = this.ActualWidth - left - right - score.Width;
            Progress.Height = 48;

            //Adjust image
            ImgBorder.Width = Progress.Width;

            double imageHeight = this.ActualHeight - top - bottom - Progress.Height; //TODO bottomscore.height
            double imageWidth = ImgBorder.Width;

            double buttonHeight = imageHeight * 0.12;
            double buttonWidth = ImgBorder.Width * 0.25;

            AdjustAnswerControls(imageHeight, imageWidth, buttonHeight, buttonWidth);

            AdjustHiddenControls(imageWidth, imageHeight);

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

        private async void AdjustControlsForFullScreenPortrait()
        {
            SetBackgroundImage(backgroundImg, PortraitImage);
            //Adjust margin
            var top = this.ActualHeight / 20;
            var bottom = this.ActualHeight / 20;
            var left = this.ActualWidth / 20;
            var right = this.ActualWidth / 20;
            var margin = new Thickness(left, top, right, bottom);
            root.Margin = margin;

            //Adjust title bar
            title.Height = 70;
            score.Width = this.ActualWidth / 12 >= 90 ? this.ActualWidth / 12 : 90;
            scoreTitle.Width = this.ActualWidth / 10;

            //Adjust progress bar
            Progress.Width = this.ActualWidth - left - right - score.Width;
            Progress.Height = 48;

            //Adjust image
            ImgBorder.Width = Progress.Width;

            double imageHeight = this.ActualHeight - top - bottom - Progress.Height; 
            double imageWidth = ImgBorder.Width;

            double buttonHeight = imageHeight * 0.05;
            double buttonWidth = ImgBorder.Width * 0.5;

            AdjustAnswerControls(imageHeight, imageWidth, buttonHeight, buttonWidth);

            AdjustHiddenControls(imageWidth, imageHeight);

        }

        private async void SetBackgroundImage(Image img, string imageFile)
        {
            BitmapImage bitmapImage = new BitmapImage();

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(imageFile));
            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                // Set the image source to the selected bitmap
                await bitmapImage.SetSourceAsync(fileStream);
                img.Source = bitmapImage;
            }
        }

        private async void AdjustControlsForSnapped()
        {
            SetBackgroundImage(backgroundImg, SnapImage);
            ImgBorder.Width = 320;
            Progress.Width = 320;

            var margin = new Thickness(0, 0, 0, 0);
            root.Margin = margin;

            //Adjust title bar
            title.Height = 0;
            //pageTitle1.Height = 0;
            //pageTitle2.Height = 0;
            //pageTitle3.Height = 0;
            //scoreTitle.Height = 0;
            //score.Height = 0;
            //Adjust progress bar
            Progress.Height = 0;
        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            switch (Windows.UI.ViewManagement.ApplicationView.Value)
            {
                case Windows.UI.ViewManagement.ApplicationViewState.Filled:
                    VisualStateManager.GoToState(this, "Fill", false);
                    AdjustControlsForFullScreenLandscape();
                    break;
                case Windows.UI.ViewManagement.ApplicationViewState.FullScreenLandscape:
                    VisualStateManager.GoToState(this, "FullScreenLandscape", false);
                    AdjustControlsForFullScreenLandscape();
                    break;
                case Windows.UI.ViewManagement.ApplicationViewState.Snapped:
                    VisualStateManager.GoToState(this, "Snapped", false);
                    AdjustControlsForSnapped();
                    break;
                case Windows.UI.ViewManagement.ApplicationViewState.FullScreenPortrait:
                    VisualStateManager.GoToState(this, "FullScreenPortrait", false);
                    AdjustControlsForFullScreenPortrait();
                    break;
                default:
                    break;
            }
        }
    }
}
