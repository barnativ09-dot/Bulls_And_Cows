using System;
using System.Collections.Generic;
using System.Linq;

namespace Bulls_And_Cows.Models
{
    /// <summary>
    /// Owns the secret number and enforces every rule of the game.
    /// This class must never call Console.ReadLine or Console.WriteLine.
    /// </summary>
    public class BullsAndCowsGame
    {
        private readonly Random _random;
        private string _secretNumber = string.Empty;
        private int _codeLength;

        /// <summary>How many guesses have been submitted in the current game.</summary>
        public int AttemptCount { get; private set; }

        /// <summary>True once the player has won the current game.</summary>
        public bool IsGameOver { get; private set; }

        /// <summary>The number of digits in the current secret number.</summary>
        public int CodeLength
        {
            get { return _codeLength; }
        }

        /// <summary>
        /// Creates a new game and immediately picks a secret number, ready to play.
        /// The default difficulty is 4 digits.
        /// </summary>
        public BullsAndCowsGame()
        {
            _random = new Random();
            _codeLength = 4;
            StartNewGame();
        }

        /// <summary>
        /// Creates a new game using the selected difficulty.
        /// Allows 4, 5, or 6 digit secret numbers.
        /// </summary>
        public BullsAndCowsGame(int codeLength)
        {
            _random = new Random();

            if (codeLength != 4 && codeLength != 5 && codeLength != 6)
            {
                throw new ArgumentException("Code length must be 4, 5, or 6.");
            }

            _codeLength = codeLength;
            StartNewGame();
        }

        /// <summary>
        /// Picks a brand-new secret number and resets AttemptCount and IsGameOver.
        /// Can be called at any time, even mid-game.
        /// </summary>
        public void StartNewGame()
        {
            _secretNumber = GenerateSecretNumber();
            AttemptCount = 0;
            IsGameOver = false;
        }

        /// <summary>
        /// Changes the difficulty and starts a brand-new game.
        /// </summary>
        public void StartNewGame(int codeLength)
        {
            if (codeLength != 4 && codeLength != 5 && codeLength != 6)
            {
                throw new ArgumentException("Code length must be 4, 5, or 6.");
            }

            _codeLength = codeLength;
            StartNewGame();
        }

        /// <summary>
        /// Builds the secret number: shuffle the digits 0-9, take the required
        /// number of digits, and make sure the first digit isn't 0.
        /// </summary>
        private string GenerateSecretNumber()
        {
            List<int> digits = Enumerable.Range(0, 10).ToList();

            // Fisher-Yates shuffle
            for (int i = digits.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (digits[i], digits[j]) = (digits[j], digits[i]);
            }

            // Make sure the first digit isn't 0
            if (digits[0] == 0)
            {
                int swapIndex = digits.FindIndex(d => d != 0);
                (digits[0], digits[swapIndex]) =
                    (digits[swapIndex], digits[0]);
            }

            return string.Concat(digits.Take(_codeLength));
        }

        /// <summary>
        /// Checks that a guess is properly formatted BEFORE it's scored.
        /// Checks, in order: correct length, all digits, all unique.
        /// Stops at the first failure.
        /// </summary>
        public bool IsValidGuess(string guess, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(guess) || guess.Length != _codeLength)
            {
                errorMessage = $"Please enter exactly {_codeLength} digits.";
                return false;
            }

            if (!guess.All(char.IsDigit))
            {
                errorMessage = $"Please enter exactly {_codeLength} digits.";
                return false;
            }

            if (guess.Distinct().Count() != _codeLength)
            {
                errorMessage = "All digits must be different from each other.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Scores an already-validated guess against the secret number,
        /// increases AttemptCount, and sets IsGameOver if it's a winning guess.
        /// </summary>
        public GuessResult SubmitGuess(string guess)
        {
            if (IsGameOver)
            {
                throw new InvalidOperationException(
                    "Cannot submit a guess: the game is already over. Start a new game first.");
            }

            int bulls = 0;

            for (int i = 0; i < _codeLength; i++)
            {
                if (guess[i] == _secretNumber[i])
                {
                    bulls++;
                }
            }

            int commonDigits = guess.Count(c => _secretNumber.Contains(c));
            int cows = commonDigits - bulls;

            AttemptCount++;

            GuessResult result = new GuessResult
            {
                Bulls = bulls,
                Cows = cows,
                CodeLength = _codeLength
            };

            if (result.IsWinningGuess)
            {
                IsGameOver = true;
            }

            return result;
        }
    }
}