using System.Diagnostics;

namespace Chess
{
    public class Pawn
    {
        private readonly Check check;
        private readonly int oldMoveLetter;
        private readonly int newMoveLetter;
        private readonly int oldMoveNumber;
        private readonly int newMoveNumber;
        private readonly int fenLength;
        private readonly bool playerColor;
        private string fen;

        public Pawn(string oldMove, string newMove, string fen)
        {
            check = new(oldMove, newMove, fen);
            oldMoveLetter = int.Parse(oldMove[..1]);
            newMoveLetter = int.Parse(newMove[..1]);
            oldMoveNumber = int.Parse(oldMove.Substring(1, 1));
            newMoveNumber = int.Parse(newMove.Substring(1, 1));
            this.fen = fen;
            fenLength = fen.IndexOf(' ');
            playerColor = fen[fenLength + 1] == 'w';
        }
        public string GetFen()
        {
            return fen;
        }
        public bool Move()
        {
            int dir = playerColor ? 1 : -1;
            int start = playerColor ? 1 : 6;
            int end = playerColor ? 3 : 4;
            string ep = playerColor ? "Z" : "z";

            if (oldMoveNumber + dir == newMoveNumber)
            {
                if (oldMoveLetter == newMoveLetter && !PieceCapture())
                {
                    Promotion();
                    return true;
                }

                if (Math.Abs(oldMoveLetter - newMoveLetter) == 1 && PieceCapture())
                {
                    Promotion();
                    return true;
                }

                if (Math.Abs(oldMoveLetter - newMoveLetter) == 1 && EnPassant())
                    return true;
            }
            else if (oldMoveNumber == start && newMoveNumber == end && oldMoveLetter == newMoveLetter && check.HorVerMoves() && !PieceCapture())
            {
                fen = DelEnPassant(playerColor, fen);
                string fenInfo = fen[(fen.Length - 5)..].Replace("-", ep + newMoveLetter);             
                fen = fen[..^5] + fenInfo;
                return true;
            }

            return false;
        }
        public string DelEnPassant(bool playerColor, string fen)
        {
            string ep = playerColor ? "z" : "Z";
            int epIndex = fen.IndexOf(ep);

            if (epIndex == -1)
                return fen;

            return fen.Replace(fen.Substring(epIndex, 2), "-");
        }
        public bool Promotion()
        {
            if (playerColor ? newMoveNumber == 7 : newMoveNumber == 0)
            {
                int newQueenPos = fenLength - (8 - newMoveLetter + newMoveNumber * 9);
                fen = fen[..newQueenPos] + (playerColor ? "Q" : "q") + fen[(newQueenPos + 1)..];
                return true;
            }

            return false;
        }
        public bool EnPassant()
        {
            string ep = playerColor ? "z" : "Z";
            int epPos = playerColor ? 4 : 3;
            int epIndex = fen.IndexOf(ep);

            if (epIndex == -1 || epPos != oldMoveNumber)
                return false;

            string enPassant = fen.Substring(epIndex, 2);

            if (enPassant == ep + newMoveLetter)
            {
                int pawnPos = fenLength - (8 - newMoveLetter + oldMoveNumber * 9);
                fen = fen[..pawnPos] + "1" + fen[(pawnPos + 1)..];
                return true;
            }

            return false;
        }
        private bool PieceCapture()
        {
            int targetPiecePos = fenLength - (8 - newMoveLetter + newMoveNumber * 9);
            return !char.IsDigit(fen[targetPiecePos]);
        }
    }
}