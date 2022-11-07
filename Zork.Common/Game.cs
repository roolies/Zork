using System;
using System.Collections.Generic;

namespace Zork.Common
{
    public class Game
    {
        public World World { get; }

        public Player Player { get; }

        public IInputService Input { get; private set; }
        
        public IOutputService Output { get; private set; }

       public bool IsRunning { get; private set; }

        public Game(World world, string startingLocation)
        {
            World = world;
            Player = new Player(World, startingLocation);
        }

        public void Run(IInputService input, IOutputService output)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Output = output ?? throw new ArgumentNullException(nameof(output));

            Input.InputReceived += Input_InputReceived;
            IsRunning = true;
            Output.WriteLine(Player.CurrentRoom);
            Output.WriteLine(Player.CurrentRoom.Description);
        }

        private void OnInputReceived(object sender, string inputString)
        {
            Commands command = ToCommand(inputString);

            Room previousLocation = Player.CurrentRoom;
            
            switch (command)
                {
                    case Commands.Quit:
                        isRunning = false;
                        Output.WriteLine("Thank you for playing!");
                        break;

                    case Commands.Look:
                        Output.WriteLine(Player.CurrentRoom.Description);
                        foreach (Item item in Player.CurrentRoom.Inventory)
                        {
                            Output.WriteLine(item.Description);
                        }
                        
                        break;

                    case Commands.North:
                    case Commands.South:
                    case Commands.East:
                    case Commands.West:
                        Directions direction = (Directions)command;
                        if (Player.Move(direction))
                        {
                            Output.WriteLine($"You moved {direction}.");
                        }
                        else
                        {
                            Output.WriteLine("The way is shut!");
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
                        
                        
                        break;

                    default:
                        Output.WriteLine("Unknown command.");
                        break;
                }

        }
        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}
