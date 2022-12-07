﻿using System;
using System.Linq;
using Newtonsoft.Json;

namespace Zork.Common
{
    public class Game
    {
        public World World { get; }

        [JsonIgnore]
        public Player Player { get; }

        [JsonIgnore]
        public IInputService Input { get; private set; }

        [JsonIgnore]
        public IOutputService Output { get; private set; }

        [JsonIgnore]
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

            IsRunning = true;
            Input.InputReceived += OnInputReceived;
            Output.WriteLine("Welcome to Zork!");
            Look();
            Output.WriteLine($"{Player.CurrentRoom}");
        }
        public void OnInputReceived(object sender, string inputString)
        {
            char separator = ' ';
            string[] commandTokens = inputString.Split(separator);

            string verb;
            string subject = null;
            if (commandTokens.Length == 0)
            {
                return;
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

            Room previousRoom = Player.CurrentRoom;
            Commands command = ToCommand(verb);
            switch (command)
            {
                case Commands.Quit:
                    IsRunning = false;
                    Output.WriteLine("Thank you for playing!");
                    break;

                case Commands.Look:
                    Look();
                    break;

                case Commands.North:
                case Commands.South:
                case Commands.East:
                case Commands.West:
                    Directions direction = (Directions)command;
                    Output.WriteLine(Player.Move(direction) ? $"You moved {direction}." : "The way is shut!");
                    
                    break;

                case Commands.Take:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Take(subject);
                    }
                    break;

                case Commands.Drop:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Drop(subject);
                    }
                    break;

                case Commands.Place:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Place(subject);
                    }
                    break;

                case Commands.Remove:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Remove(subject);
                    }
                    break;

                case Commands.Eat:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Eat(subject);
                    }
                    break;

                case Commands.Inventory:
                    if (Player.Inventory.Count() == 0)
                    {
                        Output.WriteLine("You are empty handed.");
                    }
                    else
                    {
                        Output.WriteLine("You are carrying:");
                        foreach (Item item in Player.Inventory)
                        {
                            Output.WriteLine(item.InventoryDescription);
                        }
                    }
                    break;

                case Commands.Reward:
                    Player.Score += 1;

                    break;

                case Commands.Score:
                    Output.WriteLine($"Your score is {Player.Score} in {Player.Moves} move(s).");
                    break;

                default:
                    Output.WriteLine("Unknown command.");
                    break;
            }
            if (command  != Commands.Reward && command != Commands.Score && command != Commands.Unknown)
            {
                Player.Moves += 1;
            }
            if (ReferenceEquals(previousRoom, Player.CurrentRoom) == false)
            {
                Look();
            }

            Output.WriteLine($"{Player.CurrentRoom}");
        }
        
        private void Look()
        {
            Output.WriteLine(Player.CurrentRoom.Description);
            foreach (Item item in Player.CurrentRoom.Inventory)
            {
                Output.WriteLine(item.LookDescription);
            }
        }

        private void Take(string itemName)
        {
            Item itemToTake = Player.CurrentRoom.Inventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (itemToTake == null)
            {
                Output.WriteLine("You can't see any such thing.");                
            }
            else
            {
                Player.AddItemToInventory(itemToTake);
                Player.CurrentRoom.RemoveItemFromInventory(itemToTake);
                Output.WriteLine("Taken.");
                Player.Score += 10;
                
            }
        }
        private void Drop(string itemName)
        {
            Item itemToDrop = Player.Inventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (itemToDrop == null)
            {
                Output.WriteLine("You can't see any such thing.");                
            }
            else
            {
                Player.CurrentRoom.AddItemToInventory(itemToDrop);
                Player.RemoveItemFromInventory(itemToDrop);
                Output.WriteLine("Dropped.");
            }
        }
        private void Place(string itemName)
        {
            Item itemToPlace = Player.Inventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (itemToPlace == null)
            {
                Output.WriteLine("You can't see any such thing.");
            }
           else if (Player.CurrentRoom.IsLivingRoom == true)
            {
                Player.CurrentRoom.AddItemToTrophyCase(itemToPlace);
                Player.RemoveItemFromInventory(itemToPlace);
                Output.WriteLine("Placed in trophy case.");
            }
            else if (Player.CurrentRoom.IsLivingRoom == false)
            {
                Output.WriteLine("There's no trophy case here.");
            }
        }

        private void Remove(string itemName)
        {
            Item itemToRemove = Player.CurrentRoom.TrophyCaseInventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (Player.CurrentRoom.IsLivingRoom == true)
            {
                Player.AddItemToInventory(itemToRemove);
                Player.CurrentRoom.RemoveItemFromTrophyCase(itemToRemove);
                Output.WriteLine("Removed from the trophy case.");
            }
            else if (itemToRemove == null)
            {
                Output.WriteLine("You can't see any such thing.");
            }
            else
            {
                Output.WriteLine("There's no trophy case here.");
            }
        }
        private void Eat(string itemName)
        {
            Item edibleItem = Player.Inventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (edibleItem == null)
            {
                Output.WriteLine("You have nothing to eat.");
            }
            else if (edibleItem.IsEdible == false)
            {
                Output.WriteLine("I don't think this item would agree with you.");
            }
            else if (edibleItem.IsEdible == true)
            {
                Player.RemoveItemFromInventory(edibleItem);
                Output.WriteLine("Thank you very much. It really hit the spot");
            }
        }

        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}