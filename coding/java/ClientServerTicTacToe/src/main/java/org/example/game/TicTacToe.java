package org.example.game;

import java.util.Optional;

public class TicTacToe {
    public static final GameSymbol SERVER_SYMBOL = GameSymbol.X;
    public static final GameSymbol CLIENT_SYMBOL = GameSymbol.O;
    public static final String ROW_NAME = "123456789";
    public static final String COL_NAME = "ABCDEFGHI";

    private GameSymbol[][] gameField;
    private boolean gameRunning;
    private final int gameFieldSize;
    private final int sequenceLenghtToWin;

    public TicTacToe(int gameFieldSize, int sequenceLenghtToWin) {
        this.gameFieldSize = gameFieldSize;
        this.sequenceLenghtToWin = sequenceLenghtToWin;
        this.gameField = new GameSymbol[gameFieldSize][gameFieldSize];
        initGameField();
        this.gameRunning = true;
    }

    public boolean isGameRunning() {
        return gameRunning;
    }

    public GameSymbol[][] getGameField() {
        return gameField;
    }

    public String getFormattedGameField() {
        var sb = new StringBuilder();

        sb.append(' ');
        for (int i = 0; i < gameFieldSize; i++) {
            sb.append(COL_NAME.charAt(i));
        }
        sb.append(System.lineSeparator());

        for (int i = 0; i < gameFieldSize; i++) {
            sb.append(ROW_NAME.charAt(i));
            for (int j = 0; j < gameFieldSize; j++) {
                sb.append(gameField[i][j].getSymbol());
            }
            sb.append(System.lineSeparator());
        }

        return sb.toString();
    }

    public int getSequenceLenghtToWin() {
        return sequenceLenghtToWin;
    }

    public int getGameFieldSize() {
        return gameFieldSize;
    }

    public boolean playMove(GameMove move, GameSymbol symbol) {
        if (gameField[move.row()][move.column()] != GameSymbol.NONE) {
            return false;
        }

        gameField[move.row()][move.column()] = symbol;
        return true;
    }

    public Optional<GameSymbol> getWinner() {
        boolean emptyExists = false;

        for (int i = 0; i < gameFieldSize; i++) {
            for (int j = 0; j < gameFieldSize; j++) {
                if (gameField[i][j] == GameSymbol.NONE) {
                    emptyExists = true;
                    continue;
                }

                boolean winner = checkRightDownLeftDiagonalWinner(i, j);
                if (winner) {
                   gameRunning = false;
                   return Optional.of(gameField[i][j]); // winner symbol
                }
            }
        }
        if (emptyExists) {
            return Optional.empty(); // game still running
        }

        gameRunning = false;
        return Optional.of(GameSymbol.NONE); // tie
    }

    private boolean checkRightDownLeftDiagonalWinner(int row, int column) {
        int[][] directions = {{0, 1}, {1, 0}, {1, 1}};
        GameSymbol symbol = gameField[row][column];
        for (var direction : directions) {
            int sequenceCounter = 1;
            int currRow = row + direction[0];
            int currColumn = column + direction[1];
            while (currRow < gameFieldSize && currColumn < gameFieldSize) {
                if (gameField[currRow][currColumn] == symbol) {
                    sequenceCounter++;
                } else {
                    break;
                }

                if (sequenceCounter >= sequenceLenghtToWin) {
                    return true;
                }
                currRow += direction[0];
                currColumn += direction[1];
            }
        }

        return false;
    }

    public void initGameField() {
        for (int i = 0; i < gameFieldSize; i++) {
            for (int j = 0; j < gameFieldSize; j++) {
                gameField[i][j] = GameSymbol.NONE;
            }
        }
    }
}