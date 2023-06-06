namespace Chess
{
    public class Pieces
    {
        private readonly Check check;
        private readonly King king;
        private readonly int oldMoveLetter;
        private readonly int newSquareLetter;
        private readonly int oldMoveNumber;
        private readonly int newMoveNumber;
        private readonly int fenLength;
        private readonly bool playerColor;
        private string fen;

        public Pieces(string oldMove, string newMove, string fen, char piece)
        {
            oldMoveLetter = int.Parse(oldMove[..1]);
            newSquareLetter = int.Parse(newMove[..1]);
            oldMoveNumber = int.Parse(oldMove.Substring(1, 1));
            newMoveNumber = int.Parse(newMove.Substring(1, 1));
            this.fen = fen;
            fenLength = fen.IndexOf(' ');
            playerColor = fen[fenLength + 1] == 'w';
            check = new(oldMove, newMove, fen);
            king = new(oldMove, newMove, fen, piece);
        }
        public string GetFen()
        {
            return fen;
        }
        public bool King()
        {
            if (king.Castling())
            {
                fen = king.GetFen();
                return true;
            }

            if (!king.KingRange() || king.OppKing())
                return false;

            if (check.DiagMoves() || check.HorVerMoves())
            {
                fen = king.DisallowCastling(fen);
                return true;
            }

            return false;
        }
        public bool Queen()
        {
            if (check.DiagMoves() || check.HorVerMoves())
                return true;

            return false;
        }
        public bool Rook()
        {
            if (check.HorVerMoves())
            {
                string dLC = fen[fenLength..];
                string queenSymbol = playerColor ? "Q" : "q";
                string kingSymbol = playerColor ? "K" : "k";
                string wichRook = oldMoveLetter == 0 ? queenSymbol : kingSymbol;
                dLC = dLC.Replace(wichRook, "");
                fen = fen[..fenLength] + dLC;
                return true;
            }

            return false;
        }
        public bool Bishop()
        {
            if (check.DiagMoves())
                return true;

            return false;
        }
        public bool Knight()
        {
            int rowDistance = Math.Abs(newMoveNumber - oldMoveNumber);
            int colDistance = Math.Abs(newSquareLetter - oldMoveLetter);

            if ((rowDistance == 2 && colDistance == 1) || (rowDistance == 1 && colDistance == 2))
                return true;

            return false;
        }
    }
}