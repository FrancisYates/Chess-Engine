namespace ChessUI.Enums
{
    public enum MoveType
    {
        move = 0b_0000_0000,
        enPesant = 0b_0000_0001,
        doublePawnMove = 0b_0000_0010,
        capture = 0b_1000_0000,
        castle = 0b_0100_0000,
        promotion = 0b_0010_0000
    }
}
