using System;
using Tao.Sdl;

namespace MyGame
{
    class Program
    {
        static int[,] grid = new int[7, 6]; // 7 columnas, 6 filas
        static int currentPlayer = 1; // Jugador actual (1 o 2)
        static int selectedColumn = 0; // Columna seleccionada
        static bool keyReleased = true; // Controla que se haya soltado la tecla
        static bool leftKeyReleased = true; // Controla tecla izquierda
        static bool rightKeyReleased = true; // Controla tecla derecha
        static bool gameOver = false; // Controla si el juego ha terminado
        static Image inicio;
        static Image image;
        static Image player1Image;
        static Image player2Image;
        static Image winImage;
        static Image gameOverImage;
        static bool isRunning = true;
        static int estadoJuego = 0;

        static void Main(string[] args)
        {
            try
            {
                Initialize();
                GameLoop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine("Presiona cualquier tecla para cerrar...");
                Console.ReadKey();
            }
        }

        static void Initialize()
        {
            Engine.Initialize();
            estadoJuego = 0;

            try
            {
                inicio = Engine.LoadImage("assets/inicio.jpg");
                image = Engine.LoadImage("assets/fondo4enlinea4.png");
                player1Image = Engine.LoadImage("assets/player1.png");
                player2Image = Engine.LoadImage("assets/player2.png");
                winImage = Engine.LoadImage("assets/win.jpg");
                gameOverImage = Engine.LoadImage("assets/Gameover.png");
            }
            catch (Exception e)
            {
                throw new Exception("Error al cargar las imágenes: " + e.Message);
            }
        }

        static void GameLoop()
        {
            while (isRunning)
            {
                if (!gameOver)
                {
                    CheckInputs();

                    if (currentPlayer == 2 && !gameOver) // Es el turno de la máquina
                    {
                        MachineMove(); // La máquina realiza su movimiento
                    }

                    Render();
                }
                else
                {
                    RenderGameOver();
                    if (Engine.KeyPress(Engine.KEY_ENTER))
                    {
                        ResetGame();
                    }
                }
                Sdl.SDL_Delay(20);
            }
        }

        static void ResetGame()
        {
            grid = new int[7, 6];
            currentPlayer = 1;
            selectedColumn = 0;
            gameOver = false;
        }

        static void CheckInputs()
        {
            if (estadoJuego == 0 && Engine.KeyPress(Engine.KEY_ESP))
            {
                estadoJuego = 1;
            }
            else if (estadoJuego == 2 && Engine.KeyPress(Engine.KEY_ESP) || estadoJuego == 3 && Engine.KeyPress(Engine.KEY_ESP))
            {
                estadoJuego = 1;
            }
            else
            {
                if (!Engine.KeyPress(Engine.KEY_LEFT))
                {
                    leftKeyReleased = true;
                }
                if (Engine.KeyPress(Engine.KEY_LEFT) && leftKeyReleased)
                {
                    selectedColumn = Math.Max(0, selectedColumn - 1);
                    leftKeyReleased = false;
                }

                if (!Engine.KeyPress(Engine.KEY_RIGHT))
                {
                    rightKeyReleased = true;
                }
                if (Engine.KeyPress(Engine.KEY_RIGHT) && rightKeyReleased)
                {
                    selectedColumn = Math.Min(6, selectedColumn + 1);
                    rightKeyReleased = false;
                }

                if (!Engine.KeyPress(Engine.KEY_ENTER))
                {
                    keyReleased = true;
                }
                if (Engine.KeyPress(Engine.KEY_ENTER) && keyReleased)
                {
                    PlacePiece();
                    keyReleased = false;
                }

                if (Engine.KeyPress(Engine.KEY_ESC))
                {
                    isRunning = false;
                }
            }
        }

        static void PlacePiece()
        {
            for (int row = 5; row >= 0; row--)
            {
                if (grid[selectedColumn, row] == 0)
                {
                    grid[selectedColumn, row] = currentPlayer;
                    if (CheckWin(selectedColumn, row))
                    {
                        gameOver = true;
                        return;
                    }
                    currentPlayer = currentPlayer == 1 ? 2 : 1;
                    break;
                }
            }
        }

        static bool CheckWin(int col, int row)
        {
            return CheckHorizontal(row) || CheckVertical(col) || CheckDiagonal() || CheckAntiDiagonal();
        }

        static bool CheckHorizontal(int row)
        {
            for (int col = 0; col <= 3; col++)
            {
                if (grid[col, row] != 0 &&
                    grid[col, row] == grid[col + 1, row] &&
                    grid[col, row] == grid[col + 2, row] &&
                    grid[col, row] == grid[col + 3, row])
                {
                    return true;
                }
            }
            return false;
        }

        static bool CheckVertical(int col)
        {
            for (int row = 0; row <= 2; row++)
            {
                if (grid[col, row] != 0 &&
                    grid[col, row] == grid[col, row + 1] &&
                    grid[col, row] == grid[col, row + 2] &&
                    grid[col, row] == grid[col, row + 3])
                {
                    return true;
                }
            }
            return false;
        }

        static bool CheckDiagonal()
        {
            for (int col = 0; col <= 3; col++)
            {
                for (int row = 0; row <= 2; row++)
                {
                    if (grid[col, row] != 0 &&
                        grid[col, row] == grid[col + 1, row + 1] &&
                        grid[col, row] == grid[col + 2, row + 2] &&
                        grid[col, row] == grid[col + 3, row + 3])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static bool CheckAntiDiagonal()
        {
            for (int col = 0; col <= 3; col++)
            {
                for (int row = 3; row < 6; row++)
                {
                    if (grid[col, row] != 0 &&
                        grid[col, row] == grid[col + 1, row - 1] &&
                        grid[col, row] == grid[col + 2, row - 2] &&
                        grid[col, row] == grid[col + 3, row - 3])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static void Render()
        {
            Engine.Clear();

            switch (estadoJuego)
            {
                case 0: // Renderizar pantalla de inicio
                    Engine.Draw(inicio, 0, 0);
                    break;

                case 1: // Renderizar tablero de juego
                    Engine.Draw(image, 0, 0);

                    // Dibuja las fichas en el tablero
                    for (int col = 0; col < 7; col++)
                    {
                        for (int row = 0; row < 6; row++)
                        {
                            if (grid[col, row] == 1)
                            {
                                Engine.Draw(player1Image, col * 100 , row * 100);
                            }
                            else if (grid[col, row] == 2)
                            {
                                Engine.Draw(player2Image, col * 100, row * 100);
                            }
                        }
                    }
                    break;
                case 2: // Renderizar pantalla de victoria
                    Engine.Draw(winImage, 0, 0);
                    break;
                case 3: // Renderizar pantalla de derrota
                    Engine.Draw(gameOverImage, 0, 0);
                    break;
            }

            Engine.Show();
        }

        static void RenderGameOver()
        {
            Engine.Clear();
            if (currentPlayer == 2)
            {
                estadoJuego = 3;
            }
            else
            {
                estadoJuego = 2;
            }
            Engine.Show();
        }

        static void MachineMove()
        {
            // La máquina selecciona una columna aleatoria disponible
            Random rand = new Random();
            int column = rand.Next(0, 7); // Selecciona una columna aleatoria entre 0 y 6
            while (IsColumnFull(column)) // Si la columna está llena, selecciona otra
            {
                column = rand.Next(0, 7);
            }

            // Realiza el movimiento de la máquina (coloca la ficha)
            for (int row = 5; row >= 0; row--)
            {
                if (grid[column, row] == 0)
                {
                    grid[column, row] = currentPlayer;
                    if (CheckWin(column, row)) // Verifica si la máquina ha ganado
                    {
                        gameOver = true;
                        return;
                    }
                    currentPlayer = 1; // El turno vuelve al jugador 1
                    break;
                }
            }

            // Agregar un pequeño retraso para simular "pensar"
            System.Threading.Thread.Sleep(500); // Espera 500 milisegundos antes de la siguiente jugada
        }

        static bool IsColumnFull(int column)
        {
            for (int row = 0; row < 6; row++)
            {
                if (grid[column, row] == 0) // Si hay un espacio vacío en la columna, no está llena
                {
                    return false;
                }
            }
            return true; // Si la columna está llena
        }
    }
}
