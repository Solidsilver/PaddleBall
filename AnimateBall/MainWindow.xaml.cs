using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace AnimateBall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer gameTimer = new DispatcherTimer();


        private double dx = 3;
        private double dy = 3;
        private double vertDirection = -1;
        private double horizDirection = 1;

        private double gameBallTop = 0;
        private double gameBallLeft = 0;

        private double gamePaddleTop = 0;
        private double gamePaddleLeft = 0;
        private double gamePaddleDy = 5;

        private bool mouseMode = false;
        private bool canScore = false;

        private int score = 0;
        private int hscore;

        List<string> ls = new List<string>();

        MediaPlayer mpPop;
        MediaPlayer mpBeep;
        MediaPlayer mpError;


        public MainWindow()
        {
            InitializeComponent();

            mpPop = new MediaPlayer();
            mpPop.Open(new Uri("../../sounds/plop.wav", UriKind.Relative));
            mpBeep = new MediaPlayer();
            mpBeep.Open(new Uri("../../sounds/beep.wav", UriKind.Relative));
            mpError = new MediaPlayer();
            mpError.Open(new Uri("../../sounds/err.wav", UriKind.Relative));
            //sound.LoadedBehavior = MediaState.Manual;
            //sound.Source = new Uri("sounds/ping_pong_8bit_plop.wav", UriKind.Relative);
            try
            {
                //StreamReader sr = new StreamReader(@"c:\users\luke\hs.txt");
                StreamReader sr = new StreamReader(Directory.GetCurrentDirectory().ToString() + "\\hs.txt");

                int.TryParse(sr.ReadLine(), out hscore);
                sr.Close();
            } catch (Exception e)
            {
                this.hscore = 0;
            }
            
            ls.Add("Welcome to the game!");
            ls.Add("Press Space to start");
            ls.Add("Press 'm' for mouse mode");
            lbxMessages.ItemsSource = ls;

            
            
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            gameTimer.Tick += GameTimer_Tick;

            gameBallTop = Canvas.GetTop(gameBall);
            gameBallLeft = Canvas.GetLeft(gameBall);

            gamePaddleTop = Canvas.GetTop(gamePaddle);
            gamePaddleLeft = Canvas.GetLeft(gamePaddle);

            btnAbout.Focusable = false;
            btnPause.Focusable = false;
            btnPlay.Focusable = false;
            btnClose.Focusable = false;
            lbxMessages.Focusable = false;
            gameTimer.IsEnabled = true;
            pause();
            
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            this.dx += 0.002;
            this.dy += 0.002;

            gamePaddleDy += 0.002;

            if (mouseMode)
            {
                    gamePaddleTop = Mouse.GetPosition(myGameCanvas).Y - gamePaddle.ActualHeight/2;
            }
            if (gameBallTop <= 0  || gameBallTop >= (myGameCanvas.ActualHeight - gameBall.Height))
            {
                vertDirection *= -1;
                playPop();
            }
            if (gameBallLeft >= myGameCanvas.ActualWidth - gameBall.Width)
            {
                horizDirection *= -1;
                playPop();
                this.canScore = true;
            }
            if (gameBallLeft <= 0)
            {
                playErr();
                pause();
                if (score > hscore)
                    updateHS();
                score = 0;
                dx = 3;
                dy = 3;
                lblSCore.Content = score; ;
                gameBallTop = myGameCanvas.ActualHeight/2 - gameBall.ActualHeight/2;
                gameBallLeft = myGameCanvas.ActualWidth/2 - gameBall.ActualWidth/2;
                sendMessage("You Missed!");
                
                
            }
            if (gamePaddleLeft + gamePaddle.Width >= gameBallLeft
                && gameBallTop + gameBall.Height/2 >= gamePaddleTop && gameBallTop + gameBall.Height / 2 <= gamePaddleTop + gamePaddle.Height)
            {
                horizDirection = 1;
                playBeep();
                if (canScore)
                {
                    score++;
                    if (score > hscore)
                        sendMessage("New HighScore: " + score);
                    canScore = false;
                }
                lblSCore.Content = score;
            }
            

            gameBallLeft += dx * horizDirection;
            gameBallTop += dy * vertDirection;

            Canvas.SetTop(gameBall, gameBallTop);
            Canvas.SetLeft(gameBall, gameBallLeft);
            Canvas.SetTop(gamePaddle, gamePaddleTop);

        }

        private void playErr()
        {
            mpError.Position = TimeSpan.Zero;
            mpError.Play();
        }
        private void playBeep()
        {
            mpBeep.Position = TimeSpan.Zero;
            mpBeep.Play();
        }
        private void playPop()
        {
            mpPop.Position = TimeSpan.Zero;
            mpPop.Play();
        }

        private void showMen()
        {
            /*for (double x = 1; x >= 0; x -= .00001)
            {
                rctMen.Opacity = x;
            }*/
            rctMen.Visibility = Visibility.Hidden;
            rctMen.IsEnabled = false;
        }

        private void hideMen()
        {
            /*for (double x = 0; x <=1 ; x += .00001)
            {
                rctMen.Opacity = x;
            }*/
            rctMen.Visibility = Visibility.Visible;
            rctMen.IsEnabled = true;
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Up)
            {
                mouseMode = false;
                //sendMessage("Mousemode off");
                if (gamePaddleTop - gamePaddleDy >= 0)
                    gamePaddleTop -= gamePaddleDy;
            }
            else if (e.Key == Key.Down)
            {
                mouseMode = false;
                //sendMessage("Mousemode off");
                if (gamePaddleTop + gamePaddleDy + gamePaddle.Height <= myGameCanvas.ActualHeight)
                    gamePaddleTop += gamePaddleDy;
            }
            Canvas.SetTop(gamePaddle, gamePaddleTop);
            if (e.Key == Key.M)
            {
                if (!e.IsRepeat)
                {
                    mouseMode = !this.mouseMode;
                    if (mouseMode)
                    {
                        sendMessage("Mousemode on");
                    } else
                    {
                        sendMessage("Mousemode off");
                    }
                }
            }
            if (e.Key == Key.Space)
            {
                if (!e.IsRepeat)
                {
                    pause();
                }
            }
        }

        private void pause()
        {
            gameTimer.IsEnabled = !gameTimer.IsEnabled;
            if (gameTimer.IsEnabled)
            {
                showMen();
                btnPause.Visibility = Visibility.Visible;
                sendMessage("Game Resumed");
            }
            else
            {
                hideMen();
                sendMessage("Game Paused");
                btnPause.Visibility = Visibility.Hidden;
            }
        }

        private void updateHS()
        {
            if (score > hscore) {
                hscore = score;
                System.IO.File.WriteAllText(Directory.GetCurrentDirectory().ToString() + "\\hs.txt", hscore + "");
            }
                
        }

        private void sendMessage(string msg)
        {

            //ls.Add(msg);
            ls.Insert(0, msg);
            if (ls.Count > 4)
            {
                ls.RemoveAt(4);
            }
            lbxMessages.Items.Refresh();
        }
             
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            updateHS();
            e.Cancel = !exitingMessage();
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            string msg = "Paddle Ball\n" +
                         "Created by Luke Mattfeld\n" +
                         "\n" +
                         "Controls are in the top rigth corner.\n" +
                         "Press space to pause/unpause the game.\n" +
                         "Press 'm' to toggle mouse mode.\n" +
                         "Notifications are in the bottom rigth.\n" +
                         "Enjoy!\n" +
                         "\n" +
                         "High Score: " + hscore;
            MessageBox.Show(msg, "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            pause();
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

            if (exitingMessage())
            {
                Environment.Exit(0);
            }
        }

        private bool exitingMessage()
        {
            //pause();
            string alertName = "Exiting";
            string message = "YOU ARE ABOUT TO LEAVE!\nit's ok if you do.\nI'll just sob in a corner\n\nAre you sure you want to go?";
            if (MessageBox.Show(message, alertName, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                return true;
            }
            else
            {
                sendMessage("I Feel Alive!! :)");
                return false;
            }
        }
    }
}
