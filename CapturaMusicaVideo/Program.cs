using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using VideoLibrary;
using MediaToolkit;
using MediaToolkit.Model;
using CapturaMusicaVideo.Dados;

namespace CapturaMusicaVideo
{
    class Program
    {
        static int decide;

        static void Main(string[] args)
        {
            string continua;

            BoasVindas();

            do
            {
                Console.WriteLine("\nO que deseja fazer: \n\n1. Baixar Música \n2. Baixar Video \n3. Sair");
                decide = int.Parse(Console.ReadLine());

                switch (decide)
                {
                    case 1:
                    case 2:
                        BaixarItem();
                        break;
                    case 3:
                        Console.WriteLine("\nFechando programa, até mais!");
                        Thread.Sleep(1500);
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\nComando Inválido!");
                        break;
                }

                Console.WriteLine("\nDeseja Continuar no Programa? (Sim ou Não) ");
                continua = Console.ReadLine();

            } while (continua.ToUpper() == "SIM");

            Console.WriteLine("\nFechando programa, até mais!");
            Thread.Sleep(1500);
            Environment.Exit(0);
        }

        static void BoasVindas()
        {
            string usuarioAmbiente = Environment.GetEnvironmentVariable("USERNAME");
            Console.WriteLine($"\nOlá {usuarioAmbiente}, bem vindo a linguagem C#");
        }

        static void BaixarItem()
        {
            string itemBusca;

            if (decide == 1)
            {
                Console.Write($"\nDigite o nome do artista e música: ");
                itemBusca = Console.ReadLine();
            }
            else
            {
                Console.Write("\nDigite o nome do video: ");
                itemBusca = Console.ReadLine();
            }

            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            var options = new ChromeOptions();

            service.HideCommandPromptWindow = true;
            options.AddArguments("--headless", "--disable-gpu", "--mute-audio");

            IWebDriver driver = new ChromeDriver(service, options);
            Console.WriteLine("\nChromeDriver Iniciado");

            Actions action = new Actions(driver);
            Console.WriteLine("\nBuscando...");

            driver.Navigate().GoToUrl("https://www.youtube.com/results?search_query=" + itemBusca);

            Thread.Sleep(1500);

            IWebElement titulo = driver.FindElement(By.Id("title-wrapper"));
            Console.WriteLine($"\n{titulo.Text} Encontrado");

            action.MoveToElement(titulo).Perform();
            action.Click(titulo).Perform();

            Thread.Sleep(1500);

            string url = driver.Url;

            driver.Close();
            Console.WriteLine("\nIniciando Download");

            SaveMP3(url);
            Console.WriteLine("\nDonwload Finalizado");
        }

        static void SaveMP3(string VideoURL)
        {
            var youtube = YouTube.Default;

            var vid = youtube.GetVideo(VideoURL);

            string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);

            string[] paths = new string[] { downloadsPath, vid.FullName };
            string fullPath = Path.Combine(paths);

            File.WriteAllBytes(fullPath, vid.GetBytes());

            if (decide == 1)
            {
                paths = new string[] { downloadsPath, vid.Title + ".mp3" };
                string fullPathConvert = Path.Combine(paths);

                var inputFile = new MediaFile { Filename = fullPath };
                var outputFile = new MediaFile { Filename = fullPathConvert };

                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);

                    engine.Convert(inputFile, outputFile);
                }
                File.Delete(fullPath);
            }
        }
    }
}