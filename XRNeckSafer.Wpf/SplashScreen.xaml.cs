using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace XRNeckSafer.Wpf
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Storyboard storyBoard = FindResource("StartAnimation") as Storyboard;
            storyBoard.Completed += OnAnimationCompleted;
            storyBoard.Begin();
        }

        private void OnAnimationCompleted(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new EventHandler(OnAnimationCompleted), sender, e);
                return;
            }
            Close();
        }
    }
}
