using System.Linq;
using System.IO;

namespace Demo.Entities
{
    public partial class Game
    {
        public string ImagePath
        {
            get
            {
                string fileName = string.IsNullOrWhiteSpace(Image)
                    ? "picture.png"
                    : Path.GetFileName(Image);

                return $"/Resurs/{fileName}";
            }
        }

        public double AvgRating
        {
            get
            {
                var ratings = App.Context.Review
                    .Where(r => r.Game_Id == Id_Game)
                    .Select(r => (double?)r.Rating)
                    .ToList();

                if (ratings.Count == 0) return 0;

                return ratings.Average() ?? 0;
            }
        }

        public string RatingText
        {
            get
            {
                if (AvgRating == 0)
                    return "Нет оценок";

                return $"⭐ {AvgRating:0.0}";
            }
        }

        public bool IsOwned
        {
            get
            {
                return App.Context.UserGame
                    .Any(x => x.Game_Id == Id_Game &&
                              x.User_Id == App.CurrentUser.Id_User);
            }
        }
    }
}