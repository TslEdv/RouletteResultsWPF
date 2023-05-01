using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace RouletteWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Storyboard? popupStoryboardIn;
        private Storyboard? popupStoryboardOut;
        public MainWindow()
        {
            InitializeComponent();
            _Board_AddNumbers();
            _Board.SizeChanged += _Board_Resize;
            ConfigurePopUp();
            StartListening();
        }

        private void StartListening()
        {
            // Start listening for incoming connections on port 4948
            TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 4948);
            tcpListener.Start();

            // Run the listener code in a separate task to avoid blocking the UI thread
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        // Wait for a client to connect
                        TcpClient client = tcpListener.AcceptTcpClient();

                        // Read the message sent by the client
                        NetworkStream stream = client.GetStream();
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        ResponseMessage? message = null;

                        // Try and deserialize the message
                        try
                        {
                            message = JsonSerializer.Deserialize<ResponseMessage>(data);
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        // Check if the message is not empty
                        if (message == null)
                        {
                            continue;
                        }

                        // Check if the qualifier is correct
                        if (message.Qualifier != "showWinningNumber")
                        {
                            continue;
                        }

                        // Get the winning number
                        var winningNumber = message.Data.WinningNumber;
                        DisplayResults(winningNumber);

                        // Close the client connection
                        client.Close();
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that occur while listening for incoming connections
                        MessageBox.Show(ex.Message);
                    }
                }
            });
        }

        private void DisplayResults(int winningNumber)
        {

            // Check if the message is not empty
            if (winningNumber >= 0 && winningNumber <= 36)
            {

                // Find the correct label
                Label? label = null;
                Dispatcher.Invoke(() =>
                {
                    label = (Label)FindName("_" + winningNumber);
                });

                Label? dozen = null;
                Label? evenOdd = null;
                Label? color = null;
                Label? lowHigh = null;
                Label? row = null;

                // If winning number is not null, find the correct labels
                if (winningNumber != 0)
                {
                    // Find the correct dozen
                    if (winningNumber < 13)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            dozen = (Label)FindName("_1st12");
                        });
                    }
                    else if (winningNumber < 25)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            dozen = (Label)FindName("_2nd12");
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            dozen = (Label)FindName("_3rd12");
                        });
                    }

                    // Find even or odd
                    if (winningNumber % 2 == 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            evenOdd = (Label)FindName("_Even");
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            evenOdd = (Label)FindName("_Odd");
                        });
                    }

                    // Find the correct color                   
                    Dispatcher.Invoke(() =>
                    {
                        if (label!.Background == Brushes.Red)
                        {
                            color = (Label)FindName("_Red");
                        }
                        else
                        {
                            color = (Label)FindName("_Black");
                        }
                    });


                    // Find low or high
                    if (winningNumber < 19)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            lowHigh = (Label)FindName("_Low");
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            lowHigh = (Label)FindName("_High");
                        });
                    }

                    // Find correct row
                    if (winningNumber % 3 == 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            row = (Label)FindName("_RowA");
                        });
                    }
                    else if (winningNumber % 3 == 2)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            row = (Label)FindName("_RowB");
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            row = (Label)FindName("_RowC");
                        });
                    }
                }

                // Save the original colors of labels
                Brush? originalColor = null;
                Brush? originalDozenColor = null;
                Brush? originalEvenOddColor = null;
                Brush? originalColorColor = null;
                Brush? originalLowHighColor = null;
                Brush? originalRowColor = null;
                Dispatcher.Invoke(() =>
                {
                    // Save the original colors of labels that are not null
                    if (row != null)
                    {
                        originalRowColor = row!.Background;
                    }
                    if (lowHigh != null)
                    {
                        originalLowHighColor = lowHigh!.Background;
                    }
                    if (color != null)
                    {
                        originalColorColor = color!.Background;
                    }
                    if (evenOdd != null)
                    {
                        originalEvenOddColor = evenOdd!.Background;
                    }
                    if (dozen != null)
                    {
                        originalDozenColor = dozen!.Background;
                    }
                    if (label != null)
                    {
                        originalColor = label!.Background;
                    }
                });

                // Change the colors of labels to cyan
                Dispatcher.Invoke(() =>
                {
                    // Change the colors of labels that are not null
                    if (row != null)
                    {
                        row.Background = Brushes.Cyan;
                    }
                    if (lowHigh != null)
                    {
                        lowHigh.Background = Brushes.Cyan;
                    }
                    if (color != null)
                    {
                        color.Background = Brushes.Cyan;
                    }
                    if (evenOdd != null)
                    {
                        evenOdd.Background = Brushes.Cyan;
                    }
                    if (dozen != null)
                    {
                        dozen.Background = Brushes.Cyan;
                    }
                    if (label != null)
                    {
                        label.Background = Brushes.Cyan;
                    }
                });

                // Update the TextBlock with the winning number and positions
                Dispatcher.Invoke(() =>
                {
                    if (winningNumber == 0)
                    {
                        popupTextBlock.Content = $"Winner:\n" +
                        $"{winningNumber}\n" +
                        $"Green";
                    }
                    else
                    {
                        popupTextBlock.Content = $"Winner:\n" +
                        $"{winningNumber}\n" +
                        $"{color!.Content}\n" +
                        $"{evenOdd!.Content}\n" +
                        $"{dozen!.Content}\n" +
                        $"{row!.Name}\n" +
                        $"{lowHigh!.Content}";
                    }
                    popupTextBlock.Visibility = Visibility.Visible;
                    popupStoryboardIn!.Begin();
                });

                // Wait for 10 seconds before hiding the popup
                Thread.Sleep(TimeSpan.FromSeconds(10));

                // Change the colors of labels back to original
                Dispatcher.Invoke(() =>
                {
                    // Change the colors of labels that are not null
                    if (label != null)
                    {
                        label.Background = originalColor;
                    }
                    if (dozen != null)
                    {
                        dozen.Background = originalDozenColor;
                    }
                    if (evenOdd != null)
                    {
                        evenOdd.Background = originalEvenOddColor;
                    }
                    if (color != null)
                    {
                        color.Background = originalColorColor;
                    }
                    if (lowHigh != null)
                    {
                        lowHigh.Background = originalLowHighColor;
                    }
                    if (row != null)
                    {
                        row.Background = originalRowColor;
                    }
                });

                Dispatcher.Invoke(() =>
                {
                    popupStoryboardOut!.Begin();
                });

                // Wait for the fade-out animation to complete before hiding the pop-up
                manualResetEvent.WaitOne();

                Dispatcher.Invoke(() =>
                {
                    popupTextBlock.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void ConfigurePopUp()
        {
            // Create the storyboard for the pop-up effect (fade in)
            popupStoryboardIn = new Storyboard();
            var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            Storyboard.SetTarget(fadeInAnimation, popupTextBlock);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(Label.OpacityProperty));
            popupStoryboardIn.Children.Add(fadeInAnimation);

            // Create the storyboard for the pop-up effect (fade out)
            popupStoryboardOut = new Storyboard();
            var fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            Storyboard.SetTarget(fadeOutAnimation, popupTextBlock);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(Label.OpacityProperty));
            popupStoryboardOut.Children.Add(fadeOutAnimation);

            // Set the completed event for the fade-out animation
            popupStoryboardOut.Completed += PopupStoryboardOut_Completed!;
        }
        
        // Create a manual reset event to wait for the fade-out animation to complete
        private ManualResetEvent manualResetEvent = new ManualResetEvent(false);


        // Event handler for the fade-out animation
        private void PopupStoryboardOut_Completed(object sender, EventArgs e)
        {
            manualResetEvent.Set();
            popupStoryboardOut!.Stop();
        }

        // Event handler for the SizeChanged event of the Grid
        private void _Board_Resize(object sender, SizeChangedEventArgs e)
        {
            Regex regex = new Regex(@"_\d{1,2}\b");
            foreach (Label label in _Board.Children.OfType<Label>())
            {
                double scaleFactor = Math.Min(this.ActualWidth / 640, this.ActualHeight / 480); // adjust values based on your desired scaling
                double margin = 2 * scaleFactor; // adjust value based on your desired margin

                label.Margin = new Thickness(margin);
                // Resize the number labels
                if (regex.IsMatch(label.Name) && label.Name != "_0")
                {
                    Grid grid = _Board;
                    int rows = grid.RowDefinitions.Count;
                    int columns = grid.ColumnDefinitions.Count;
                    double availableHeight = grid.ActualHeight - (label.Margin.Top + label.Margin.Bottom) * rows;
                    double availableWidth = grid.ActualWidth - (label.Margin.Left + label.Margin.Right) * columns;
                    double cellHeight = availableHeight / rows;
                    double cellWidth = availableWidth / columns;
                    label.Height = cellHeight;
                    label.Width = cellWidth;
                    label.FontSize = Math.Min(cellHeight, cellWidth) * 0.4;
                }
                // Resize zero label
                else if (label.Name == "_0")
                {
                    label.SizeChanged += (sender, e) =>
                    {
                        Label currentLabel = (Label)sender;
                        double fontSize = Math.Min(currentLabel.ActualWidth, currentLabel.ActualHeight) * 0.4;
                        currentLabel.FontSize = fontSize;
                    };
                }
                // Resize the popup label
                else if (label.Name == "popupTextBlock")
                {
                    label.SizeChanged += (sender, e) =>
                    {
                        Label currentLabel = (Label)sender;
                        double fontSize = Math.Min(currentLabel.ActualWidth / 3, currentLabel.ActualHeight / 2) * 0.4;
                        currentLabel.FontSize = fontSize;
                    };
                }
                // Resize rest of the labels
                else
                {
                    label.SizeChanged += (sender, e) =>
                    {
                        Label currentLabel = (Label)sender;
                        double fontSize = Math.Min(currentLabel.ActualWidth / 2, currentLabel.ActualHeight) * 0.4;
                        currentLabel.FontSize = fontSize;
                    };
                }
            }
        }

        // Add the labels to the Grid
        private void _Board_AddNumbers()
        {
            Grid grid = _Board;
            int rowCounter = 2;
            int columnCounter = 1;
            for (int i = 1; i < 37; i++)
            {
                Label label = new Label();

                // If no label with the name exists, create a new label
                if ((Label)FindName("_" + i) == null)
                {
                    RegisterName("_" + i, label);
                    label.Content = i;
                    label.Name = "_" + i;
                    label.Foreground = Brushes.White;
                    if (i % 2 == 0)
                    {
                        label.Background = Brushes.Black;
                    }
                    else
                    {
                        label.Background = Brushes.Red;
                    }
                    label.BorderBrush = Brushes.White;
                    label.BorderThickness = new Thickness(2);
                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    label.VerticalContentAlignment = VerticalAlignment.Center;
                    Grid.SetRow(label, rowCounter);
                    Grid.SetColumn(label, columnCounter);
                    Grid.SetRowSpan(label, 1);
                    Grid.SetColumnSpan(label, 1);
                    grid.Children.Add(label);
                }

                // Count rows and columns
                if (i % 3 == 0)
                {
                    rowCounter = 2;
                    columnCounter++;
                }
                else
                {
                    rowCounter--;
                }
            }
        }
    }
}
