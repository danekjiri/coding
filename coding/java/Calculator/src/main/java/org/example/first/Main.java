package org.example.first;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

public class Main {
    public static void main(String[] args) throws IOException {
        BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
        var calc = new Calculator();

        String line;
        while((line = br.readLine()) != null) {
            double d = calc.eval(line);
            System.out.println(d);
        }
    }
}