namespace Chess
{
    public class Board
    {
        public List<PictureBox> squares = new();
        private readonly Form form;
        private readonly DragDrop dragDrop;

        public Board(Form form)
        {
            this.form = form;
            dragDrop = new DragDrop(form);
        }
        public void CreateBoard()
        {
            const int squareSize = 95;
            const int boardSize = squareSize * 8;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    PictureBox square = new()
                    {
                        Name = $"{row}{7 - col}",
                        Size = new Size(squareSize, squareSize),
                        Location = new Point(row * squareSize, col * squareSize),
                    };

                    if ((row + col) % 2 == 0)
                    {
                        square.BackColor = Color.White;
                    }
                    else
                    {
                        square.BackColor = Color.RosyBrown;
                    }

                    squares.Add(square);
                    form.Controls.Add(square);

                    square.MouseEnter += new EventHandler(dragDrop.MouseEnter);
                    square.MouseDown += new MouseEventHandler(dragDrop.MouseDown);
                    square.MouseUp += new MouseEventHandler(dragDrop.MouseUp);
                }
            }

            form.ClientSize = new Size(boardSize + 300, boardSize);
        }
    }
}