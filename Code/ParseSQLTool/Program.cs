using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using ParseSQLTool.Database;
using ParseSQLTool.Loader;

namespace ParseSQLTool
{
    public class Program
    {
        /// <summary>
        /// Основной запуск программы.
        /// </summary>
        private static void Main()
        {
            var loader = new LoadSettings(); // Загружаемые настройки парсера из файла Settings.ini находиться в папке с парсером.
            var settings = loader.Settings; // Получение ссылки на настройки
            var connection = new Connection(settings.HostServer, settings.UserName, settings.DataBaseName, settings.Password); // Установка соединения с базой данных MySQL.
            var connect = connection.Connect(); // Попытка соединения.
            if (connect)// Если попытка удачна, то
            {
                while (true) // Ставим парсер в режим работы на проверку каждые 10 минут, время можно изменить в настройках.
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[" + DateTime.Now.ToString() + "] " + "Запускаю проверку данных " );
                    MainMethod(settings,connection);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + "Проверка данных через: " + settings.MinutesTimeCheck + " минут");
                    Thread.Sleep(settings.MinutesTimeCheck * 60000);
                }
            }
        }
        /// <summary>
        /// Основной метод.
        /// </summary>
        /// <param name="connection">На вход подаётся соединение с MySQL.</param>
        private static void MainMethod(Settings settings,Connection connection)
        {
            var total = 0; // Количество записей в источнике.
            CheckTotalCount(out total); // Получаем данные.
            if(total != 0) // Если при подключении не произошла ошибка, то
            {
                var root = LoadData(total); // Загружаем данные из источника.
                if(root != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    var getAll = "SELECT * FROM " + settings.DatabaseTable;
                    var command = new MySqlCommand(getAll, connection.MySQLConnection);
                    command.CommandTimeout = 99999;
                    var reader = command.ExecuteReader();
                    var dataInBase = false;
                    while (reader.Read() && !dataInBase)
                    {
                        if (!dataInBase)
                        {
                            dataInBase = true;
                        }
                    }
                    reader.Close();
                    LoadDataInClearDataBase(root, settings, connection, command, dataInBase, total);
                    Console.WriteLine();
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] " + "Запись успешно выполнена");
                }
            }
        }

        /// <summary>
        /// Метод, выполняющий полную загрузку данных в базу в случае, если она пуста.
        /// </summary>
        /// <param name="root">Входные данные</param>
        /// <param name="connection">Соединение с MySql</param>
        /// <param name="command">Ссылка на команду</param>
        /// <param name="dataInBase">Является ли база данных пустой</param>
        /// <param name="total">Количество записей которое требуется внести.</param>
        /// <returns></returns>
        private static void LoadDataInClearDataBase(Root root, Settings settings,Connection connection, MySqlCommand command, bool dataInBase,int total)
        {
            if (dataInBase)
            {
                var delete = "DELETE FROM " + settings.DatabaseTable;
                command = new MySqlCommand(delete, connection.MySQLConnection);
                command.ExecuteNonQuery();
                dataInBase = false;
            }
            if (!dataInBase)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[" + DateTime.Now.ToString() + "] " + "Выполняю запись полученных данных в базу ");
                var index = 1;
                using (var progress = new ProgressBar())
                {
                    foreach (var item in root.Data.searchReportWoodDeal.content)
                    {
                        var idDeclaration = item.dealNumber;
                        var nameSeller = item.sellerName;
                        if (nameSeller == null)
                        {
                            nameSeller = "";
                        }
                        else
                        {
                            nameSeller = nameSeller.Replace("\"", "");
                            nameSeller = nameSeller.Replace("\'", "");
                        }
                        var numberSeller = item.sellerInn;
                        if (numberSeller == null)
                        {
                            numberSeller = "";
                        }
                        var nameBuyer = item.buyerName;
                        if (nameBuyer == null)
                        {
                            nameBuyer = "";
                        }
                        else
                        {
                            nameBuyer = nameBuyer.Replace("\"", "");
                            nameBuyer = nameBuyer.Replace("\'", "");
                        }
                        var numberBuyer = item.buyerInn;
                        if (nameBuyer == null)
                        {
                            numberBuyer = "";
                        }
                        var dateTransation = item.dealDate;
                        if (dateTransation == null)
                        {
                            dateTransation = "";
                        }
                        var volumeReport = "Пр: " + item.woodVolumeSeller.ToString().Replace(",", ".") + "/ " + "Пк: " + item.woodVolumeBuyer.ToString().Replace(",", ".");
                        //Запрос на добавление данных в базу в таблицу Transations
                        var insertSQL = "INSERT INTO " + settings.DatabaseTable + "(idDeclaration,nameSeller,numberSeller,nameBuyer,numberBuyer,dateTransation,volumeReport) VALUES (" + "'" + idDeclaration + "'" + "," + "'" + nameSeller + "'" + "," + "'" + numberSeller + "'" + "," + "'" + nameBuyer + "'" + "," + "'" + numberBuyer + "'" + "," + "'" + dateTransation + "'" + "," + "'" + volumeReport + "'" + ")";

                        command = new MySqlCommand(insertSQL, connection.MySQLConnection);
                        command.ExecuteNonQuery();
                        progress.Report(((index * 100) / total) * 0.01d);
                        index++;
                    }
                }
            }
        }
        /// <summary>
        /// Метод, занимающийся проверкой количества записей из источника.
        /// </summary>
        /// <param name="total">Возвращает количество записей источника.</param>
        private static void CheckTotalCount(out int total)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green; 
                var web = (HttpWebRequest)HttpWebRequest.Create("https://www.lesegais.ru/open-area/graphql");
                //Запрос на получение количества записей от источника, был получен из консоли браузера при использовании подключения Fetch на Java.
                var body = "{\"query\":\"query SearchReportWoodDealCount($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {\\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) {\\n    total\\n    number\\n    size\\n    overallBuyerVolume\\n    overallSellerVolume\\n    __typename\\n  }\\n}\\n\",\"variables\":{\"size\":20,\"number\":0,\"filter\":null},\"operationName\":\"SearchReportWoodDealCount\"}";
                var dataBody = Encoding.Default.GetBytes(body); // Шифруем запрос в массив байт.
                web.Method = "POST"; // Заставляем сайт думать что мы браузер, заголовки, фереры, были также получены из браузера.
                web.Accept = "*/*";
                web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
                web.ContentType = "application/json";
                web.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru,en;q=0.9");
                web.Headers.Add("sec-ch-ua", "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"102\", \"Yandex\";v=\"22\"");
                web.Headers.Add("sec-ch-ua-mobile", "?0");
                web.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                web.Headers.Add("sec-fetch-dest", "empty");
                web.Headers.Add("sec-fetch-mode", "cors");
                web.Headers.Add("sec-fetch-site", "same-origin");
                web.Credentials = CredentialCache.DefaultCredentials;
                web.Referer = "https://www.lesegais.ru/open-area/deal";
                using (var progress = new ProgressBar())
                {
                    using (var stream = web.GetRequestStream()) // Передаём Body в поток чтобы получить необходимые данные из источника. 
                    {
                        progress.Report(0.01d); // Указываем прогресс бару сколько осталось.
                        stream.Write(dataBody, 0, dataBody.Length);
                        using (var twitpicResponse = (HttpWebResponse)web.GetResponse())
                        {
                            using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                            {
                                progress.Report(0.5d);
                                JavaScriptSerializer js = new JavaScriptSerializer(); // Получаем данные из источника
                                var objText = reader.ReadToEnd();
                                var root = JsonConvert.DeserializeObject<Root>(objText); // Преобразуем их в удоборимый вид
                                total = root.Data.searchReportWoodDeal.total;
                                progress.Report(100);
                                Thread.Sleep(1000);
                                Console.WriteLine();
                                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + "Количество записей в источнике: " + total);
                            }
                        }
                    }
                }
                
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + ex.Message);
                total = 0;
            }
        }
        /// <summary>
        /// Метод, занимающийся загрузкой данных из источника.
        /// </summary>
        /// <param name="total">На вход подаётся количество данных, которые требуется загрузить.</param>
        /// <returns>Возвращает полученные данные.</returns>
        private static Root LoadData(int total)
        {
            try
            {
                Root root;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[" + DateTime.Now.ToString() + "] " + "Выполняю загрузку данных из источника ");
                var web = (HttpWebRequest)HttpWebRequest.Create("https://www.lesegais.ru/open-area/graphql");
                //Запрос к источнику, был получен из браузера на основе Fetch JavaScript, выборка данных составляет количество данных в базе источника.
                var body = "{\"query\":\"query SearchReportWoodDeal($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {\\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) {\\n    content {\\n      sellerName\\n      sellerInn\\n      buyerName\\n      buyerInn\\n      woodVolumeBuyer\\n      woodVolumeSeller\\n      dealDate\\n      dealNumber\\n      __typename\\n    }\\n    __typename\\n  }\\n}\\n\",\"variables\":{\"size\":" + total + ",\"number\":0,\"filter\":null,\"orders\":null},\"operationName\":\"SearchReportWoodDeal\"}";
                var dataBody = Encoding.Default.GetBytes(body); // Кодируем запрос.
                web.Method = "POST";//Заставляем думать источник что мы браузер, все заголовки,реферы, были получены из консоли браузера.
                web.Accept = "*/*";
                web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
                web.ContentType = "application/json";
                web.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru,en;q=0.9");
                web.Headers.Add("sec-ch-ua", "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"102\", \"Yandex\";v=\"22\"");
                web.Headers.Add("sec-ch-ua-mobile", "?0");
                web.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                web.Headers.Add("sec-fetch-dest", "empty");
                web.Headers.Add("sec-fetch-mode", "cors");
                web.Headers.Add("sec-fetch-site", "same-origin");
                web.Credentials = CredentialCache.DefaultCredentials;
                web.Referer = "https://www.lesegais.ru/open-area/deal";
                using (var progress = new ProgressBar())
                {
                    using (var stream = web.GetRequestStream()) // Посылаем запрос к источнику.
                    {
                        progress.Report(0.01d); // Указываем прогресс бару сколько осталось.
                        stream.Write(dataBody, 0, dataBody.Length);
                        using (var twitpicResponse = (HttpWebResponse)web.GetResponse())
                        {
                            using (var reader = new StreamReader(twitpicResponse.GetResponseStream())) // Получаем данные от источника
                            {
                                progress.Report(0.5d); // Указываем прогресс бару сколько осталось.
                                JavaScriptSerializer js = new JavaScriptSerializer();
                                var objText = reader.ReadToEnd(); // Считываем данные и преобразуем в удобоваримый вид
                                root = JsonConvert.DeserializeObject<Root>(objText);
                                progress.Report(100);
                                Thread.Sleep(1000);
                                Console.WriteLine();
                                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + "Данные загружены!");
                            }
                        }
                    }
                }

                return root;
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[" + DateTime.Now.ToString() + "] " + ex.Message);
                return null;
            }
            
        }
    }
}
