using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio;
using NAudio.Wave;
using Button = System.Windows.Controls.Button;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;

namespace AudioPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Audio a;
        public MainWindow()
        {
            a = new Audio(this);
            InitializeComponent();
            
            a.GetFiles();


        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            a.Stop();
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {

            a.AddFile(); 
            
        }


        private void VolumeSlider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            a.VolumeChange(VolumeSlider.Value);
            
            if (volumeLabel != null)
            {
                string vlabel = Math.Round(VolumeSlider.Value * 100, 0).ToString() ;
                vlabel += "%";
                volumeLabel.Content = vlabel;
            }

        }
        public bool isPaused = false;
        private void PauseResumeButton_Click(object sender, RoutedEventArgs e)
        {
            a.PauseResume();
            
        }

    }

}
