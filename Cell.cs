namespace GenerationPathsFromSquares
{
  public class Cell(int x, int y, (int, int) finish, int freeNeighbors = 4)
  {
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    public int cellType = (int)Cells.None;
    public int inputRotate;
    public int outputRotate;

    public int TotalDistanceToFinish { get; set; } = Math.Abs(finish.Item1 - x) + Math.Abs(finish.Item2 - y);
    public int FreeNeighbors { get; set; } = freeNeighbors;

    public (int, int, int, int) GetCoordinates()
    {
      return (X, Y, inputRotate, outputRotate);
    }
  }

  public enum Cells
  {
    None,
    StartCells,
    EndCells,
    InclinedCells,
    StandardCells,
    MovableCells,
    FallingOffCells,
    JumpingCells
  }
}
