using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinnTorget
{
    /// <summary>
    /// Interaction logic for FancyBalloon.xaml
    /// </summary>
    public partial class FancyBalloon : UserControl
    {
        public FancyBalloon()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CloseButtonPressed != null)
                CloseButtonPressed();
        }

        private void TbStuffName_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mainWin = Application.Current.MainWindow as MainWindow;
            if (mainWin != null)
                mainWin.ShowAndActivate();
        }

        public event CloseButtonEventHandler CloseButtonPressed;

        #region BalloonText dependency property

        /// <summary>
        /// Description
        /// </summary>
        public static readonly DependencyProperty BalloonTextProperty =
            DependencyProperty.Register("BalloonText",
                                        typeof(string),
                                        typeof(FancyBalloon),
                                        new FrameworkPropertyMetadata(""));

        /// <summary>
        /// A property wrapper for the <see cref="BalloonTextProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public string BalloonText
        {
            get { return (string)GetValue(BalloonTextProperty); }
            set { SetValue(BalloonTextProperty, value); }
        }

        #endregion

    }
}
