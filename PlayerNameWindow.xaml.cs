using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Solitaire
{
    /// <summary>
    /// Логика взаимодействия для PlayerNameWindow.xaml
    /// </summary>
    public partial class PlayerNameWindow : Window
    {
        public string PlayerName { get; private set; }

        public PlayerNameWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerName = PlayerNameTextBox.Text;
            DialogResult = true;
            Close();
        }
    }

}
