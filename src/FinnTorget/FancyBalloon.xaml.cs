using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyNotifyIcon;

namespace FinnTorget
{
    /// <summary>
    /// Interaction logic for FancyBalloon.xaml
    /// </summary>
    public partial class FancyBalloon : UserControl, IBalloon
    {
        public FancyBalloon()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Closing != null)
                Closing();
        }

        private void TbStuffName_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mainWin = Application.Current.MainWindow as MainWindow;
            if (mainWin != null)
                mainWin.ShowAndActivate();

            
        }

        public event ClosingBalloonEventHandler Closing;

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

    public interface IBalloon
    {
        event ClosingBalloonEventHandler Closing;
    }
    
}
