using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TangoAttack3;

namespace TangoAttack.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /*
        public ChartValues<City> Results { get; set; }
        public ObservableCollection<string> Labels { get; set; }
        public Func<double, string> MillionFormatter { get; set; }
        public object Mapper { get; set; }
        */

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private Simulator Simulator { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Simulator = new Simulator(1000, 8);

            /*
            var DataBase = new List<City>()
            {
                new City() { Id = new Guid(), Name = "lunes", Population = 15.23, Area = 2.3, Country = "Germany" },
                new City() { Id = new Guid(), Name = "martes", Population = 10.23, Area = 9.3, Country = "Perukistán" },
                new City() { Id = new Guid(), Name = "miércoles", Population = 35.23, Area = 3.5, Country = "Colombia" },
                new City() { Id = new Guid(), Name = "jueves", Population = 1.3, Area = 12.5, Country = "VARsil" },
            };

            //lets configure the chart to plot cities
            Mapper = Mappers.Xy<City>()
                .X((city, index) => index)
                .Y(city => city.Population);

            //lets take the first 15 records by default;
            var records = DataBase.ToList();

            //Results = records;
            Results = records.AsChartValues();

            Labels = new ObservableCollection<string>(records.Select(x => x.Name));

            MillionFormatter = value => (value / 1000000).ToString("N") + "M";

            DataContext = this;
            */
            DataContext = this;

        }

        private void BitLength_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Solo permite números
            if (!int.TryParse(e.Text, out int result))
            {
                e.Handled = true;
                return;
            }

            // Obtén el texto actual del TextBox
            TextBox textBox = sender as TextBox;
            string newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Valida que el número esté entre 4 y 8
            if (int.TryParse(newText, out int newValue))
            {
                if (newValue < 4 || newValue > 8)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void BitLength_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Evita que se ingresen caracteres no deseados como espacios
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void StartSimulationBtn_Click(object sender, RoutedEventArgs e)
        {
            if(this.BitLength.Text == "")
            {
                MessageBox.Show("Por favor, ingrese un valor para la longitud de bits.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (this.SessionsTextBox.Text == "")
            {
                MessageBox.Show("Por favor, ingrese un valor para la cantidad de sesiones.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                Simulator = new Simulator(96, int.Parse(this.BitLength.Text));
                Simulator.StartSimulation();

                var (k1, k2, id) = Simulator.GetK1K2ID();

                this.K1.Text = Convert.ToString(k1, 2).PadLeft(Simulator.BitsLength, '0');
                this.K2.Text = Convert.ToString(k2, 2).PadLeft(Simulator.BitsLength, '0');
                this.ID.Text = Convert.ToString(id, 2).PadLeft(Simulator.BitsLength, '0');

                var (k1e, k2e, ide) = Simulator.GetK1K2IDEstimation();

                this.K1E.Text = Convert.ToString(k1e, 2).PadLeft(Simulator.BitsLength, '0');
                this.K2E.Text = Convert.ToString(k2e, 2).PadLeft(Simulator.BitsLength, '0');
                this.IDE.Text = Convert.ToString(ide, 2).PadLeft(Simulator.BitsLength, '0');

                var data = Simulator.GetData();

                SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "K1 Distances",
                        Values = new ChartValues<int>(data.Item1)
                    },
                    new LineSeries
                    {
                        Title = "K2 Distances",
                        Values = new ChartValues<int>(data.Item2)
                    },
                    new LineSeries
                    {
                        Title = "ID Distances",
                        Values = new ChartValues<int>(data.Item3)
                    }
                };

                Labels = new List<string>();
                for (int i = 0; i < data.Item1.Count; i++)
                {
                    Labels.Add(i.ToString());
                }

                OnPropertyChanged(nameof(SeriesCollection));
                OnPropertyChanged(nameof(Labels));

                if (k1 == k1e && k2 == k2e && id == ide)
                {
                    MessageBox.Show("El ataque ha sido exitoso.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("El ataque ha fallado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SessionsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Solo permite números
            if (!int.TryParse(e.Text, out int result))
            {
                e.Handled = true;
                return;
            }

            // Obtén el texto actual del TextBox
            TextBox textBox = sender as TextBox;
            string newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Valida que el número esté entre 4 y 8
            if (int.TryParse(newText, out int newValue))
            {
                if (newValue < 0 || newValue > 9999999)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void SessionsTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Evita que se ingresen caracteres no deseados como espacios
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }

    public class City
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Population { get; set; }
        public double Area { get; set; }
        public string Country { get; set; }
    }
}