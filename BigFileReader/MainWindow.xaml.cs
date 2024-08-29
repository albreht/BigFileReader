using BigFileReader.Model;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BigFileReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private AsyncRelayCommand _commandSearch;
        private int _currentLine;
        private readonly FileReader _fileReader;

        private Visibility _progressBarVisibility;

        private string _text2Search;

        private long _chunkSize { get; set; }

        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var selected = e.AddedItems[0] as SearchResult;

            var lineNumberIndex = LineBreaks.IndexOf(LineBreaks.First(p => p.LineNumber == selected.LineNumber));

            CurrentLine = lineNumberIndex;

        }

        private async Task LoadPreviewAsync(LineBreak lineBreak)
        {
            Text = await _fileReader.FileContentsAsync(lineBreak, 100);
            RaisePropertyChanged(nameof(Text));
            textViewer.ScrollToHome();
        }

        private void RaisePropertyChanged([CallerMemberName] string method = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(method));
        }

        private void textViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                CurrentLine++;
            }
            else if (CurrentLine > 0)
            {
                CurrentLine--;
            }

        }

        public MainWindow(string filePath)
        {

            InitializeComponent();

            DataContext = this;

            Title = $"big file search {filePath}";

            ProgressBarVisibility = Visibility.Collapsed;

            LineBreaks = new List<LineBreak>();
            _chunkSize = 40480;
            _fileReader = new FileReader(filePath);
            _fileReader.SetChunkSize(_chunkSize);


            LineBreaksCount = _fileReader.FileLength / _chunkSize;
            

            LineBreaks.Add(new LineBreak() { IsEstimated = false, LineNumber = 0 });
            for (int i = 1; i < LineBreaksCount; i++)
            {
                LineBreaks.Add(new LineBreak() { IsEstimated = true, LineNumber = i * _chunkSize });
            }

           _= LoadPreviewAsync(LineBreaks[0]);

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public AsyncRelayCommand CommandSearch => _commandSearch ??= new AsyncRelayCommand(async () =>
        {
            ProgressBarVisibility = Visibility.Visible;

            var results = await _fileReader.SearchWord(Text2Search);

            foreach (var lineBreak in results
            .Select(p => p.LineNumber)
                .Except(LineBreaks.Select(p => p.LineNumber)))
            {
                LineBreaks.Add(new LineBreak() { IsEstimated = false, LineNumber = lineBreak });
            }

            LineBreaks = LineBreaks.OrderBy(p => p.LineNumber).ToList();
            LineBreaksCount = LineBreaks.Count;
            RaisePropertyChanged(nameof(LineBreaksCount));

            SearchResults = results;
            RaisePropertyChanged(nameof(SearchResults));


            ProgressBarVisibility = Visibility.Collapsed;
        }, () => Text2Search != null && Text2Search.Length > 2);

        public int CurrentLine
        {
            get => _currentLine;
            set
            {
                _currentLine = value;
                RaisePropertyChanged();
                _=LoadPreviewAsync(LineBreaks[_currentLine]);
            }

        }

        public List<LineBreak> LineBreaks { get; set; }
        public long LineBreaksCount { get; set; }

        public Visibility ProgressBarVisibility
        {
            get => _progressBarVisibility; set
            {
                _progressBarVisibility = value;

                RaisePropertyChanged();
            }
        }
        public List<SearchResult> SearchResults { get; set; }

        public string Text { get; set; }

        public string Text2Search
        {
            get => _text2Search; set
            {
                _text2Search = value;

                CommandSearch.NotifyCanExecuteChanged();

            }
        }
    }
}