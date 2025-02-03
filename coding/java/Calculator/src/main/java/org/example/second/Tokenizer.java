package org.example.second;

public class Tokenizer {
    private final String input;
    private int pos;

    public Tokenizer(String input) {
        this.input = input;
        this.pos = 0;
    }

    public Token nextToken() {
        skipWhitespace();

        if (pos >= input.length()) {
            return new Token(TokenType.EOF, "");
        }

        char ch = input.charAt(pos);

        // Single-character tokens
        switch (ch) {
            case '+':
                pos++;
                return new Token(TokenType.PLUS, "+");
            case '-':
                if (pos + 1 < input.length() && Character.isDigit(input.charAt(pos + 1))) {
                    return parseNumber();
                } else {
                    pos++;
                    return new Token(TokenType.MINUS, "-");
                }
            case '*':
                pos++;
                return new Token(TokenType.STAR, "*");
            case '/':
                pos++;
                return new Token(TokenType.SLASH, "/");
            case '(':
                pos++;
                return new Token(TokenType.LPAREN, "(");
            case ')':
                pos++;
                return new Token(TokenType.RPAREN, ")");

            default:
                // If it's a digit or a dot, parse a number
                if (Character.isDigit(ch) || ch == '.') {
                    return parseNumber();
                }
                throw new IllegalArgumentException("Unexpected character: " + ch);
        }
    }

    private Token parseNumber() {
        int startPos = pos;
        boolean foundDigit = false;

        // Allow an initial minus sign if we didn't already consume it
        if (input.charAt(pos) == '-') {
            pos++;
        }

        while (pos < input.length()) {
            char c = input.charAt(pos);

            if (Character.isDigit(c)) {
                foundDigit = true;
                pos++;
            } else if (c == '.') {
                pos++;
            } else if (c == 'e' || c == 'E') {
                pos++;
                // optional sign after e/E
                if (pos < input.length() && (input.charAt(pos) == '+' || input.charAt(pos) == '-')) {
                    pos++;
                }
            } else {
                // Not part of number
                break;
            }
        }

        if (!foundDigit) {
            throw new IllegalArgumentException("Invalid number format at position " + startPos);
        }

        String numberStr = input.substring(startPos, pos);
        return new Token(TokenType.NUMBER, numberStr);
    }

    private void skipWhitespace() {
        while (pos < input.length() && Character.isWhitespace(input.charAt(pos))) {
            pos++;
        }
    }
}
