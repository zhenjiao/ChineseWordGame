using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChineseWordGame.Questions;

namespace ChineseWordGame.Common
{
    public enum PlayMode
    {
        Normal,
        Replay,
    }

    public enum ReplayMode
    {
        All,
        Errors,
    }

    public class Global
    {
        public static PlayMode PlayMode { get; set; }

        public static ReplayMode ReplayMode { get; set; }

        public static List<QuestionHistory> QuestionAndAnswers { get; set; }

    }


    public class Utilities
    {
        public static void SetImage(Image img, byte[] imageData)
        {
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                BitmapImage bi = new BitmapImage();
                bi.SetSource(ms);
                img.Source = bi;
                img.Visibility = Visibility.Visible;
            }
        }

        public static void SetImage(ImageBrush imgBrush, byte[] imageData)
        {
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                BitmapImage bi = new BitmapImage();
                bi.SetSource(ms);
                imgBrush.ImageSource = bi;
            }
        }

        
    }
}
