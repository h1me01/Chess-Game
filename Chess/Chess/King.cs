namespace Chess
{
    public class King
    {
        private readonly Fen fenFunctions = new();
        private readonly int oldMoveLetter;
        private readonly int newMoveLetter;
        private readonly int oldMoveNumber;
        private readonly int newMoveNumber;
        private readonly int fenLength;
        private readonly bool playerColor;
        private readonly char piece;
        private string fen;

        public King(string oldMove, string newMove, string fen, char piece)
        {
            oldMoveLetter = int.Parse(oldMove[..1]);
            newMoveLetter = int.Parse(newMove[..1]);
            oldMoveNumber = int.Parse(oldMove.Substring(1, 1));
            newMoveNumber = int.Parse(newMove.Substring(1, 1));
            this.fen = fen;
            fenLength = fen.IndexOf(' ');
            playerColor = fen[fenLength + 1] == 'w';
            this.piece = piece;
        }
        public string GetFen()
        {
            return fen;
        }
        public bool KingRange()
        {
            int diffLetters = Math.Abs(oldMoveLetter - newMoveLetter);
            int diffNumbers = Math.Abs(oldMoveNumber - newMoveNumber);

            if (diffLetters > 1 || diffNumbers > 1)
                return false;

            return true;
        }
        public string DisallowCastling(string fen)
        {
            string fenInfo = fen[(fenLength + 3)..];
            string[] kingSymbol = playerColor ? new string[] { "K", "Q" } : new string[] { "k", "q" };

            for (int i = 0; i < kingSymbol.Length; i++)
                fenInfo = fenInfo.Replace(kingSymbol[i], "");

            if (!fenInfo.Any(c => "qrbnp".Contains(c)))
                fenInfo = '-' + fenInfo;

            return fen[..(fenLength + 3)] + fenInfo;
        }
        public bool OppKing()
        {
            int[] dx = { 1, 1, 1, 0, 0, -1, -1, -1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] kingCords = KingCords("", 'k', 'K');

            for (int i = 0; i < dx.Length; i++)
            {
                int newRow = kingCords[0] + dx[i];
                int newCol = kingCords[1] + dy[i];

                if (newRow == newMoveNumber && newCol == newMoveLetter)
                    return true;
            }

            return false;
        }
        public bool Castling()
        {
            string tempFen = fen;
            string fenInfo = fen[(fenLength + 3)..];
            char kingSymbol = playerColor ? 'K' : 'k';
            char queenSymbol = playerColor ? 'Q' : 'q';
            char rookSymbol = playerColor ? 'R' : 'r';
            bool allowKs = newMoveLetter == 6 && fenInfo.Contains(kingSymbol);
            bool allowQs = newMoveLetter == 2 && fenInfo.Contains(queenSymbol);

            if (!allowKs && !allowQs)
                return false;

            for (int i = 1; i < (allowKs ? 3 : 4); i++)
            {
                int newKingLetter = allowKs ? oldMoveLetter + i : oldMoveLetter - i;
                int currentSquare = fenLength - (8 - newKingLetter + oldMoveNumber * 9);

                if (!int.TryParse(fen.AsSpan(currentSquare, 1), out _))
                    return false;

                if (Checks(true))
                    return false;

                fen = fenFunctions.Update(fen, playerColor ? "40" : "47", newKingLetter + (playerColor ? "0" : "7"), playerColor ? 'K' : 'k', false);
            }

            fen = tempFen;
            string[] kingPos;
            string[] rookPos;

            if (allowKs)
            {
                kingPos = playerColor ? new string[] { "40", "60" } : new string[] { "47", "67" };
                rookPos = playerColor ? new string[] { "70", "50" } : new string[] { "77", "57" };

            }
            else
            {
                kingPos = playerColor ? new string[] { "40", "20" } : new string[] { "47", "27" };
                rookPos = playerColor ? new string[] { "00", "30", } : new string[] { "07", "37", };
            }

            fen = fenFunctions.Update(fen, kingPos[0], kingPos[1], kingSymbol, false);
            fen = fenFunctions.Update(fen, rookPos[0], rookPos[1], rookSymbol, false);

            fen = DisallowCastling(fen);
            return true;
        }
        public bool Checks(bool castles = false)
        {
            string tempFen = !castles ? fenFunctions.Update(fen, $"{oldMoveLetter}{oldMoveNumber}", $"{newMoveLetter}{newMoveNumber}", piece, false) : fen;
            int[] kingCords = KingCords(tempFen);
            int kingPos = fenLength - (8 - kingCords[1] + kingCords[0] * 9);

            if (QRBCheck(kingPos, tempFen) || KCheck(kingPos, tempFen) || PCheck(kingPos, tempFen))
                return true;

            return false;
        }
        private int[] KingCords(string tempFen = "", char king = 'K', char king2 = 'k')
        {
            int kN = 0;
            int kLN = 0;

            for (int i = 0; i < fenLength; i++)
            {
                char c = tempFen == "" ? fen[i] : tempFen[i];

                if (playerColor && c == king)
                {
                    kLN = i % 9;
                    kN = 7 - i / 9;
                }
                else if (!playerColor && c == king2)
                {
                    kLN = i % 9;
                    kN = (i / 9 - 7) * -1;
                }
            }

            int[] kingCords = { kN, kLN };
            return kingCords;
        }
        private bool QRBCheck(int kP, string tempFen)
        {
            char queenSymbol = playerColor ? 'q' : 'Q';
            char rookSymbol = playerColor ? 'r' : 'R';
            char bishopSymbol = playerColor ? 'b' : 'B';
            int[] horizontalOffsets = { -1, 1 };
            int[] verticalOffsets = { -9, 9 };
            int[] diagonalOffsets = { -10, -8, 8, 10 };

            bool IsValidPosition(int pos)
            {
                return pos >= 0 && pos < fenLength;
            }

            bool IsNotEnemy(int pos)
            {
                return !char.IsDigit(tempFen[pos]);
            }

            foreach (var horOffset in horizontalOffsets)
            {
                int pos = kP + horOffset;

                while (IsValidPosition(pos))
                {
                    if (tempFen[pos] == queenSymbol || tempFen[pos] == rookSymbol)
                        return true;

                    if (IsNotEnemy(pos))
                        break;

                    pos += horOffset;
                }
            }

            foreach (var verOffset in verticalOffsets)
            {
                int pos = kP + verOffset;

                while (IsValidPosition(pos))
                {
                    if (tempFen[pos] == queenSymbol || tempFen[pos] == rookSymbol)
                        return true;

                    if (IsNotEnemy(pos))
                        break;

                    pos += verOffset;
                }
            }

            foreach (var diagOffset in diagonalOffsets)
            {
                int pos = kP + diagOffset;

                while (IsValidPosition(pos))
                {
                    if (tempFen[pos] == queenSymbol || tempFen[pos] == bishopSymbol)
                        return true;

                    if (IsNotEnemy(pos))
                        break;

                    pos += diagOffset;
                }
            }

            return false;
        }
        private bool KCheck(int kP, string tempFen)
        {
            List<int> knigthPositions = new();
            string knightSymbol = playerColor ? "n" : "N";
            int index = tempFen.IndexOf(knightSymbol);
            int[] possMoves = { 7, 11, -7, -11, 17, 19, -17, -19 };

            while (index != -1)
            {
                knigthPositions.Add(index);
                index = tempFen.IndexOf(knightSymbol, index + 1);
            }

            foreach (var item in knigthPositions)
            {
                for (int i = 0; i < possMoves.Length; i++)
                    if (item + possMoves[i] == kP)
                        return true;
            }

            return false;
        }
        private bool PCheck(int kP, string tempFen)
        {
            char pawnSymbol = playerColor ? 'p' : 'P';
            int rightPawn = playerColor ? kP - 8 : kP + 8;
            int leftPawn = playerColor ? kP - 10 : kP + 10;

            if (rightPawn > 0 && rightPawn <= fenLength)
                if (tempFen[rightPawn] == pawnSymbol)
                    return true;

            if (leftPawn > 0 && leftPawn <= fenLength)
                if (tempFen[leftPawn] == pawnSymbol)
                    return true;

            return false;
        }
    }
}