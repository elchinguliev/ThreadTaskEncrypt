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
        //new
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
        bool ThreadPlaying=false;
        Thread encryptWordsThread = null;

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

            PlayCommand = new RelayCommand((p) =>
            {
                if (!ThreadPlaying)
                {
                    encryptWordsThread = new Thread(() =>
                    {
                        if (wordsListBox.Count == 0)
                        {
                            MessageBox.Show("Add words!");
                            return;
                        }

                        while (wordsListBox.Count != 0)
                        {
                            Thread.Sleep(700);
                            if (wordsListBox != null && wordsListBox.Count > 0)
                            {
                                List<string> enc_words2 = wordsCryptedListBox.ToList();
                                List<string> words2 = wordsListBox.ToList();

                                string word = wordsListBox.Last();
                                var enc_word = word.Encrypt("random");

                                enc_words2.Add(enc_word);
                                words2.Remove(word);

                                wordsListBox = new ObservableCollection<string>(words2);
                                wordsCryptedListBox = new ObservableCollection<string>(enc_words2);
                            }
                        }
                        MessageBox.Show("Words finished! Thread aborted!");
                        ThreadPlaying = false;
                    });
                    encryptWordsThread.IsBackground = true;
                    ThreadPlaying = true;
                    encryptWordsThread.Start();
                }
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
