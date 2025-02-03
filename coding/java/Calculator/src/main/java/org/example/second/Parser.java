package org.example.second;


public class Parser {
    private final Tokenizer tokenizer;
    private Token currentToken;

    public Parser(String input) {
        this.tokenizer = new Tokenizer(input);
        this.advance(); // Load the first token
    }

    /**
     * Expression := Term (('+' | '-') Term)*
     */
    public double parseExpression() {
        double value = parseTerm();

        while (currentToken.getType() == TokenType.PLUS
                || currentToken.getType() == TokenType.MINUS) {

            TokenType op = currentToken.getType();
            advance(); // consume '+' or '-'

            double termValue = parseTerm();
            if (op == TokenType.PLUS) {
                value += termValue;
            } else {
                value -= termValue;
            }
        }

        return value;
    }

    /**
     * Term := Factor (('*' | '/') Factor)*
     */
    private double parseTerm() {
        double value = parseFactor();

        while (currentToken.getType() == TokenType.STAR
                || currentToken.getType() == TokenType.SLASH) {

            TokenType op = currentToken.getType();
            advance(); // consume '*' or '/'

            double factorValue = parseFactor();
            if (op == TokenType.STAR) {
                value *= factorValue;
            } else {
                value /= factorValue;
            }
        }

        return value;
    }

    /**
     * Factor := '(' Expression ')' | Number
     */
    private double parseFactor() {
        if (currentToken.getType() == TokenType.LPAREN) {
            // '(' Expression ')'
            advance(); // consume '('
            double innerValue = parseExpression();

            if (currentToken.getType() != TokenType.RPAREN) {
                throw new IllegalArgumentException("Missing closing parenthesis ')'");
            }
            advance(); // consume ')'
            return innerValue;
        } else if (currentToken.getType() == TokenType.NUMBER) {
            // It's a number
            double value = Double.parseDouble(currentToken.getValue());
            advance(); // consume the number
            return value;
        } else {
            throw new IllegalArgumentException("Unexpected token: " + currentToken.getType());
        }
    }

    /**
     * Advances to the next token in the stream.
     */
    private void advance() {
        currentToken = tokenizer.nextToken();
    }
}
