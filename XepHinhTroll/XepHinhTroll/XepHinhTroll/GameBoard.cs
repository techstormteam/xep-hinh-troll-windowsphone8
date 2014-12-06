using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovingPicture
{
    public class GameBoard
    {
        public const int MAX_COLUMN_NUMBER = 100;
        public const int MAX_ROW_NUMBER = 100;

        private bool wonGame;

        private int columnNumber;
        private int rowNumber;
        private int[,] pictureMatrix = new int[MAX_COLUMN_NUMBER, MAX_ROW_NUMBER];
        private Position blankPosition;

        private const int DEFAULT_COLUMN_NUMBER = 3;
        private const int DEFAULT_ROW_NUMBER = 4;

        private const int ELEMENT_NUMBER_ORIENTED = 4;
        private int[,] orientation = { { -1, 0 }, { 0, +1 }, { +1, 0 }, { 0, -1 } };

        public GameBoard(int columnNumber, int rowNumber)
        {
            wonGame = false;
            this.columnNumber = columnNumber;
            this.rowNumber = rowNumber;
            initialMatrix();
        }

        public void RandomizeFragments(int times)
        {
            Position previousPosition = new Position()
            {
                X = blankPosition.X,
                Y = blankPosition.Y
            };

            Random random = new Random();
            Position position = new Position();

            for (int i = 0; i < times; i++)
            {
                int j = random.Next(ELEMENT_NUMBER_ORIENTED);
                position.X = blankPosition.X + orientation[j, 0];
                position.Y = blankPosition.Y + orientation[j, 1];
                if (previousPosition.X != position.X || previousPosition.Y != position.Y)
                {
                    // Save pre-step to ensure that don't go back.
                    previousPosition.X = blankPosition.X;
                    previousPosition.Y = blankPosition.Y;
                    MoveAt(position.X, position.Y);
                }
            }
            
        }

        private void verifyWinning()
        {
            wonGame = true;
            if (blankPosition.X == 0 && blankPosition.Y == rowNumber)
            {
                int countdown = rowNumber * columnNumber;
                for (int i = 0; i < rowNumber; i++)
                {
                    for (int j = 0; j < columnNumber; j++)
                    {
                        if (countdown-- != pictureMatrix[i, j])
                        {
                            wonGame = false;
                        }
                    }
                }
            }
        }

        public bool MoveAt(int x, int y)
        {
            // Special case: (x, y) is at additional blank (at the beginning).
            if (x == 0 && y == rowNumber)
            {
                if (checkAndMove(x, y))
                {
                    verifyWinning();
                }
                return true;
            }

            if (x >= columnNumber || x < 0)
            {
                return false;
            }

            if (y >= rowNumber || y < 0)
            {
                return false;
            }

            return checkAndMove(x, y);
        }

        private bool checkAndMove(int x, int y)
        {
            if (nextToBlankCell(x, y))
            {
                pictureMatrix[blankPosition.Y, blankPosition.X] = pictureMatrix[y, x];
                pictureMatrix[y, x] = 0;
                blankPosition.X = x;
                blankPosition.Y = y;
                return true;
            }
            return false;
        }

        private bool nextToBlankCell(int x, int y)
        {
            for (int i = 0; i < ELEMENT_NUMBER_ORIENTED; i++)
            {
                if ((blankPosition.X + orientation[i, 0]) == x
                    && blankPosition.Y + orientation[i, 1] == y)
                {
                    return true;
                }
            }
            return false;
        }

        private void initialMatrix()
        {
            if (columnNumber <= 0)
            {
                columnNumber = DEFAULT_COLUMN_NUMBER;
            }

            if (columnNumber > MAX_COLUMN_NUMBER)
            {
                columnNumber = MAX_COLUMN_NUMBER;
            }

            if (rowNumber <= 0)
            {
                rowNumber = DEFAULT_ROW_NUMBER;
            }

            if (rowNumber > MAX_ROW_NUMBER)
            {
                rowNumber = MAX_ROW_NUMBER;
            }

            if (null == blankPosition)
            {
                blankPosition = new Position()
                {
                    X = 0,
                    Y = rowNumber
                };
            }

            int maxElementNumber = columnNumber * rowNumber;
            for (int rowIndex = 0; rowIndex < rowNumber; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columnNumber; columnIndex++)
                {
                    pictureMatrix[rowIndex, columnIndex] = maxElementNumber - (columnNumber * rowIndex + columnIndex);
                }
            }
        }

        public bool IsWin
        {
            get { return wonGame; }
        }

        public int TotalElementNumberExceptBlankCell
        {
            get
            {
                return columnNumber * rowNumber;
            }
        }

        public int ColumnNumber
        {
            get
            {
                if (columnNumber == 0)
                {
                    columnNumber = DEFAULT_COLUMN_NUMBER;
                    initialMatrix();
                }
                return columnNumber;
            }
        }

        public int RowNumber
        {
            get
            {
                if (rowNumber == 0)
                {
                    rowNumber = DEFAULT_ROW_NUMBER;
                    initialMatrix();
                }
                return rowNumber;
            }
        }

        public int BlankX
        {
            get
            {
                if (null == blankPosition)
                {
                    blankPosition = new Position()
                    {
                        X = 0,
                        Y = rowNumber
                    };
                }
                return blankPosition.X;
            }
        }

        public int BlankY
        {
            get
            {
                if (null == blankPosition)
                {
                    blankPosition = new Position()
                    {
                        X = 0,
                        Y = rowNumber
                    };
                }
                return blankPosition.Y;
            }
        }

        public int[,] PictureMatrix
        {
            get
            {
                if (pictureMatrix.Length == 0)
                {
                    initialMatrix();
                }
                return pictureMatrix;
            }
        }

    }
}
