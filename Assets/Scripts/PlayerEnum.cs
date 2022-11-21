public enum PlayerEnum
{
   None, Black, White
}

public static class PlayerExtensions
{
   public static PlayerEnum Opponent(this PlayerEnum player)
   {
      if (player == PlayerEnum.Black)
      {
         return PlayerEnum.White;
      }
      else if (player == PlayerEnum.White)
      {
         return PlayerEnum.Black;
      }

      return PlayerEnum.None;
   }
}