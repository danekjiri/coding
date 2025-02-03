package org.example.first;

public class Calculator {

    public double eval(String expr) {
        return new Input(expr).eval(expr);
    }

    private static class Input {
        private String str;
        private int pos;

        public Input(String str) {
            this.str = str;
        }

        public int ahead() {
            skipWhiteSpace();
            return pos == str.length() ? -1 : str.charAt(pos);
        }

        public char read() {
            return str.charAt(pos++);
        }

        public double readDouble() {
            skipWhiteSpace();

            int start = pos;

            if (pos < str.length() && str.charAt(pos) == '-') {
                pos++;
            }

            while (pos < str.length() &&  (str.charAt(pos) >= '0' && str.charAt(pos) <= '9' || str.charAt(pos) == '.')) {
                pos++;
            }

            if (pos < str.length() && (str.charAt(pos) == 'e' || str.charAt(pos) == 'E')) {
                pos++;
                if (pos < str.length() && str.charAt(pos) == '-') {
                    pos++;
                }

                while (pos < str.length() &&  (str.charAt(pos) >= '0' && str.charAt(pos) <= '9')) {
                    pos++;
                }
            }

            String number = str.substring(start, pos);
            return Double.parseDouble(number);
        }

        public void skipWhiteSpace() {
            while (pos < str.length() && Character.isWhitespace(str.charAt(pos))) {
                pos++;
            }
        }

        private Input input;

        public double eval(String expr) {
            input = new Input(expr);

            double result = evalExpression();

            if (input.ahead() != -1) {
                throw new IllegalArgumentException("Unexpected character: " + input.ahead());
            }

            return result;
        }

        private double evalExpression() {
            double result = evalTerm();

            while (true) {
                switch (input.ahead()) {
                    case '+' -> {
                        input.read();
                        result += evalTerm();
                    }
                    case '-' -> {
                        input.read();
                        result -= evalTerm();
                    }
                    default -> {
                        return result;
                    }

                }
            }
        }

        private double evalTerm() {
            double result = evalFactor();

            while (true) {
                switch (input.ahead()) {
                    case '*' -> {
                        input.read();
                        result *= evalFactor();
                    }
                    case '/' -> {
                        input.read();
                        result /= evalFactor();
                    }
                    default -> {
                        return result;
                    }
                }
            }
        }

        private double evalFactor() {
            if (input.ahead() == '(') {
                input.read();
                double result = evalExpression();

                if (input.ahead() != ')') {
                    throw new IllegalArgumentException("Expected ')'");
                }

                input.read();
                return result;
            } else {
                return input.readDouble();
            }
        }
    }
}
