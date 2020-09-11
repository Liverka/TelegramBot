using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Model;
using System.Net.Http.Headers;
using Nancy.Json;

namespace TelegramBot
{
    class Program
    {
        static ITelegramBotClient botClient;
        static void Main()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(@"https://localhost:5001/Categories");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"id\":4," +
                  "\"name\":\"bla\"}";


                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

            botClient = new TelegramBotClient("1298355350:AAFKwA3vE8R722Q8SlOg4zJaihRJGucNxCk");

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            botClient.OnMessage += Bot_OnMessage;
            botClient.OnCallbackQuery += Bot_OnCallbackQuery;

            botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            botClient.StopReceiving();
        }

        private static async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            if (e.CallbackQuery.Data == "12")
            {
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Недвижимость", "s_11"),
                        InlineKeyboardButton.WithCallbackData("Авто", "s_12"),
                        InlineKeyboardButton.WithCallbackData("Услуги", "s_13")
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Обмен криптовалюты", "s_21"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Назад", "s_31"),
                    }
                });

                await botClient.EditMessageTextAsync(
                    chatId: e.CallbackQuery.Message.Chat.Id,
                    messageId: e.CallbackQuery.Message.MessageId,
                    text: "Сейчас можно продать, купить товары/услуги за BTC",
                    replyMarkup: inlineKeyboard
                );

            }

            if (e.CallbackQuery.Data == "s_31")
            {
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Покупка", "11"),
                        InlineKeyboardButton.WithCallbackData("Продажа", "12"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Мои объявления", "21"),
                    }
                });

                await botClient.EditMessageTextAsync(
                    chatId: e.CallbackQuery.Message.Chat.Id,
                    messageId: e.CallbackQuery.Message.MessageId,
                    text: "Сейчас можно продать, купить товары/услуги за BTC",
                    replyMarkup: inlineKeyboard
                );

            }

        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text == "/start")
            {
                Console.WriteLine($"Received a text message in chat  {e.Message.Chat.Id}.");

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Покупка", "11"),
                        InlineKeyboardButton.WithCallbackData("Продажа", "12"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Мои объявления", "21"),
                    }
                });

                await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat.Id,
                    text: "Сейчас можно продать, купить товары/услуги за BTC",
                    replyMarkup: inlineKeyboard
                );

                //var rkm = new ReplyKeyboardRemove();
                //await botClient.SendTextMessageAsync(e.Message.Chat.Id, "Text", ParseMode.Default, false, false, 0, rkm);
            }

        }

        public void GetMenu(){

            string html = string.Empty;
            string url = @"https://localhost:5001/Categories";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            Console.WriteLine(html);


        }

        public static async Task<Uri> CreateProductAsync()
        {
            Category category = new Category
            {
                name = "Gizmo",
            };

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:5001/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsJsonAsync(
                "Categories", category);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }
    }
}
