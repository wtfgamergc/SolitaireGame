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
using Solitaire.Records;

namespace Solitaire.Records
{
    public partial class RecordsWindow : Window
    {
        private readonly RecordsService _recordsService;

        public RecordsWindow()
        {
            InitializeComponent();
            _recordsService = new RecordsService();
            LoadRecords();
        }

        private void LoadRecords()
        {
            var records = _recordsService.GetRecords();
            RecordsDataGrid.ItemsSource = records;
        }


    }
}
