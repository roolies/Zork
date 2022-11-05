using System;
using System.Collections.Generic;

namespace Zork.Common
{
    public class Game
    {
        public World World { get; }

        public Player Player { get; }

        public Room Room { get; }
        
        public IOutputService Output { get; private set; }

        public Game(World world, string startingLocation)
        {
            World = world;
            Player = new Player(World, startingLocation);
        }
        
        public void Run(IOutputService output)
        {
            Output = output;

            Room previousRoom = null;
            bool isRunning = true;
            while (isRunning)
            {
                Output.WriteLine(Player.CurrentRoom);
                if (previousRoom != Player.CurrentRoom)
                {
                    Output.WriteLine(Player.CurrentRoom.Description);
                    previousRoom = Player.CurrentRoom;
                }
               
                Output.Write("> ");

                string inputString = Console.ReadLine().Trim();
                
                char  separator = ' ';
                string[] commandTokens = inputString.Split(separator);

                string verb = null;
                string subject = null;

                    if (commandTokens.Length == 0)
                {
                    continue;
                }
                else if (commandTokens.Length == 1)
                {
                    verb = commandTokens[0];

                }
                else
                {
                    verb = commandTokens[0];
                    subject = commandTokens[1];
                }

                Commands command = ToCommand(verb);
                string outputString;
                switch (command)
                {
                    case Commands.Quit:
                        isRunning = false;
                        outputString = "Thank you for playing!";
                        break;

                    case Commands.Look:
                        Output.WriteLine(Player.CurrentRoom.Description);
                        foreach (Item item in Player.CurrentRoom.Inventory)
                        {
                            Output.WriteLine(item.Description);
                        }
                        outputString = null;
                        break;

                    case Commands.North:
                    case Commands.South:
                    case Commands.East:
                    case Commands.West:
                        Directions direction = (Directions)command;
                        if (Player.Move(direction))
                        {
                            outputString = $"You moved {direction}.";
                        }
                        else
                        {
                            outputString = "The way is shut!";
                        }
                        break;

                    case Commands.Take:
                       
                        foreach (Item item in Player.CurrentRoom.Inventory)
                        {
                            if (string.Compare(item.Name, subject, ignoreCase: true) == 0)
                            {
                                Player.Inventory.Add(item);
                                Player.CurrentRoom.Inventory.Remove(item);
                                Output.WriteLine("Taken.");
                                break;
                            }
                            else
                            {
                                Output.WriteLine("You can't see any such thing.");
                            }
                            break;
                        }
                        outputString = null;
                            break;

                    case Commands.Drop:
                       
                        foreach (Item item in Player.Inventory)
                        {
                            if (string.Compare(item.Name, subject, ignoreCase: true) == 0)
                            {
                                Player.Inventory.Remove(item);
                                Player.CurrentRoom.Inventory.Add(item);
                                Output.WriteLine("Dropped.");
                                break;
                            }
                            else
                            {
                                Output.WriteLine("You aren't holding that item.");
                            }
                            break;
                        }
                        outputString = null;
                        break;

                    case Commands.Inventory:
                      
                        if (Player.Inventory.Count > 0)
                        {
                            foreach (Item item in Player.Inventory)
                            {
                                Output.WriteLine(item.Name);
                            }
                        }
                        else
                        {
                            Output.WriteLine("You are empty handed.");
                        }
                        
                        outputString = null;
                        break;

                    default:
                        outputString = "Unknown command.";
                        break;
                }
               
                 Output.WriteLine(outputString);
            }
        }
        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}
