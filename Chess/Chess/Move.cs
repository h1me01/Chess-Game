namespace Chess
{
    public class Move
    {
        private readonly Check check;
        private readonly Pawn pawn;
        private readonly Pieces pieces;
        private readonly King king;
        private readonly char piece;
        private readonly string fen;

        public Move(string oldMove, string newMove, string fen, char piece)
        {
            Fen fenFunctions = new();
            fen = fenFunctions.UntangleFen(fen);
            this.piece = char.ToLower(piece);
            check = new(oldMove, newMove, fen);
            pawn = new(oldMove, newMove, fen);
            pieces = new(oldMove, newMove, fen, piece);
            king = new(oldMove, newMove, fen, piece);
            this.fen = fen;
        }
        public string GetFen()
        {
            if (piece == 'p')
                return pawn.GetFen();
            else if (piece == 'r')
                return pieces.GetFen();
            else if (piece == 'k')
                return pieces.GetFen();
            return fen;
        }
        public bool Check()
        {
            if (check.OwnPieceCapture() || king.Checks())
                return false;

            return piece switch
            {
                'k' => pieces.King(),
                'q' => pieces.Queen(),
                'r' => pieces.Rook(),
                'b' => pieces.Bishop(),
                'n' => pieces.Knight(),
                'p' => pawn.Move(),
                _ => false,
            };
        }
        public bool[] End(string oldMove, string newMove, string fen, char piece)
        {
            End end = new(oldMove, newMove, fen, piece);
            bool[] result = new bool[] { false, false };

            if (end.Checkmate())
            {
                result[0] = true;
                result[1] = fen[fen.IndexOf(' ') + 1] == 'w';
            }
            else if (end.Stalemate() || end.Draw())
                result[0] = true;

            return result;
        }
    }
}