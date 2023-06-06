#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match implicitly implemented member.

namespace Chess
{
    public class DragDrop
    {
        private readonly Form form;
        private readonly Fen fenFunctions = new();
        private readonly PGN pgn = new();
        private Image? pieceImg;
        private PictureBox oldPieceSquare;
        private string oldMove;
        private string newMove;
        private char piece;
        private bool end = false;
        private int currentMove = 1;
        private int currentFmr = 0;

        public DragDrop(Form form)
        {
            this.form = form;
        }
        public new void MouseEnter(object sender, EventArgs e)
        {
            PictureBox currentSquare = (PictureBox)sender;

            if (pieceImg == null)
                return;

            newMove = currentSquare.Name[..2];
            Move move = new(oldMove, newMove, form.fen, piece);
            bool pieceColor = char.IsUpper(piece);
            bool playerColor = form.fen[form.fen.IndexOf(' ') + 1] == 'w';

            if (!move.Check() || playerColor != pieceColor)
            {
                oldPieceSquare.Image = pieceImg;
                oldPieceSquare.Name += piece;
                pieceImg = null;
                return;
            }

            pieceImg = null;
            form.fen = fenFunctions.Update(move.GetFen(), oldMove, newMove, piece);
            pgn.SaveMoves(oldMove, currentSquare.Name, piece, currentMove);
            form.fen = SwitchColor();
            form.fen = Fullmoves();
            form.fen = FiftyMoveRule(currentSquare);
            fenFunctions.Draw(form, form.fen);
            end = move.End(oldMove, newMove, form.fen, piece)[0];

            if (end)
            {
                foreach (var squares in form.squares)
                {
                    squares.MouseEnter -= new EventHandler(MouseEnter);
                    squares.MouseDown -= new MouseEventHandler(MouseDown);
                    squares.MouseUp -= new MouseEventHandler(MouseUp);
                }

                Button savePgnBtn = new()
                {
                    Text = "Copy PGN",
                    ForeColor = Color.White,
                    Size = new Size(150, 50),
                    BackColor = Color.RosyBrown,
                    Location = new Point(820, 600)
                };

                form.Controls.Add(savePgnBtn);
                Clipboard.SetDataObject(savePgnBtn);
                Clipboard.SetText(pgn.GetPGN());
            }
        }
        public new void MouseDown(object sender, EventArgs e)
        {
            PictureBox currentSquare = (PictureBox)sender;
            oldPieceSquare = currentSquare;

            if (currentSquare.Image != null)
            {
                piece = currentSquare.Name.Last();
                oldMove = currentSquare.Name[..2];
                currentSquare.Name = currentSquare.Name[..2];
                Bitmap bmp = (Bitmap)currentSquare.Image;
                form.Cursor = new Cursor(bmp.GetHicon());
                pieceImg = currentSquare.Image;
                currentSquare.Image = null;
            }
        }
        public new void MouseUp(object sender, EventArgs e)
        {
            PictureBox currentSquare = (PictureBox)sender;

            if (currentSquare.ClientRectangle.Contains(currentSquare.PointToClient(Control.MousePosition)))
            {
                currentSquare.Image = pieceImg;
                currentSquare.Name += piece;
                pieceImg = null;
            }

            form.Cursor = Cursors.Default;
        }
        private string SwitchColor()
        {
            if (form.fen.Contains("w "))
                form.fen = form.fen.Replace("w ", "b ");
            else
                form.fen = form.fen.Replace("b ", "w ");

            return form.fen;
        }
        private string FiftyMoveRule(PictureBox newSquare)
        {
            string fenInfo = form.fen[(form.fen.IndexOf(' ') + 1)..];
            string[] fenInfoSplit = fenInfo.Split(' ');

            if (char.ToLower(piece) == 'p' || newSquare.Name.Length == 3)
            {
                currentFmr = 0;
                return form.fen[..(form.fen.IndexOf(' ') + 1)] + fenInfo.Replace(fenInfoSplit[3] + " ", "0 ");
            }

            return form.fen[..(form.fen.IndexOf(' ') + 1)] + fenInfo.Replace(fenInfoSplit[3], (++currentFmr).ToString());
        }
        private string Fullmoves()
        {
            currentMove++;

            if (currentMove % 2 != 0)
                return form.fen;

            string[] currentMoves = form.fen.Split(' ');
            return form.fen.Replace(" " + currentMoves[^1].ToString(), " " + (currentMove / 2).ToString());
        }
    }
}