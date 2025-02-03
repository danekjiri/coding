package org.example;

import java.util.concurrent.*;

public class GameOfLife {
    private Cell[][] cells;
    private final int boardSize;
    private final Rules rules;
    private static final int[][] directions = {
            // clock-wise direction from top
            {0, 1}, {1, 1}, {1, 0}, {1, -1}, {0, -1}, {-1, -1}, {-1, 0}, {-1, 1}
    };
    private final int numberOfThreads;


    public GameOfLife(Cell[][] cells, Rules rules, int boardSize) {
        assert cells != null;
        assert cells.length == boardSize;
        assert cells[0].length == boardSize;

        this.cells = cells;
        this.rules = rules;
        this.boardSize = boardSize;
        this.numberOfThreads = Math.min(Runtime.getRuntime().availableProcessors(), boardSize);
    }

    private int getNumberOfAliveNeighbours(int row, int col) {

        int numberOfAliveNeighbours = 0;

        for (int[] direction : directions) {
            int currRow = (row + direction[0] + boardSize) % boardSize;
            int currCol = (col + direction[1] + boardSize) % boardSize;

            if (cells[currRow][currCol] == Cell.ALIVE) {
                numberOfAliveNeighbours++;
            }
        }

        return numberOfAliveNeighbours;
    }

    public void step() {
        Cell[][] newCells = new Cell[boardSize][boardSize];

        try (var executor = Executors.newFixedThreadPool(numberOfThreads)) {
            for (int row = 0; row < boardSize; row++) {
                int finalRow = row;
                executor.execute(() -> {
                    for (int col = 0; col < boardSize; col++) {
                        int numberOfAliveNeighbours = getNumberOfAliveNeighbours(finalRow, col);
                        if (cells[finalRow][col] == Cell.ALIVE) {
                            if (numberOfAliveNeighbours < rules.A()) { // first rule
                                newCells[finalRow][col] = Cell.DEAD;
                            } else if (numberOfAliveNeighbours > rules.B()) { // second rule
                                newCells[finalRow][col] = Cell.DEAD;
                            } else {
                                newCells[finalRow][col] = Cell.ALIVE; // third rule
                            }
                        } else { // is dead
                            if (numberOfAliveNeighbours == rules.C()) { // forth rule
                                newCells[finalRow][col] = Cell.ALIVE;
                            } else { // fifth rule
                                newCells[finalRow][col] = Cell.DEAD;
                            }
                        }
                    }
                });
            }
        }

        cells = newCells;
    }

    public String getFormattedBoard() {
        StringBuilder sb = new StringBuilder(boardSize*boardSize);

        for (int row = 0; row < boardSize; row++) {
            for (int col = 0; col < boardSize; col++) {
                sb.append(cells[row][col].getSymbol());
            }
            sb.append(System.lineSeparator());
        }

        return sb.toString();
    }

    public enum Cell {
        DEAD('_'),
        ALIVE('X');

        private char symbol;

        Cell(char symbol) {
            this.symbol = symbol;
        }

        public char getSymbol() {
            return symbol;
        }

        public static Cell parseSymbol(char symbol) {
            for (var cell : Cell.values()) {
                if (cell.symbol == symbol) {
                    return cell;
                }
            }

            throw new IllegalArgumentException("Incorrect symbol passed into the method.");
        }
    }

    public record Rules(int A, int B, int C) {}
}
