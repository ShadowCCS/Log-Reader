using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LogReader
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartMonitoring(); // Start monitoring without `await`
        }

        string filePath = "";

        private async void StartMonitoring()
        {
            filePath = LogPathBox.Text;

            if (File.Exists(filePath))
            {
                await UpdateOutputAsync();
            }
            else
            {
                await Task.Delay(5000); // Wait for 5 seconds asynchronously
                StartMonitoring(); // Retry without awaiting
            }
        }

        private async Task UpdateOutputAsync()
        {
            while (true) // Continuously update
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        // Read all text from the file asynchronously
                        string fileContent = await File.ReadAllTextAsync(filePath);

                        // Display the content in the TextBox
                        LogOutput.Text = fileContent;
                    }
                    else
                    {
                        // File no longer exists, break the loop
                        LogOutput.Text = "File not found.";
                        return;
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"An error occurred while reading the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Exit the method if there's an IO error
                }

                await Task.Delay(2000); // Wait asynchronously for 2 seconds
            }
        }

        private void LeftMouseDown(object sender, MouseEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    File.WriteAllText(filePath, string.Empty);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"An error occurred while writing to the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CheckBoxTopmostChanged(object sender, RoutedEventArgs e)
        {
            if (TopMostCheckBox.IsChecked == true)
            {
                this.Topmost = true; // Corrected property name
            }
            else
            {
                this.Topmost = false; // Corrected property name
            }
        }

        bool HasWipedText = false;
        private void GotFocus(object sender, RoutedEventArgs e)
        {
            if (!HasWipedText)
            {
                LogPathBox.Text = string.Empty;
                HasWipedText = true;
            }
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            if (LogPathBox.Text == string.Empty)
            {
                LogPathBox.Text = "Log Path Here...";
                HasWipedText = false;
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

    }
}
