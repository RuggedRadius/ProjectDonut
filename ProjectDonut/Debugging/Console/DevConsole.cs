using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDonut.Core;
using ProjectDonut.Interfaces;
using ProjectDonut.ProceduralGeneration.World.Structures;
using ProjectDonut.UI.DialogueSystem;
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
            interpreter.RegisterCommand("msg", (args) =>
            {
                if (args.Length < 1)
                {
                    Global.Debug.Console.Output.Append("Usage: \r\n" +
                        "\t - msg <your_message> i.e. scroll hello");
                    return;
                }

                var width = 22;
                var height = 3;
                var rect = new Rectangle(
                    (-(width * Global.TileSize) / 2) + (Global.GraphicsDevice.Viewport.Width / 2), 
                    (-(height * Global.TileSize) / 2) + (Global.GraphicsDevice.Viewport.Height / 2), 
                    width, 
                    height);

                var message = string.Join(" ", args);
                var lines = new List<Dialogue>() { Global.DialogueManager.CreateDialogue(rect, message, 3000f) };

                Global.DialogueManager.ExecuteMultipleLines(lines);
            });

            interpreter.RegisterCommand("grid", (args) =>
            {
                if (args.Length != 1)
                {
                    Global.Debug.Console.Output.Append("Usage: grid <value> i.e. \"grid on\" , \"grid off\"");
                    return;
                }

                switch (args[0].ToLower())
                {
                    case "on":
                        Global.SHOW_GRID_OUTLINE = true;
                        break;

                    case "off":
                        Global.SHOW_GRID_OUTLINE = false;
                        break;
                }
            });

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
                    Global.ScrollDisplay.ClearAllScrolls();
                    return;
                }

                var scroll = new ScrollDisplay()
                {
                    Text = string.Join(" ", args),
                    SubText = "Console Message",
                    IsTimed = true,
                    ShowDuration = 5f,
                    ScrollOutDuration = 1f
                };
                Global.ScrollDisplay.DisplayScroll(scroll);
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
