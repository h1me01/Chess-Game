using System.Diagnostics;

namespace Chess
{
    public class End
    {
        private readonly King king;
        private readonly int fenLength;
        private readonly bool playerColor;
        private readonly string fen;

        public End(string oldMove, string newMove, string fen, char piece)
        {
            king = new(oldMove, newMove, fen, piece);
            this.fen = fen;
            fenLength = fen.IndexOf(' ');
            playerColor = fen[fenLength + 1] == 'w';
        }
        private List<string> GetSquares()
        {
            List<string> squares = new();

            for (int file = 0; file <= 7; file++)
            {
                for (int rank = 0; rank <= 7; rank++)
                    squares.Add($"{file}{rank}");
            }

            return squares;
        }
        private List<string> GetPiecesPos()
        {
            List<string> getAllPieces = new();
            int currentRank = 7;
            int letterPos = -1;

            for (int i = 0; i < fenLength; i++)
            {
                char c = fen[i];
                letterPos += int.TryParse(fen.AsSpan(i, 1), out int n) ? n : 1;
                bool isDigit = char.IsDigit(c);
                bool isWhite = char.IsUpper(c);

                if (c == '/')
                {
                    currentRank--;
                    letterPos = -1;
                }
                else if (playerColor && isWhite && !isDigit)
                    getAllPieces.Add(c.ToString() + letterPos + currentRank.ToString());
                else if (!playerColor && !isWhite && !isDigit)
                    getAllPieces.Add(c.ToString() + letterPos + currentRank.ToString());
            }

            return getAllPieces;
        }
        public bool Moves()
        {
            List<string> getAllMoves = GetSquares();
            List<string> getAllPieces = GetPiecesPos();

            foreach (var item in getAllPieces)
            {
                foreach (var item1 in getAllMoves)
                {
                    char piece = item[0];
                    string oldMove = item.ToLower()[1..];
                    string newMove = item1;
                    Move move = new(oldMove, newMove, fen, piece);

                    if (move.Check())
                        return true;
                }
            }

            return false;
        }
        public bool Checkmate()
        {
            return !Moves() && king.Checks();
        }
        public bool Draw()
        {
            string tempFen = fen.ToLower();
            return !tempFen.Any(c => "qrbnp".Contains(c));
        }
        public bool Stalemate()
        {
            return !Moves() && !king.Checks();
        }
    }
}