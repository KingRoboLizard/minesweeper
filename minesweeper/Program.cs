using Raylib_cs;

Raylib.InitWindow(600, 600, "Minesweeper");
Raylib.SetTargetFPS(60);

int menu = 0;
int mineCount = 0;

List<Rectangle> buttons = new();
Tile[,] grid = new Tile[0, 0];
List<Color> colors = new() { Color.GRAY, Color.BLUE, Color.GREEN, Color.RED, Color.DARKBLUE, Color.MAROON, Color.SKYBLUE, Color.BLACK, Color.DARKGRAY, Color.BLACK, Color.RED };
Random random = new();

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.LIGHTGRAY);
    if (menu == 0)
    {
        for (int i = 0; i < 3; i++)
        {
            buttons.Add(new(40 + Raylib.GetScreenWidth() / 3 * i, 100, 50, 50));
        }
        for (int i = 0; i < buttons.Count; i++)
        {
            Raylib.DrawRectangleRec(buttons[i], Color.WHITE);
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), buttons[i]))
                {
                    grid = new Tile[16 * (i + 1), 16 * (i + 1)];
                    for (var j = 0; j < grid.GetLength(0); j++)
                    {
                        for (var k = 0; k < grid.GetLength(1); k++)
                        {
                            grid[j, k] = new();
                        }
                    }
                    mineCount = 80 * (i*2 + 1);
                    menu = 1;
                    GenerateGame();
                    break;
                }
            }
        }
    }
    else if (menu == 1)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j].shown)
                {
                    Raylib.DrawText($"{grid[i, j].val}", i * Raylib.GetScreenWidth() / grid.GetLength(0), j * Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0) / 2, colors[grid[i, j].val]);
                }
                else
                {
                    if (grid[i, j].flag == true)
                    {
                        Raylib.DrawRectangle(i * Raylib.GetScreenWidth() / grid.GetLength(0), j * Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0), Color.RED);
                    }
                    else
                    {
                        Raylib.DrawRectangle(i * Raylib.GetScreenWidth() / grid.GetLength(0), j * Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0), Color.GRAY);
                    }
                }
                Raylib.DrawRectangleLines(i * Raylib.GetScreenWidth() / grid.GetLength(0), j * Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0), Color.DARKGRAY);

                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(i * Raylib.GetScreenWidth() / grid.GetLength(0), j * Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0))))
                {
                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                    {
                        if (grid[i, j].val == 9)
                        {
                            menu = 0;
                        }
                        else if (grid[i, j].val == 0)
                        {
                            for (var k = i - 1; k < i + 2; k++)
                            {
                                for (var l = j - 1; l < j + 2; l++)
                                {
                                    if (k < grid.GetLength(0) && l < grid.GetLength(1) && k >= 0 && l >= 0)
                                    {
                                        if (grid[k, l].val != 9 && grid[k, l].shown == false)
                                        {
                                            fill(k, l);
                                            // if (grid[k, l].val == 0)
                                            // {
                                            //     i = k;
                                            //     j = l;
                                            // }
                                            // grid[k, l].shown = true;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            grid[i, j].shown = true;
                        }
                    }
                    else if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON))
                    {
                        grid[i, j].flag = !grid[i, j].flag;
                    }
                }
            }
        }
    }
    Raylib.EndDrawing();
}

void fill(int x, int y)
{
    if (x < grid.GetLength(0) && y < grid.GetLength(1) && x >= 0 && y >= 0 && !grid[x, y].shown && grid[x, y].val == 0)
    {
        grid[x, y].shown = true;
        fill(x, y + 1);
        fill(x, y - 1);
        fill(x + 1, y);
        fill(x - 1, y);

        for (int i = x - 1; i < x + 2; i++)
        {
            for (int j = y - 1; j < y + 2; j++)
            {
                if (i < grid.GetLength(0) && j < grid.GetLength(1) && i >= 0 && j >= 0 && grid[i, j].val != 9 && grid[i, j].val != 0)
                {
                    grid[i, j].shown = true;
                }
            }
        }
    }
}

void GenerateGame()
{
    int x = random.Next(0, grid.GetLength(0));
    int y = random.Next(0, grid.GetLength(1));

    for (int i = 0; i < mineCount; i++)
    {
        if (grid[x, y].val != 9)
        {
            grid[x, y].val = 9;
            for (int j = x - 1; j < x + 2; j++)
            {
                for (int k = y - 1; k < y + 2; k++)
                {
                    if (j < grid.GetLength(0) && j >= 0 && k < grid.GetLength(1) && k >= 0 && grid[j, k].val != 9)
                    {
                        grid[j, k].val++;
                    }
                }
            }
        }
        else
        {
            x = random.Next(0, grid.GetLength(0));
            y = random.Next(0, grid.GetLength(1));
        }
    }
}

class Tile
{
    public int val;
    public bool shown;
    public bool flag;
    public Tile(int value = 0, bool Shown = false, bool flagged = false)
    {
        val = value;
        shown = Shown;
        flag = flagged;
    }
}