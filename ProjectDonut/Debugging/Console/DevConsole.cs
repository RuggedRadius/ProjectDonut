using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Structures;
using ProjectDonut.UI.ScrollDisplay;
using QuakeConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDonut.Debugging.Console
{
    public static class DevConsole
    {

        public static void InitialiseConsole(Game1 game)
        {
            Global.Debug.Console = new ConsoleComponent(game);

            var interpreter = new ManualInterpreter();
            Global.Debug.Console.Interpreter = interpreter;
            game.Components.Add(Global.Debug.Console);

            RegisterCommands(interpreter, game);
        }

        private static void RegisterCommands(ManualInterpreter interpreter, Game1 game)
        {
            interpreter.RegisterCommand("fog", (args) =>
            {
                if (args.Length != 1)
                {
                    Global.Debug.Console.Output.Append("Usage: fog <value> i.e. \"fog on\" , \"fog off\"");
                    return;
                }

                switch (args[0].ToLower())
                {
                    case "on":
                        Global.SHOW_FOG_OF_WAR = true;
                        break;

                    case "off":
                        Global.SHOW_FOG_OF_WAR = false;
                        break;
                }
            });

            interpreter.RegisterCommand("chunklines", (args) =>
            {
                if (args.Length != 1)
                {
                    Global.Debug.Console.Output.Append("Usage: chunklines <value> i.e. \"chunklines on\" , \"chunklines off\"");
                    return;
                }

                switch (args[0].ToLower())
                {
                    case "on":
                        Global.DRAW_WORLD_CHUNK_OUTLINE = true;
                        break;

                    case "off":
                        Global.DRAW_WORLD_CHUNK_OUTLINE = false;
                        break;
                }
            });

            interpreter.RegisterCommand("structure-debug", (args) =>
            {
                if (args.Length != 1)
                {
                    Global.Debug.Console.Output.Append("Usage: structure-debug <value> i.e. \"structure-debug on\" , \"structure-debug off\"");
                    return;
                }

                switch (args[0].ToLower())
                {
                    case "on":
                        Global.DRAW_STRUCTURE_DEBUG = true;
                        break;

                    case "off":
                        Global.DRAW_STRUCTURE_DEBUG = false;
                        break;
                }
            });

            interpreter.RegisterCommand("light", (args) =>
            {
                if (args.Length != 1)
                {
                    Global.Debug.Console.Output.Append("Usage: light <value> i.e. \"light on\" , \"light off\"");
                    return;
                }

                switch (args[0].ToLower())
                {
                    case "on":
                        if (game.Components.Contains(Global.Penumbra) == false)
                        {
                            game.Components.Add(Global.Penumbra);
                        }
                        Global.LIGHTING_ENABLED = true;
                        break;

                    case "off":
                        game.Components.Remove(Global.Penumbra);
                        Global.LIGHTING_ENABLED = false;
                        break;
                }
            });

            interpreter.RegisterCommand("scroll", (args) =>
            {
                if (args.Length < 1)
                {
                    Global.Debug.Console.Output.Append("Usage: \r\n" +
                        "\t - scroll \"<your_message>\" i.e. scroll \"hello\"" +
                        "\t - scroll hide");
                    return;
                }

                if (args.Length == 1 && args[0] == "hide")
                {
                    Global.ScrollDisplay.HideScroll();
                    return;
                }

                var message = string.Join(" ", args);

                ScrollDisplayer.CurrentStructure = new WorldStructure(Vector2.Zero, null, WorldStructureType.Castle)
                {
                    StructureName = message,
                };
                Global.ScrollDisplay.DisplayScroll();
            });

            interpreter.RegisterCommand("speed", (args) =>
            {
                if (args.Length < 1)
                {
                    Global.Debug.Console.Output.Append("Usage: \r\n" +
                        "\t - speed <value> i.e. speed 1000");
                    return;
                }

                var success = int.TryParse(args[0].ToString(), out int value);
                if (success)
                {
                    Global.PlayerObj.MovementSpeed = value;
                }
                else
                {
                    Global.Debug.Console.Output.Append($"Error parsing speed value \"{args[0].ToString()}\"");
                }
            });
        }
    }
}
