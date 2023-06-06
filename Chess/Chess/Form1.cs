namespace Chess
{
    public partial class Form : System.Windows.Forms.Form
    {
        public List<PictureBox> squares;
        public string fen;

        public Form()
        {
            InitializeComponent();
        }
        private void Form_Load(object sender, EventArgs e)
        {
            Board board = new(this);
            board.CreateBoard();
            squares = board.squares;
            Fen fen = new();
            fen.Draw(this);
            this.fen = fen.GetFen();
        }
    }
}