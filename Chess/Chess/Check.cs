namespace Chess
{
    public class Check
    {
        private readonly int oldMoveLetter;
        private readonly int newMoveLetter;
        private readonly int oldMoveNumber;
        private readonly int newMoveNumber;
        private readonly string fen;
        private readonly int fenLength;
        private readonly bool playerColor;

        public Check(string oldMove, string newMove, string fen)
        {
            oldMoveLetter = int.Parse(oldMove[..1]);
            newMoveLetter = int.Parse(newMove[..1]);
            oldMoveNumber = int.Parse(oldMove.Substring(1, 1));
            newMoveNumber = int.Parse(newMove.Substring(1, 1));
            this.fen = fen;
            fenLength = fen.IndexOf(' ');
            playerColor = fen[fenLength + 1] == 'w';
        }
        public bool DiagMoves()
        {
            int diffLetterNumber = Math.Abs(oldMoveLetter - newMoveLetter);
            int diffRankNumber = Math.Abs(oldMoveNumber - newMoveNumber);

            if (diffLetterNumber == diffRankNumber)
                if (!Blockades())
                    return true;

            return false;
        }
        public bool HorVerMoves()
        {
            if (oldMoveLetter == newMoveLetter || oldMoveNumber == newMoveNumber)
                if (!Blockades())
                    return true;

            return false;
        }
        public bool Blockades()
        {
            int diffNumbers = Math.Abs(oldMoveNumber - newMoveNumber);
            int diffLetters = Math.Abs(oldMoveLetter - newMoveLetter);
            bool isHor = oldMoveNumber == newMoveNumber;
            bool isVer = oldMoveLetter == newMoveLetter;
            bool isDiag = diffLetters == diffNumbers;
            int piecePos = fenLength - (8 - oldMoveLetter + oldMoveNumber * 9);

            for (int i = 1; i < Math.Max(diffNumbers, diffLetters); i++)
            {
                int hor = isHor ? i * Math.Sign(newMoveLetter - oldMoveLetter) : 0;
                int ver = isVer ? i * Math.Sign(newMoveNumber - oldMoveNumber) * -9 : 0;
                int diag = isDiag ? i * Math.Sign(newMoveNumber - oldMoveNumber) * -9 + i * Math.Sign(newMoveLetter - oldMoveLetter) : 0;
                int addDir = hor + ver + diag;

                if (!char.IsDigit(fen[piecePos + addDir]))
                    return true;
            }

            return false;
        }
        public bool OwnPieceCapture()
        {
            int targetPiecePos = fenLength - (8 - newMoveLetter + newMoveNumber * 9);
            char c = fen[targetPiecePos];

            if (char.IsDigit(c))
                return false;

            return playerColor == char.IsUpper(c);
        }
    }
}