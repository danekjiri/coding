package org.example.second;

public class Calculator {

    public double eval(String expression) {
        Parser parser = new Parser(expression);
        return parser.parseExpression();
    }
}
