using System.ComponentModel;

namespace Ac.Ratings.Services.Interfaces;

public enum RatingRoundingMode {
    [Description("Round down")]
    RoundDown,
    [Description("Round up")]
    RoundUp
}