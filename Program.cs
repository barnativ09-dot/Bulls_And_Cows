namespace bulls_and_cows___jonatan;

        
    public class Program
    {
        public static void Main(string[] args)
        {

            var game = new BullsAndCowsGame();
            PrintWelcome();
            while (true)
            {
                Console.Write("Enter your guess (or q/quit, n/new): ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                input = input.Trim();

                // c. If the text matches 'quit' -> print a goodbye message and exit the loop.
                if (IsQuit(input))
                {
                    Console.WriteLine("Thanks for playing Bulls and Cows. Goodbye!");
                    break;
                }

                // d. Else if the text matches 'new game' -> call StartNewGame(), clear history, print message.
                if (IsNewGame(input))
                {
                    game.StartNewGame();
                    Console.WriteLine("A new game has started. A fresh secret number has been picked!");
                    continue;
                }

                // e. Else if the text matches 'history' -> print every record in the history list.

                // f. Else if the game is already over -> tell the player to start a new game.
                if (game.IsGameOver)
                {
                    Console.WriteLine("The game is already over! Type 'n' to start a new game, or 'q' to quit.");
                    continue;
                }

                // g. Else, treat the text as a guess: first call IsValidGuess.
                if (!BullsAndCowsGame.IsValidGuess(input, out string errorMessage))
                {
                    Console.WriteLine($"Invalid guess: {errorMessage}");
                    continue;
                }

                // h. If the guess is valid, call SubmitGuess and get back a GuessResult.
                GuessResult result = game.SubmitGuess(input);

                // j. Print the Bulls/Cows feedback.
                Console.WriteLine($"Result: {result.Feedback}");

                // k. If the result is a winning guess, print a victory message with total attempts.
                if (result.IsWinningGuess)
                {
                    Console.WriteLine(
                        $"\nCongratulations! You guessed the secret number in {game.AttemptCount} attempts!");
                    Console.WriteLine("Type 'n' to play again, or 'q' to quit.");
                }
            }
            // Step 5: (Loop repeats from step 4 until the player quits.)
        }

        /// <summary>
        /// Checks if the player's input matches the "quit" command, ignoring uppercase/lowercase.
        /// </summary>
        private static bool IsQuit(string input)
        {
            string text = input.ToLower();
            return text == "q" || text == "quit" || text == "exit";
        }

        /// <summary>
        /// Checks if the player's input matches the "new game" command, ignoring uppercase/lowercase.
        /// </summary>
        private static bool IsNewGame(string input)
        {
            string text = input.ToLower();
            return text == "n" || text == "new";
        }

        /// <summary>
        /// Checks if the player's input matches the "history" command, ignoring uppercase/lowercase.
        /// </summary>
       

        /// <summary>
        /// Prints the game's title and a short explanation of the rules, once, when the program starts.
        /// </summary>
        private static void PrintWelcome()
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("           BULLS AND COWS GAME");
            Console.WriteLine("=========================================");
            Console.WriteLine("The computer has picked a secret 4-digit number.");
            Console.WriteLine("All 4 digits are different, and it does not start with 0.");
            Console.WriteLine("Try to guess it! For each guess you'll get:");
            Console.WriteLine("  Bulls - digits that are correct AND in the correct position.");
            Console.WriteLine("  Cows  - digits that are correct but in the wrong position.");
            Console.WriteLine("You win when you score 4 Bulls.");
            Console.WriteLine();
            Console.WriteLine("Commands: 'q' or 'quit' to exit, 'n' or 'new' for a new game,");
        }

        /// <summary>
        /// Prints every GuessRecord collected so far in a neat list.
        /// </summary>
        
    }



