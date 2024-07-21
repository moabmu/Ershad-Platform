using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModepEduYanbu.ViewModels.ViewModelsForPartialViews
{
    public class PartialStarsRatingWithCountLinkViewModel
    {
        public PartialStarsRatingWithCountLinkViewModel(int overallRating, int ratingCount, string url)
        {
            OverallRating = overallRating;
            RatingCount = ratingCount;
            URL = url;
        }

        public PartialStarsRatingWithCountLinkViewModel(int overallRating, int ratingCount, string action, string controller, Dictionary<string,string> parms)
        {
            OverallRating = overallRating;
            RatingCount = ratingCount;
            TargetAction = action;
            TargetController = controller;
            Parameters = parms;
        }

        public readonly int OverallRating;
        public readonly int RatingCount;
        public readonly string TargetAction;
        public readonly string TargetController;
        public readonly Dictionary<string, string> Parameters;
        public readonly string URL;
    }
}
