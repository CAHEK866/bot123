using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;

namespace WeatherBot
{
    class Program
    {
        private static string token { get; set; } = "1910345991:AAEp39ll6gnyAeKGSlJpIim_qPMaa143Izs";
        private static TelegramBotClient client;

        static string NameCity;
        static float tempOfCity;
        static string nameOfCity;
        static string answerOnWether;

        public static void Main(string[] args)
        {
            client = new TelegramBotClient(token) { Timeout = TimeSpan.FromSeconds(10) };

            var me = client.GetMeAsync().Result;
            Console.WriteLine($"Bot_Id: {me.Id} \nBot_Name: {me.FirstName} ");

            client.OnMessage += Bot_OnMessage;
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Type == MessageType.Text)
            {
                if (message.Text == "/start")
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Привет! Я погодный бот, я отправляю тебе погоду в твоем городе, для начала напиши свой город.", replyToMessageId: message.MessageId);
                    return;
                }

                if (!string.IsNullOrEmpty(message.Text) && message.Text != "Актуальная погода" && message.Text != "Погода на завтра")
                {
                    NameCity = message.Text;
                    Pogoda_seichas(NameCity);

                    await client.SendTextMessageAsync(message.Chat.Id, $"{answerOnWether} \n\nТемпература в {nameOfCity}: {Math.Round(tempOfCity)} °C");

                    await client.SendTextMessageAsync(message.Chat.Id, "Нажмите 'Актуальная погода' для обновления данных.", replyMarkup: GetButtons());
                    Console.WriteLine(message.Text);
                }

                else if (message.Text.ToLower() == "актуальная погода".ToLower())
                {
                    if (string.IsNullOrEmpty(NameCity))
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Пожалуйста, напиши свой город.");
                    }
                    else
                    {
                        Pogoda_seichas(NameCity);

                        await client.SendTextMessageAsync(message.Chat.Id, $"{answerOnWether} \n\nТемпература в {nameOfCity}: {Math.Round(tempOfCity)} °C");
                    }
                }
                else if (message.Text == "Погода на завтра")
                {
                    if (string.IsNullOrEmpty(NameCity))
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Пожалуйста, напиши свой город.");
                    }
                    else
                    {
                        string forecastMessage = Pogoda_na_zavtra(NameCity);
                        await client.SendTextMessageAsync(message.Chat.Id, forecastMessage);
                    }
                }
            }
        }

        public static string Pogoda_na_zavtra(string cityName)
        {
            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid=2351aaee5394613fc0d14424239de2bd&units=metric";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                //Console.WriteLine(response); 
                WeatherForecastResponse forecastResponse = JsonConvert.DeserializeObject<WeatherForecastResponse>(response);

                var nextDayForecast = forecastResponse.List
                   
                    .FirstOrDefault();

                if (nextDayForecast != null)
                {
                    float tempNow = (float)Math.Round(nextDayForecast.Main.Temp);
                    float tempMin = (float)Math.Round(nextDayForecast.Main.Temp_Min); 
                    float tempMax = (float)Math.Round(nextDayForecast.Main.Temp_Max);
                    float feelsLike = (float)Math.Round(nextDayForecast.Main.Feels_Like);

                    string forecastMessage = $"Прогноз на завтра в {nameOfCity}: \nТемпература завтра: {tempNow}°C\nМинимальная температура: {tempMin}°C\nМаксимальная температура: {tempMax}°C\nОщущаемая температура: {feelsLike}°C";

                    return forecastMessage; 
                }
                else
                {
                    return ""
                        ; 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Возникло исключение:");
                return "" + e.Message;
            }
        }


        
        public static void Pogoda_seichas(string cityName)
        {
            try
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?q=" + cityName + "&unit=metric&appid=2351aaee5394613fc0d14424239de2bd";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);

                nameOfCity = weatherResponse.Name;
                tempOfCity = weatherResponse.Main.Temp - 273;
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("Возникло исключение");
                return;
            }
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
        {
            new List<KeyboardButton> { new KeyboardButton { Text = "Актуальная погода" } },
            new List<KeyboardButton> { new KeyboardButton { Text = "Погода на завтра" } }
        },
                ResizeKeyboard = true,
            };
            }
    }
}

