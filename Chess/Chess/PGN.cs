 namespace Chess
{
    public class PGN
    {
        private readonly string[] letters = { "a", "b", "c", "d", "e", "f", "g", "h" };
        private string? pgn;

        public string GetPGN()
        {
            return pgn ?? "";
        }
        public void SaveMoves(string oldMove, string newMove, char piece2, int currentMove)
        {
            char piece = char.ToLower(piece2);
            int oldMoveLetter = int.Parse(oldMove[..1]);
            int newMoveLetter = int.Parse(newMove[..1]);
            int newMoveNumber = int.Parse(newMove.Substring(1, 1));
            bool isCastle = piece == 'k' && oldMoveLetter == 4 && (newMoveLetter == 2 || newMoveLetter == 6);
            string currentNotation = char.ToUpper(piece).ToString() + letters[oldMoveLetter] + letters[newMoveLetter] + (newMoveNumber + 1);

            if (isCastle)
                currentNotation = newMoveLetter == 2 ? "O-O-O" : "O-O";

            pgn += (currentMove % 2 != 0 ? currentMove / 2 + 1 + ". " : "") + currentNotation.Replace("P", "") + " ";
        }
    }
}