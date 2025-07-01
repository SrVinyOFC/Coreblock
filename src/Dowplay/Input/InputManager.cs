using System;
using System.Numerics;
using Microsoft.Xna.Framework.Input;
using SharpDX.XInput; // Para gamepad (instale o pacote SharpDX.XInput via NuGet)

namespace Coreblock
{
    public static class InputManager
    {
        private static Controller gamepad = new Controller(UserIndex.One);

        public static Vector2 GetDirectionalInput()
        {
            Vector2 direction = new Vector2(0, 0);

            if (IsKeyPressed(Keys.W) || IsGamepadButtonPressed(GamepadButtonFlags.DPadUp))
                direction.Y -= 1;
            if (IsKeyPressed(Keys.S) || IsGamepadButtonPressed(GamepadButtonFlags.DPadDown))
                direction.Y += 1;
            if (IsKeyPressed(Keys.A) || IsGamepadButtonPressed(GamepadButtonFlags.DPadLeft))
                direction.X -= 1;
            if (IsKeyPressed(Keys.D) || IsGamepadButtonPressed(GamepadButtonFlags.DPadRight))
                direction.X += 1;


            return Vector2.Normalize(direction);
        }

        public static bool IsMouseButtonPressed(MouseButton button)
        {
            var mouseState = Mouse.GetState();
            return button switch
            {
                MouseButton.Left => mouseState.LeftButton == ButtonState.Pressed,
                MouseButton.Right => mouseState.RightButton == ButtonState.Pressed,
                MouseButton.Middle => mouseState.MiddleButton == ButtonState.Pressed,
                MouseButton.XButton1 => mouseState.XButton1 == ButtonState.Pressed,
                MouseButton.XButton2 => mouseState.XButton2 == ButtonState.Pressed,
                _ => false,
            };
        }


        public enum MouseButton
        {
            Left,
            Right,
            Middle,
            XButton1,
            XButton2
        }

        public static Vector2 GetMousePosition()
        {
            var mouseState = Mouse.GetState();
            return new Vector2(mouseState.X, mouseState.Y);
        }

        public static Vector2 GetMousePositionWorld()
        {
            var mouseState = Mouse.GetState();
            var monogameVec = Camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y), Globals.TileSize);
            return new Vector2(monogameVec.X, monogameVec.Y);
        }

        public static string ReadString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        public static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int result))
                {
                    return result;
                }
                Console.WriteLine("Invalid input. Please enter a valid integer.");
            }
        }

        public static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out double result))
                {
                    return result;
                }
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }

        public static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt + " (yes/no): ");
                string input = Console.ReadLine()?.Trim().ToLower();
                if (input == "yes" || input == "y")
                {
                    return true;
                }
                if (input == "no" || input == "n")
                {
                    return false;
                }
                Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
            }
        }

        public static bool IsKeyPressed(Keys key)
        {
            var keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(key);
        }

        // Detecta um único pressionamento de tecla (não repete enquanto segurar)
        private static readonly System.Collections.Generic.Dictionary<Keys, bool> previousKeyStates = new System.Collections.Generic.Dictionary<Keys, bool>();
        public static bool IsSingleKeyPress(Keys key)
        {
            var keyboardState = Keyboard.GetState();
            bool isCurrentlyDown = keyboardState.IsKeyDown(key);

            bool wasPreviouslyDown = previousKeyStates.TryGetValue(key, out bool prev) ? prev : false;
            previousKeyStates[key] = isCurrentlyDown;

            // Retorna true só quando a tecla foi pressionada neste frame e não estava pressionada no anterior
            return !wasPreviouslyDown && isCurrentlyDown;
        }

        // Detecta um único clique de botão do mouse (não repete enquanto segurar)
        private static readonly System.Collections.Generic.Dictionary<MouseButton, bool> previousMouseButtonStates = new System.Collections.Generic.Dictionary<MouseButton, bool>();
        public static bool IsSingleMouseButtonPress(MouseButton button)
        {
            var mouseState = Mouse.GetState();

            bool isCurrentlyDown = button switch
            {
            MouseButton.Left => mouseState.LeftButton == ButtonState.Pressed,
            MouseButton.Right => mouseState.RightButton == ButtonState.Pressed,
            MouseButton.Middle => mouseState.MiddleButton == ButtonState.Pressed,
            MouseButton.XButton1 => mouseState.XButton1 == ButtonState.Pressed,
            MouseButton.XButton2 => mouseState.XButton2 == ButtonState.Pressed,
            _ => false,
            };

            bool wasPreviouslyDown = previousMouseButtonStates.TryGetValue(button, out bool prev) ? prev : false;
            previousMouseButtonStates[button] = isCurrentlyDown;

            // Retorna true só quando o botão foi pressionado neste frame e não estava pressionado no anterior
            return !wasPreviouslyDown && isCurrentlyDown;
        }

        public static bool IsGamepadButtonPressed(GamepadButtonFlags button)
        {
            if (gamepad.IsConnected)
            {
                var state = gamepad.GetState();
                return (state.Gamepad.Buttons & button) == button;
            }
            return false;
        }
    }
}