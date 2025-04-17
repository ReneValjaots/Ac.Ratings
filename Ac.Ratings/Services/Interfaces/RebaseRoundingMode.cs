using System.ComponentModel;

namespace Ac.Ratings.Services.Interfaces;

public enum RebaseRoundingMode {
    [Description("Round down")]
    RoundDown,
    [Description("Round up")]
    RoundUp
}