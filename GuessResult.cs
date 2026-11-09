namespace Bulls_And_Cows.Models;

﻿public class GuessResult
{
    /// <summary>Number of digits that are correct AND in the correct position.</summary>
    public int Bulls { get; set; }

    /// <summary>Number of digits that are correct but in the wrong position.</summary>
    public int Cows { get; set; }

    /// <summary>
    /// The number of digits required for the current difficulty.
    /// </summary>
    public int CodeLength { get; set; }

    /// <summary>
    /// True when all digits are correct and in the correct positions.
    /// </summary>
    public bool IsWinningGuess => Bulls == CodeLength;
}