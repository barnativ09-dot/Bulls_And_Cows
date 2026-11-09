using System;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;   // needed for Application.Current.Quit()
using Bulls_And_Cows.Models;   // your Model classes: BullsAndCowsGame, GuessResult

namespace Bulls_And_Cows.ViewModels
{
    /// <summary>
    /// The ViewModel for MainPage. It sits between the screen (View) and the
    /// game rules (Model). It holds the text the screen shows and reacts to
    /// the buttons. It never touches controls directly.
    /// </summary>
    public class GameViewModel : INotifyPropertyChanged
    {
        // ---------------------------------------------------------------
        // INotifyPropertyChanged: lets the screen know a property changed
        // ---------------------------------------------------------------
        public event PropertyChangedEventHandler? PropertyChanged;

        // Call this after a property value changes. The screen then
        // reads the new value and updates itself.
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // ---------------------------------------------------------------
        // The Model: it owns the secret number and enforces the rules
        // ---------------------------------------------------------------
        private BullsAndCowsGame _game;

        // ---------------------------------------------------------------
        // Backing fields (the real storage behind each property)
        // ---------------------------------------------------------------
        private string _statusMessage = string.Empty;
        private string _currentGuess = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _hasError = false;
        private bool _isGameActive = true;
        private int _guessCount = 0;

        // ---------------------------------------------------------------
        // Properties the screen binds to
        // ---------------------------------------------------------------

        /// <summary>Text under the New Game button (results, hints, win message).</summary>
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        /// <summary>What the player typed in the entry box (two-way binding).</summary>
        public string CurrentGuess
        {
            get { return _currentGuess; }
            set
            {
                _currentGuess = value;
                OnPropertyChanged(nameof(CurrentGuess));
            }
        }

        /// <summary>Text inside the pink error box.</summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));

                // The error box is visible only when there is some error text.
                HasError = (value != string.Empty);
            }
        }

        /// <summary>True when the pink error box should be visible.</summary>
        public bool HasError
        {
            get { return _hasError; }
            set
            {
                _hasError = value;
                OnPropertyChanged(nameof(HasError));
            }
        }

        /// <summary>False after a win, so the entry and guess buttons turn off.</summary>
        public bool IsGameActive
        {
            get { return _isGameActive; }
            set
            {
                _isGameActive = value;
                OnPropertyChanged(nameof(IsGameActive));
            }
        }

        /// <summary>How many guesses have been submitted in this game.</summary>
        public int GuessCount
        {
            get { return _guessCount; }
            set
            {
                _guessCount = value;
                OnPropertyChanged(nameof(GuessCount));
            }
        }

        // ---------------------------------------------------------------
        // Commands the buttons bind to (classes are at the end of this file)
        // ---------------------------------------------------------------
        public ICommand NewGameCommand { get; }
        public ICommand SubmitGuessCommand { get; }
        public ICommand QuitCommand { get; }

        // ---------------------------------------------------------------
        // Constructor: create the Model and the commands, then start playing
        // ---------------------------------------------------------------
        public GameViewModel()
        {
            _game = new BullsAndCowsGame();

            NewGameCommand = new NewGameButtonCommand(this);
            SubmitGuessCommand = new SubmitGuessButtonCommand(this);
            QuitCommand = new QuitButtonCommand(this);

            StartNewGame();
        }

        // ---------------------------------------------------------------
        // Methods the commands call
        // ---------------------------------------------------------------

        /// <summary>New Game button: pick a new secret and reset the screen.</summary>
        public void StartNewGame()
        {
            _game.StartNewGame();

            CurrentGuess = string.Empty;
            ErrorMessage = string.Empty;
            IsGameActive = true;
            GuessCount = _game.AttemptCount;   // 0 at the start of a game
            StatusMessage = "Starting a new game... Guess the " + _game.CodeLength + "-digit number.";
        }

        /// <summary>Submit a Guess button: checks the guess, then scores it.</summary>
        public void SubmitGuess()
        {
            // No guesses after a win.
            if (!IsGameActive)
            {
                return;
            }

            // Stop here if the guess is not valid (the error box shows why).
            if (!CurrentGuessIsValid())
            {
                return;
            }

            // The guess is valid, so the Model can score it.
            // (Trim removes stray spaces, just like the console app does.)
            GuessResult result = _game.SubmitGuess(CurrentGuess.Trim());

            if (result.IsWinningGuess)
            {
                StatusMessage = "Correct! You guessed it in "
                                + _game.AttemptCount + " attempts. Press New Game to play again.";
            }
            else
            {
                StatusMessage = "-> " + result.Bulls + " Bulls, " + result.Cows + " Cows";
            }

            // Update the guess counter shown on the screen.
            GuessCount = _game.AttemptCount;

            // Clear the entry box and turn the controls off if the game is over.
            CurrentGuess = string.Empty;
            IsGameActive = !_game.IsGameOver;
        }

        /// <summary>Quit button: closes the app.</summary>
        public void QuitGame()
        {
            Application.Current?.Quit();
        }

        /// <summary>
        /// Asks the Model if the current guess is allowed (same checks and
        /// messages as the console app). Shows or clears the error box.
        /// Returns true if the guess is valid.
        /// </summary>
        private bool CurrentGuessIsValid()
        {
            // The entry can be null, so use an empty string instead.
            string guess = (CurrentGuess ?? string.Empty).Trim();
            string errorMessage;

            if (_game.IsValidGuess(guess, out errorMessage))
            {
                ErrorMessage = string.Empty;
                return true;
            }

            ErrorMessage = errorMessage;
            return false;
        }
    }

    // ===================================================================
    // Command classes: each one implements ICommand and calls ONE method
    // of the ViewModel when its button is pressed.
    // ===================================================================

    /// <summary>Command for the "New Game" button.</summary>
    public class NewGameButtonCommand : ICommand
    {
        private readonly GameViewModel _viewModel;

        public NewGameButtonCommand(GameViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        // The button is always allowed to run, so this event is never raised.
        public event EventHandler? CanExecuteChanged { add { } remove { } }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _viewModel.StartNewGame();
        }
    }

    /// <summary>Command for the "Quit" button.</summary>
    public class QuitButtonCommand : ICommand
    {
        private readonly GameViewModel _viewModel;

        public QuitButtonCommand(GameViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public event EventHandler? CanExecuteChanged { add { } remove { } }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _viewModel.QuitGame();
        }
    }

    /// <summary>Command for the "Submit a Guess" button.</summary>
    public class SubmitGuessButtonCommand : ICommand
    {
        private readonly GameViewModel _viewModel;

        public SubmitGuessButtonCommand(GameViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public event EventHandler? CanExecuteChanged { add { } remove { } }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _viewModel.SubmitGuess();
        }
    }
}
