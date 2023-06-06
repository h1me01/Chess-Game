namespace Chess
{
    public class Fen
    {
        private const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        private readonly string path = Directory.GetCurrentDirectory();

        public string GetFen()
        {
            return fen;
        }
        public void Draw(Form form, string fen = fen)
        {
            List<PictureBox> squares = form.squares;
            int fenLength = fen.IndexOf(' ');
            int col = 0;
            int row = 7;

            for (int i = 0; i < fenLength; i++)
            {
                char c = fen[i];
                int isNumber = int.TryParse(c.ToString(), out int n) ? n : 0;
                bool wichPlayer = c == char.ToUpper(c);
                col += isNumber;

                if (c == '/')
                {
                    row--;
                    col = 0;
                }
                else if (isNumber == 0)
                {
                    string piece = (wichPlayer ? "w" + char.ToLower(c) : "b" + char.ToLower(c)) + ".png";
                    int square = (col * 8) + 7 - row;
                    squares[square].Image = null;
                    squares[square].Name = squares[square].Name[..2];
                    squares[square].Image = LoadPiece(piece);
                    squares[square].Name += c;
                    col++;
                }
            }
        }
        public string Update(string fen, string oldMove, string newMove, char piece, bool tangleFen = true)
        {
            fen = UntangleFen(fen);
            Pawn pawn = new(oldMove, newMove, fen);
            int oldMoveLetter = int.Parse(oldMove[..1]);
            int newMoveLetter = int.Parse(newMove[..1]);
            int oldMoveNumber = int.Parse(oldMove.Substring(1, 1));
            int newMoveNumber = int.Parse(newMove.Substring(1, 1));
            int fenLength = fen.IndexOf(' ');
            bool playerColor = fen[fenLength + 1] == 'w';
            int row = 7;
            int col = -1;

            for (int i = 0; i < fenLength; i++)
            {
                char c = fen[i];
                col += int.TryParse(c.ToString(), out int n) ? n : 1;

                if (c == '/')
                {
                    row--;
                    col = -1;
                }

                if (row == oldMoveNumber && col == oldMoveLetter)
                    fen = $"{fen[..i]}1{fen[(i + 1)..]}";

                if (row == newMoveNumber && col == newMoveLetter && !(pawn.Promotion() && char.ToLower(piece) == 'p'))
                    fen = $"{fen[..i]}{(playerColor ? char.ToUpper(piece) : piece)}{fen[(i + 1)..]}";
            }

            if (tangleFen)
                fen = TangleFen(fen);

            fen = pawn.DelEnPassant(playerColor, fen);
            return fen;
        }
        public string TangleFen(string fen)
        {
            int currentNumber = 0;
            int indexStart = 0;

            for (int i = 0; i < fen.Length; i++)
            {
                if (int.TryParse(fen.AsSpan(i, 1), out int n))
                {
                    currentNumber += n;
                    if (currentNumber == n) indexStart = i;
                }
                else
                {
                    if (currentNumber > 0)
                    {
                        fen = fen[..indexStart] + currentNumber + fen[i..];
                        i = indexStart;
                        currentNumber = 0;
                    }
                }
            }

            return fen;
        }
        public string UntangleFen(string fen)
        {
            int fenLength = fen.IndexOf(' ');

            for (int i = 0; i < fenLength; i++)
            {
                if (int.TryParse(fen.AsSpan(i, 1), out int n) && n > 1)
                {
                    string newStr = new('1', n);
                    fen = fen[..i] + newStr + fen[(i + 1)..];
                }

                fenLength = fen.IndexOf(' ');
            }

            return fen;
        }
        private Image LoadPiece(string src)
        {
            string piecePath = Path.Combine(path, "chessPieces", src);
            Bitmap bmp = new(piecePath);
            return bmp;
        }
    }
}