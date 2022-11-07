using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Zork.Common;

namespace Zork.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            const string defaultRoomsFilename = @"Content\Game.json";
            string gameFilename = (args.Length > 0 ? args[(int)CommandLineArguments.GameFilename] : defaultRoomsFilename);
            Game game = JsonConvert.DeserializeObject<Game>(File.ReadAllText(gameFilename));

            var output = new ConsoleOutputService();
            var input = new ConsoleInputService();

            Console.WriteLine("Welcome to Zork!");
            game.Run(input, output);

            while (game.IsRunning)
            {
                game.Output.Write("> ");
                input.ProcessInput();
            }
            output.WriteLine("Finished.");
        }

        private enum CommandLineArguments
        {
            GameFilename = 0
        }
    }
}