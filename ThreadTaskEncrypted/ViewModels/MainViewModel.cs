using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ThreadTaskEncrypted.Commands;
using ThreadTaskEncrypted.Models;

namespace ThreadTaskEncrypted.ViewModels
{
    public class MainViewModel:BaseViewModel
    {
        public RelayCommand PlayCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand PauseCommand { get; set; }
        public RelayCommand ResumeCommand { get; set; }
        public RelayCommand EnterCommand { get; set; }
        public Thread Thread { get; set; }

        private string wordTxtBox;

        public string WordTxtBox
        {
            get { return wordTxtBox; }
            set { wordTxtBox = value; }
        }

        private ObservableCollection<string> wordsListBox;

        public ObservableCollection<string> WordsListBox
        {
            get { return wordsListBox; }
            set { wordsListBox = value; }
        }

        private ObservableCollection<string> wordsCryptedListBox;
        public ObservableCollection<string> WordsCryptedListBox
        {
            get { return wordsCryptedListBox; }
            set { wordsCryptedListBox = value; OnPropertyChanged(); }

        }


        [Obsolete]
        public MainViewModel()
        {

            wordsListBox = new ObservableCollection<string>();
            wordsCryptedListBox = new ObservableCollection<string>();
            WordsListBox.Add("lemon");
            WordsListBox.Add("mango");
            WordsListBox.Add("data");
            WordsListBox.Add("salam");

            EnterCommand = new RelayCommand((obj) =>
            {
                Thread enterThread = new Thread(() =>
                {
                    if (WordTxtBox != string.Empty)
                    {
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            wordsListBox.Add(wordTxtBox);
                        });
                    }
                    else
                        MessageBox.Show("Enter word");

                    wordTxtBox = string.Empty;
                });

                enterThread.Start();

            });

            PlayCommand=new RelayCommand((obj) =>
            {
                Thread PlayedThread = new Thread(() =>
                {
                    Thread thread = new Thread(() =>
                    {
                        ObservableCollection<string> list = new ObservableCollection<string>(wordsListBox);
                        string deletedlist = string.Empty;
                        foreach (var item in list)
                        {
                            deletedlist = item;

                            Thread threadRemove = new Thread(() =>
                            {
                                App.Current.Dispatcher.Invoke((Action)delegate 
                                {
                                    wordsListBox.Remove(deletedlist);
                                });
                            });


                            Thread threadAdd = new Thread(() =>
                            {
                                threadRemove.Join();
                                App.Current.Dispatcher.Invoke((Action)delegate 
                                {
                                    WordsCryptedListBox.Add(Encrypted.getHashSha256(deletedlist));
                                });

                            });

                            threadRemove.Start();
                            Thread.Sleep(2500);
                            threadAdd.Start();
                        };
                    });
                    Thread.Start();
                    if (wordsListBox.Count == 0)
                    {
                        Thread?.Abort();
                    }
                });
                PlayedThread.Start();
            });

            StopCommand = new RelayCommand((obj) =>
            {
                Thread.Abort();
                MessageBox.Show("Stoped");
            });

            PauseCommand = new RelayCommand((obj) =>
            {
                Thread.Suspend();
                MessageBox.Show("Suspended");
            });

            ResumeCommand = new RelayCommand((obj) =>
            {
                Thread.Resume();
                MessageBox.Show("Resumed");
            });


        }


    }
}
