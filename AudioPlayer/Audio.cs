using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Label = System.Windows.Controls.Label;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;

namespace AudioPlayer
{
    class Audio
    {
        Mp3FileReader mp3Reader;
        WaveFileReader waveReader;
        IWavePlayer waveOutDevice = new WaveOut();
        static MainWindow mainWindow;
        public static string[] files;
        static string path = System.Windows.Forms.Application.StartupPath + @"\Files\";
        static string fileFolderPath;
        bool firstTime = true;
        public Audio(MainWindow window)
        {
            mainWindow = window; 
            fileFolderPath = path.Replace(@"\bin\Debug", "");
            
        }
        public void GetFiles()
        {
            int fileCount = Directory.GetFiles(fileFolderPath, "*.*", SearchOption.AllDirectories).Length; // Will Retrieve count of all files in directry and sub directries
            files = Directory.GetFiles(fileFolderPath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < fileCount; i++)
            {
                CreateUI(files[i], null);
            }
            
        }
        public void CreateUI(string fileName, string safename)
        {
            Button file = new Button();
            string name1 = fileName.Replace(path.Replace(@"\bin\Debug", ""), "");
            string name2 = name1.Replace("mp3", "").Replace("wav", "");
            string name3 = name2.Replace(" ", "");
            Label l = new Label();
            if (safename != null)
            {
                file.Name = safename;
            }
            file.Name = CleanString(name3);

            l.Content = CleanString(name2);

            file.Content = l;
            file.Click += delegate (object sender, RoutedEventArgs e)
            {
                Play(fileName);
            };
            mainWindow.fileList.Children.Add(file);

            //Context Menu Setup
            ContextMenu cmenu = new ContextMenu();
            MenuItem deleteButton = new MenuItem();
            deleteButton.Header = "Delete";
            MenuItem infobutton = new MenuItem();
            infobutton.Header = "Info";

            infobutton.Click += delegate (object sender, RoutedEventArgs e)
            {
                double size = Math.Round((new System.IO.FileInfo(fileName).Length / 1000000f), 2);
                if (fileName.Contains(".mp3"))
                {
                    Mp3FileReader temp = new Mp3FileReader(fileName);

                    double sec;
                    double min = 0;
                    sec = temp.TotalTime.TotalSeconds;
                    if (sec / 60 > 1)
                    {
                        min = Math.Round(sec / 60, 0);

                        sec -= min * 60;
                        if (sec < 0)
                        {
                            sec = 60 + sec;
                            min -= 1;
                        }
                    }
                    string length;
                    if (sec > 10)
                    {
                        length = min + ":" + Math.Round(sec, 0);
                    }
                    else
                    {
                        length = min + ":" + "0" + Math.Round(sec, 0);
                    }

                    temp.Dispose();
                    MessageBox.Show("Name: " + CleanString(name2) + System.Environment.NewLine + "Location: " + fileName + System.Environment.NewLine + "Size: " + size.ToString() + " MB" + System.Environment.NewLine + "Length: " + length + Environment.NewLine + "Format: " + ".MP3");
                }
                if (fileName.Contains(".wav"))
                {
                    WaveFileReader temp = new WaveFileReader(fileName);

                    double sec;
                    double min = 0;
                    sec = temp.TotalTime.TotalSeconds;
                    if (sec / 60 > 1)
                    {
                        min = Math.Round(sec / 60, 0);

                        sec -= min * 60;
                        if (sec < 0)
                        {
                            sec = 60 + sec;
                            min -= 1;
                        }
                    }
                    string length;
                    if (sec > 10)
                    {
                        length = min + ":" + Math.Round(sec, 0);
                    }
                    else
                    {
                        length = min + ":" + "0" + Math.Round(sec, 0);
                    }

                    temp.Dispose();
                    MessageBox.Show("Name: " + CleanString(name2) + System.Environment.NewLine + "Location: " + fileName + System.Environment.NewLine + "Size: " + size.ToString() + " MB" + System.Environment.NewLine + "Length: " + length + Environment.NewLine + "Format: " + ".WAV");
                }

            };
            deleteButton.Click += delegate (object sender, RoutedEventArgs e)
            {
                
                if (System.Windows.Forms.MessageBox.Show("Are you sure you want to delete this file?", "Confirm Deletion", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    File.Delete(fileName);
                    MessageBox.Show("Deleted File: " + CleanString(name2));
                    ReloadFiles();
                }

            };

            cmenu.Items.Add(infobutton);
            cmenu.Items.Add(deleteButton);
            file.ContextMenu = cmenu;
            //file.ContextMenu.Items.Add(infobutton);
        }
        public static string filepath;
        public bool ismp3 = false;
        public void VolumeChange(double volume)
        {
            waveOutDevice.Volume = (float)volume;
            
        }
        
        public void PauseResume()
        {

            if (waveOutDevice.PlaybackState != PlaybackState.Stopped)
            {
                if (mainWindow.isPaused)
                {
                    timer1.Start();
                    waveOutDevice.Play();
                    mainWindow.isPaused = false;
                    mainWindow.pauseResumeButton.Content = "Pause";
                }
                else
                {
                    timer1.Stop();
                    waveOutDevice.Pause();
                    mainWindow.isPaused = true;
                    mainWindow.pauseResumeButton.Content = "Play";
                }
            }


            
        }
        public void Play(string name)
        {
            waveOutDevice.Volume = (float)mainWindow.VolumeSlider.Value;
            TimeSpan duration = new TimeSpan();
            if (!firstTime)
            {
                Stop();
                mainWindow.fileProgress.Value = 0;
            }
            

            if (name.Contains(".mp3"))
            {
                mp3Reader = new Mp3FileReader(name);
                waveOutDevice.Init(mp3Reader);
                
                waveOutDevice.Play();
                firstTime = false;
                duration = mp3Reader.TotalTime;
                ismp3 = true;
            }
            if (name.Contains(".wav"))
            {
                waveReader = new WaveFileReader(name);
                waveOutDevice.Init(waveReader);
                waveOutDevice.Play();
                firstTime = false;
                duration = waveReader.TotalTime;
                ismp3 = false;
            }
            InitTimer();
            filepath = name;


            mainWindow.fileProgress.Maximum = Math.Round(duration.TotalSeconds, 0);
            double sec;
            double min = 0;
            sec = duration.TotalSeconds;
            if (sec / 60 > 1)
            {
                min = Math.Round(sec / 60, 0);

                sec -= min * 60;
                if (sec < 0)
                {
                    sec = 60 + sec;
                    min -= 1;
                }
            }
            string text = min + ":" + Math.Round(sec, 0);
            mainWindow.maxLabel.Content = text;
        }
        private Timer timer1;

        public void InitTimer()
        {
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000; // in miliseconds
            timer1.Start();

        }
        Timer mintimer; double minutes;
        private void mintimer_Tick(Object sender, EventArgs e)
        {
            minutes += 1;
            mintimer.Stop();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            
            double seconds =  0;
            if (ismp3)
            {
                seconds = mp3Reader.CurrentTime.Seconds;
            }
            else
            {
                seconds = waveReader.CurrentTime.Seconds;
            }

            if (seconds == 59)
            {


                mintimer = new Timer();
                mintimer.Tick += new EventHandler(mintimer_Tick);
                mintimer.Interval = 100; // in miliseconds
                mintimer.Start();

            }

            

            string timetext;
            if (seconds < 10)
            {
                timetext = minutes.ToString() + ":" + "0" + seconds.ToString();
            }
            else
            {
                timetext = minutes.ToString() + ":" + seconds.ToString();
            }
            
            mainWindow.fileTime.Content = timetext;
            mainWindow.fileProgress.Value = (minutes * 60) + seconds;


        }
        public void Stop()
        {
            mainWindow.fileProgress.Value = 0;
            mainWindow.fileTime.Content = 0;
            minutes = 0;
            timer1.Stop();
            waveOutDevice.Stop();
            if (ismp3)
            {
                mp3Reader.Dispose();
            }
            else
            {
                waveReader.Dispose();
            }

        }

        void ReloadFiles()
        {
            files = null;
            mainWindow.fileList.Children.Clear();

            GetFiles();
        }
        public void AddFile()
        {
            string filepath = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Media to Import";
            openFileDialog.Filter = "Audio Files (*.mp3; *.wav)|*.MP3; *.WAV | MP3 File (.mp3)|*.mp3 Wave | Windows Audio File (.wav)|*.wav";
            openFileDialog.FileName = "Select Media";
            if (openFileDialog.ShowDialog().ToString().Equals("OK"))
            {
                filepath = openFileDialog.SafeFileName;
                string filename = openFileDialog.FileName;
                File.Copy(openFileDialog.FileName, path.Replace(@"\bin\Debug", "") + openFileDialog.SafeFileName, true);
                ReloadFiles();
            }


        }
        public string CleanString(string data)
        {
            return data.Replace(".", "").Replace("(", "").Replace(")", "").Replace("Music", "").Replace("-", "").Replace("Official", "").Replace("Video", "").Replace("_", "");
            
        }
    }
    
}
