package org.example;

import java.io.*;
import java.util.Scanner;

public class Processor {
    private final GameOfLife game;
    private final int iterations;

    private Processor(GameOfLife game, int iterations) {
        this.game = game;
        this.iterations = iterations;
    }

    public static Processor fromInput(InputStream input) {
        var scanner = new Scanner(input);
        // parse first line - rules
        int A = scanner.nextInt();
        int B = scanner.nextInt();
        int C = scanner.nextInt();
        scanner.nextLine();

        // parse second line - size, iterations
        int boardSize = scanner.nextInt();
        int iterations = scanner.nextInt();
        scanner.nextLine();

        // parse the rest of input - cells
        GameOfLife.Cell[][] cells = new GameOfLife.Cell[boardSize][boardSize];
        for (int line = 0; line < boardSize; line++) {
            String lineContent = scanner.nextLine();
            assert lineContent.length() == boardSize;

            for (int col = 0; col < boardSize; col++) {
                char character = lineContent.charAt(col);
                GameOfLife.Cell currentCell = GameOfLife.Cell.parseSymbol(character);
                cells[line][col] = currentCell;
            }
        }

        GameOfLife.Rules rules = new GameOfLife.Rules(A, B, C);
        GameOfLife game = new GameOfLife(cells, rules, boardSize);
        return new Processor(game, iterations);
    }

    public void execute() {
        for (int i = 0; i < iterations; i++) {
            game.step();
        }

        System.out.print(game.getFormattedBoard());
    }

    public static void main(String[] args) throws FileNotFoundException {
        String fileName = "/Users/jiridanek/Documents/mff_uk/5_semestr/java/GameOfLife/src/main/resources/input";

        FileInputStream input = new FileInputStream(fileName);
        //InputStream input = System.in;
        Processor processor = Processor.fromInput(input);
        processor.execute();
    }
}