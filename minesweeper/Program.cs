using Raylib_cs;

Raylib.InitWindow(600, 600, "Minesweeper");
Raylib.SetTargetFPS(60);

int menu = 0;
int mineCount = 0;
int flagCount = 0;
Texture2D flag = Raylib.LoadTexture("flag.png");

bool win = false;

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
            Raylib.DrawText($"{(i + 1) * 16}", (int)buttons[i].x, (int)buttons[i].y + 8, 24, Color.BLACK);
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
                    mineCount = 40 * (i * 2 + 1);
                    flagCount = mineCount;
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
                    Raylib.DrawRectangle(i * Raylib.GetScreenWidth() / grid.GetLength(0), j * Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0), Color.GRAY);
                    if (grid[i, j].flag == true)
                    {
                        Raylib.DrawTextureEx(flag, new(i * Raylib.GetScreenWidth() / grid.GetLength(0), j * Raylib.GetScreenWidth() / grid.GetLength(0)), 0, (Raylib.GetScreenWidth() / grid.GetLength(0)) / 8, Color.GRAY);
                    }
                }
                Raylib.DrawRectangleLines(i * Raylib.GetScreenWidth() / grid.GetLength(0), j * Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0), Color.DARKGRAY);

                if (!win)
                {
                    if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(i * Raylib.GetScreenWidth() / grid.GetLength(0), j * Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0), Raylib.GetScreenWidth() / grid.GetLength(0))))
                    {
                        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                        {
                            if (!grid[i, j].flag)
                            {
                                if (grid[i, j].val == 9)
                                {
                                    menu = 0;
                                }
                                else if (grid[i, j].val == 0)
                                {
                                    fill(i, j);
                                }
                                else
                                {
                                    grid[i, j].shown = true;
                                }
                            }
                            CheckWin();
                        }
                        else if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON))
                        {
                            if (grid[i, j].flag)
                            {
                                flagCount++;
                                grid[i, j].flag = !grid[i, j].flag;
                            }
                            else if (flagCount > 0)
                            {
                                flagCount--;
                                grid[i, j].flag = !grid[i, j].flag;
                            }
                        }
                    }
                }
            }
        }
        if (win)
        {
            Raylib.DrawText("YOU WIN!", Raylib.GetScreenWidth() / 2 - 112, Raylib.GetScreenHeight() / 2 - 48, 48, Color.BLACK);
        }
    }
    Raylib.EndDrawing();
}

void CheckWin()
{
    int count = 0;
    for (int x = 0; x < grid.GetLength(0); x++)
    {
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            if (!grid[x, y].shown) { count++; }
            if (count > mineCount) { break; }
        }
    }
    if (count == mineCount)
    {
        win = true;
    }
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
        while (grid[x, y].val == 9)
        {
            x = random.Next(0, grid.GetLength(0));
            y = random.Next(0, grid.GetLength(1));
        }
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
    }
}

class Tile
{
    public int val = 0;
    public bool shown = false;
    public bool flag = false;
}