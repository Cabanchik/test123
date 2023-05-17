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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace WpfApp5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();            

       }
     
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string coordResponse = "";
            string weatherResponse = "";
            //конструкция чтобы программа не крашилась при оставленном пустом поле
            if (countryName.Text == "")
            {
                MessageBox.Show("all fields must be filled");
                return;
            }
            //трай чтобы программа не крашилась (если апи сломается или что-то типо этого)
            try
            {
                //Апи запрос на коорды введеного города
                HttpWebRequest cityCoordsRequest = (HttpWebRequest)WebRequest.Create("http://api.openweathermap.org/geo/1.0/direct?q=" + countryName.Text + "&appid=4086105d3bb4d764d1ffa756afafb0b3");
                //Получение ответа от апи
                HttpWebResponse coordsJson = (HttpWebResponse)cityCoordsRequest.GetResponse();                
                using (StreamReader streamReader = new StreamReader(coordsJson.GetResponseStream()))
                {
                    coordResponse = streamReader.ReadToEnd();

                }
                //тут удаляются знаки [] потому что ответ о координатах приходит (зачем-то) в массиве
                string arrayRemover = coordResponse.Replace("[", "").Replace("]", "");
                //дессириализация из Json 
                var coordsOutput = JsonConvert.DeserializeObject<CityInfo>(arrayRemover);
                //конструкция чтобы не крашилось если пользователь введет название неправильно
                if (coordsOutput != null)
                {
                    //апи запрос на погоду в веденном городе
                    HttpWebRequest weatherRequest = (HttpWebRequest)WebRequest.Create("https://api.openweathermap.org/data/2.5/weather?lat=" + coordsOutput.lat + "&lon=" + coordsOutput.lon + "&units=metric&appid=4086105d3bb4d764d1ffa756afafb0b3");
                    HttpWebResponse weatherJson = (HttpWebResponse)weatherRequest.GetResponse();
                    using (StreamReader streamReader = new StreamReader(weatherJson.GetResponseStream()))
                    {
                        weatherResponse = streamReader.ReadToEnd();
                    }

                    var weatherOutput = JsonConvert.DeserializeObject<WeatherOutput>(weatherResponse);
                    //создаются экземпляры классов из Json чтобы вывести данные пользователю
                    Weather weather = weatherOutput.weather[0];
                    Main mainTemp = weatherOutput.main;
                    Wind wind = weatherOutput.wind;
                    //вывод
                    info.Content = "Weather is: " + weather.main + ", " + weather.description;
                    temp.Content = "Temperature is: " + mainTemp.temp + "°C";
                    windSpeed.Content = "Wind speed is: " + wind.speed + "m/ph";

                   
                }
                else
                {
                    MessageBox.Show("City name is incorrect");
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong");
            }

            

            

        }
    }
}
