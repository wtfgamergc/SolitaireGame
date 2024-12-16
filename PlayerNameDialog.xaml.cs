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
    /// Логика взаимодействия для PlayerNameDialog.xaml
    /// </summary>
    public partial class PlayerNameDialog : Window
    {
        public string PlayerName => PlayerNameTextBox.Text;

        public PlayerNameDialog()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PlayerName))
            {
                DialogResult = true;  // Закрываем окно, если имя введено
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите имя.");
            }
        }
    }

}
