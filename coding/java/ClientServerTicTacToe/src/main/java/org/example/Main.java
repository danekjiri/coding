package org.example;

import org.example.client.Client;
import org.example.game.TicTacToe;
import org.example.server.Server;

import java.io.IOException;

public class Main {
    public static void main(String[] args) {

        if (args.length == 3) {
            // server
            parseArgsAndStartServer(args);
        } else if (args.length == 2) {
            // client
            parseArgsAndStartClient(args);
        }
    }

    private static void parseArgsAndStartServer(String[] args) {
        int port = Integer.parseInt(args[0]);
        int gameFieldSize = Integer.parseInt(args[1]);
        int sequenceLenghtToWin = Integer.parseInt(args[2]);
        TicTacToe game = new TicTacToe(gameFieldSize, sequenceLenghtToWin);
        try {
            var server = new Server(port, game, System.in, System.out);
            server.start();
        } catch (IOException ex) {
            System.err.println("Could not start server");
        }
    }

    private static void parseArgsAndStartClient(String[] args) {
        int port = Integer.parseInt(args[0]);
        String hostname = args[1];
        var client = new Client(port, hostname, System.in, System.out);
        client.start();
    }
}