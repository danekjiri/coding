package org.example.game;

public enum GameSymbol {
    X('X'),
    O('O'),
    NONE(' ');

    private char symbol;

    public char getSymbol() {
        return symbol;
    }

    GameSymbol(char symbol) {
        this.symbol = symbol;
    }
}
