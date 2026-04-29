using System.Text;

namespace GenerationPathsFromSquares
{
  public class GeneratorPathNew
  {
    public (int, int) StartPositionGenerate { get; set; }
    public (int, int) EndPositionGenerate { get; set; }
    /// <summary>
    /// Матрица всего поля
    /// </summary>
    public int[,] squares;
    /// <summary>
    /// Путь по полю(матрица)
    /// </summary>
    public List<Cell> visitedCells = []; // 4
    private readonly Random rnd = new();
    public readonly HashSet<(int, int)> forbiddenCells = [];

    public GeneratorPathNew(int startX,
                   int startY,
                   int endX,
                   int endY,
                   int maxSquaresX,
                   int maxSquaresY)
    {
      #region Проверка данных
      if (maxSquaresX <= 1)
      {
        throw new ArgumentException($"{nameof(maxSquaresX)} не может быть меньше 2");
      }
      if (maxSquaresY <= 1)
      {
        throw new ArgumentException($"{nameof(maxSquaresY)} не может быть меньше 2");
      }
      if (startX < 1 || startX >= maxSquaresX)
      {
        throw new ArgumentException($"{nameof(startX)} не может быть меньше 1 или больше maxSquaresX");
      }
      if (startY < 1 || startY >= maxSquaresY)
      {
        throw new ArgumentException($"{nameof(startY)} не может быть меньше 1 или больше maxSquaresY");
      }
      if (endX < 1 || endX >= maxSquaresX)
      {
        throw new ArgumentException($"{nameof(endX)} не может быть меньше 1 или больше maxSquaresX");
      }
      if (endY < 1 || endY >= maxSquaresY)
      {
        throw new ArgumentException($"{nameof(endY)} не может быть меньше 1 или больше maxSquaresY");
      }
      #endregion
      StartPositionGenerate = (startX - 1, startY - 1);
      EndPositionGenerate = (endX - 1, endY - 1);
      squares = new int[maxSquaresX - 1, maxSquaresY - 1];
      MarkStartAndFinish();
    }

    private void MarkStartAndFinish()
    {
      squares[StartPositionGenerate.Item1, StartPositionGenerate.Item2] = (int)Cells.StartCells;
      squares[EndPositionGenerate.Item1, EndPositionGenerate.Item2] = (int)Cells.EndCells;
    }

    /// <summary>
    /// Генерирует путь <see cref="visitedCells"/> и поле лабиринта <see cref="squares"/>; Модифицирует <see cref="forbiddenCells"/>
    /// </summary>
    public void GenerateRoad()
    {
      (int X, int Y) currentCell = StartPositionGenerate;

      while (currentCell != EndPositionGenerate)
      {
        #region Обработка посещенных ячеек и пути
        // Если текущая ячейка последняя посещенная
        if (visitedCells.Count > 0
          && visitedCells.Last().X == currentCell.X
          && visitedCells.Last().Y == currentCell.Y)
        {
          // запрет ячейки и удаление последней ячейки в пути, переход к предпоследней
          forbiddenCells.Add(currentCell);
          squares[currentCell.X, currentCell.Y] = 0;
          visitedCells.RemoveAt(visitedCells.Count - 1);
          currentCell = (visitedCells.Last().X, visitedCells.Last().Y);
        }
        else
        {
          visitedCells.Add(new Cell(currentCell.X, currentCell.Y, EndPositionGenerate) { cellType = squares[currentCell.X, currentCell.Y] });
        }
        #endregion

        currentCell = GenerateNextCell(currentCell);
        if (currentCell == EndPositionGenerate)
        {
          break;
        }

        // Устанавливаем тип ячейки
        squares[currentCell.X, currentCell.Y] = rnd.Next(3, 7 + 1);
        if (rnd.Next(0, 3 + 1) == 1
          || squares[currentCell.X, currentCell.Y] == 7
          || squares[currentCell.X, currentCell.Y] == 6)
        {
          squares[currentCell.X, currentCell.Y] = 4;
        }

        // Включение анимации
        Console.SetCursorPosition(0, 0);
        Console.WriteLine(ToString(1));
      }

      // Добавление ячейки финиша
      //visitedCells.Add(new Cell(EndPositionGenerate.Item1, EndPositionGenerate.Item2, EndPositionGenerate) { cellType = (int)Cells.EndCells });
    }

    private (int, int) GenerateNextCell((int X, int Y) currentCell)
    {
      List<Cell> cells = [];

      for (int i = -1; i <= 1; i++)
      {
        for (int j = -1; j <= 1; j++)
        {
          #region Главные проверки

          // проверка на смежные по диагонали ячейки (-4 ячейки по диагонали от текущей)
          if (Math.Abs(i) == Math.Abs(j))
          { continue; }

          (int X, int Y) newCell = (currentCell.X + i, currentCell.Y + j);

          // проверка на текущую ячейки (-1 текущая ячейка)
          if (currentCell == newCell)
          { continue; }

          // проверка границ лабиринта
          if (newCell.X < 0 || newCell.Y < 0 || newCell.X >= squares.GetLength(0) || newCell.Y >= squares.GetLength(1))
          { continue; }

          // проверка ячейки на доступность
          if (forbiddenCells.Contains(newCell))
          { continue; }

          // проверка ячейки на посещенную
          if (IsInVisitedCells(newCell.X, newCell.Y))
          { continue; }
          #endregion

          Cell cell = new(newCell.X, newCell.Y, EndPositionGenerate, GetCountFreeNeighbors(newCell.X, newCell.Y));
          if (cell.FreeNeighbors > 0)
          {
            cells.Add(cell);
            if ((cell.X, cell.Y) == EndPositionGenerate)
            {
              return EndPositionGenerate;
            }
          }
        }
      }

      Cell nextCell = new(currentCell.X, currentCell.Y, (0, 0));

      // Полный рандом (не отключайте Управляемый рандом)
      //cells.ForEach(cell => cell.TotalDistanceToFinish = 0);
      //cells.ForEach(cell => cell.FreeNeighbors = 0);

      // Управляемый рандом

      //cells.Sort((a, b) => ((a.TotalDistanceToFinish + a.FreeNeighbors) - (b.TotalDistanceToFinish + b.FreeNeighbors)));
      //cells.Sort((a, b) => ((a.TotalDistanceToFinish) - (b.TotalDistanceToFinish)));

      cells.Sort((a, b) => (a.TotalDistanceToFinish + a.FreeNeighbors)
        .CompareTo(b.TotalDistanceToFinish + b.FreeNeighbors));

      int nextCellMaxPoints = int.MaxValue;

      int randomPointCell;
      // TODO: можно рассмотреть другие виды
      //int randomIndex = rnd.Next(cells.Count);
      //if (cells.Count > 0)
      //{
      //  nextCell = cells[randomIndex];
      //}
      //if (cells.Count == 3)
      //{
      //  nextCell = cells[2];
      //}

      for (int index = 0; index < cells.Count; index++)
      {
        randomPointCell = rnd.Next(0, index + 2); // генерируем случайное значение (от 0 до index + 1)
        if (randomPointCell <= nextCellMaxPoints) // обновляем выбранную клетку
        {
          nextCellMaxPoints = randomPointCell;
          nextCell = cells[index];
        }
      }

      //int nextCellMaxPoints = int.MaxValue;
      //for (int index = 0; index < cells.Count; index++)
      //{
      //  int randomPointCell = 0;
      //  for (int point = 1; point <= index; point++)
      //  {
      //    randomPointCell += rnd.Next(0, 1 + 1);
      //
      //  }
      //  if (randomPointCell <= nextCellMaxPoints)
      //  {
      //    nextCellMaxPoints = randomPointCell;
      //    nextCell = cells[index];
      //
      //    // Простой генератор (УПРАВЛЯЕМЫЙ)
      //    /*int nextRND = rnd.Next(0, 1 + 10);
      //    if (cells.Count >= 4 && nextRND == 10)
      //    {
      //        nextCell = cells[cells.Count - 4];
      //    }
      //    else if (cells.Count >= 3 && nextRND >= 8)
      //    {
      //        nextCell = cells[cells.Count - 3];
      //    }
      //    else if (cells.Count >= 2 && nextRND >= 5)
      //    {
      //        nextCell = cells[cells.Count - 2];
      //    }
      //    else
      //    {
      //        nextCell = cells[cells.Count - 1];
      //    }*/
      //
      //  }
      //}

      // Оптимизация Т-образных развилок(выбирает ячейку путь от которой до финиша более короткий)
      if (cells.Count == 2 && Math.Abs(cells[0].TotalDistanceToFinish - cells[1].TotalDistanceToFinish) == 2
          && (cells[0].X == cells[1].X || cells[0].Y == cells[1].Y))
      {
        nextCell = cells.Aggregate((currentMin, cell) =>
          cell.TotalDistanceToFinish < currentMin.TotalDistanceToFinish ? cell : currentMin);
      }

      return (nextCell.X, nextCell.Y);
    }

    private int GetCountFreeNeighbors(int x, int y)
    {
      int countFreeNeighbors = 0;
      (int, int)[] directions = [(-1, 0), (1, 0), (0, -1), (0, 1)]; // Верх, низ, лево, право

      foreach (var (dx, dy) in directions)
      {
        if (!IsInVisitedCells(x + dx, y + dy))
          countFreeNeighbors++;
      }

      return countFreeNeighbors;


      //int countFreeNeighbors = 4;
      //for (int i = -1; i <= 1; i++)
      //{
      //  for (int j = -1; j <= 1; j++)
      //  {
      //    if (IsInVisitedCells(x, y))
      //    {
      //      countFreeNeighbors--;
      //    }
      //  }

      //}
      //return countFreeNeighbors;
    }

    private bool IsInVisitedCells(int cellX, int cellY)
    {
      if (cellX < 0 || cellY < 0 || cellX >= squares.GetLength(0) || cellY >= squares.GetLength(1))
      { return true; } // Ячейка за границами — считается посещенной

      return visitedCells.Any(visitedCell => visitedCell.X == cellX && visitedCell.Y == cellY);
    }

    public void ProcessPath()
    {
      SetCellRotation();
    }

    private void SetCellRotation()
    {
      for (int i = 0; i < visitedCells.Count; i++)
      {
        if (i == visitedCells.Count - 1)
        {
          visitedCells[i].inputRotate = visitedCells[i - 1].outputRotate;
          visitedCells[i].outputRotate = visitedCells[i - 1].inputRotate;
          break;
        }
        if (visitedCells[i].X == visitedCells[i + 1].X)
        {
          if (visitedCells[i].Y - visitedCells[i + 1].Y > 0)
          {
            visitedCells[i + 1].inputRotate = 180;
            visitedCells[i].outputRotate = 0;
            continue;
          }
          else
          {
            visitedCells[i + 1].inputRotate = 0;
            visitedCells[i].outputRotate = 180;
            continue;
          }

        }
        else
        {
          if (visitedCells[i].X - visitedCells[i + 1].X > 0)
          {
            visitedCells[i + 1].inputRotate = 90;
            visitedCells[i].outputRotate = 270;
            continue;
          }
          else
          {
            visitedCells[i + 1].inputRotate = 270;
            visitedCells[i].outputRotate = 90;
            continue;
          }
        }
      }
    }

    /// <summary>
    /// Вывод лабиринта с цифрами типов клеток
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      StringBuilder output = new();
      for (int i = 0; i <= squares.GetLength(0) - 1; i++)
      {
        for (int j = 0; j <= squares.GetLength(1) - 1; j++)
        {
          output.Append(' ');
          output.Append(squares[i, j]);
          output.Append(' ');
        }
        output.Append('\n');
      }
      return output.ToString();
    }

    /// <summary>
    /// Вывод лабиринта по шаблону
    /// </summary>
    /// <param name="mode">0 - без маркировки запрещенных ячеек; 1 - с маркировкой;</param>
    /// <returns></returns>
    public string ToString(int mode)
    {
      StringBuilder output = new();
      if (mode == 0)
      {
        for (int i = 0; i <= squares.GetLength(0) - 1; i++)
        {
          for (int j = 0; j <= squares.GetLength(1) - 1; j++)
          {
            output.Append(' ');
            if (squares[i, j] == (int)Cells.StartCells)
            {
              output.Append('-');
              continue;
            }

            if (squares[i, j] == (int)Cells.EndCells)
            {
              output.Append('+');
              continue;
            }
            if (squares[i, j] != 0)
            { output.Append('■'); continue; }
            output.Append(' ');
          }
          output.Append('\n');
        }
      }
      if (mode == 1)
      {
        for (int i = 0; i <= squares.GetLength(0) - 1; i++)
        {
          for (int j = 0; j <= squares.GetLength(1) - 1; j++)
          {
            output.Append(' ');

            if (squares[i, j] == (int)Cells.StartCells)
            {
              output.Append('-');
              continue;
            }

            if (squares[i, j] == (int)Cells.EndCells)
            {
              output.Append('+');
              continue;
            }

            if (forbiddenCells.Contains((i, j)))
            {
              output.Append('□');
              continue;
            }

            if (squares[i, j] != 0)
            { output.Append('■'); continue; }
            output.Append(' ');
          }
          output.Append('\n');
        }
      }
      return output.ToString();
    }
  }
}
