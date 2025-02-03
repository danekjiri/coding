package org.example.client;

import org.example.game.*;

import java.io.*;
import java.net.Socket;
import java.util.Optional;

public class Client {
    private final int port;
    private final String hostname;
    private TicTacToe game;
    private final BufferedReader userInput;
    private final PrintWriter userOutput;

    public Client(int port, String hostname, InputStream userInput, OutputStream userOutput) {
        this.port = port;
        this.hostname = hostname;
        this.userInput = new BufferedReader(new InputStreamReader(userInput));
        this.userOutput = new PrintWriter(new OutputStreamWriter(userOutput), true);
    }

    public void start() {
        try {
            Thread.sleep(2000);
        } catch (InterruptedException e) {}

        try (var socket = new Socket(hostname, port)) {
            ObjectOutputStream out = new ObjectOutputStream(socket.getOutputStream());
            ObjectInputStream in = new ObjectInputStream(socket.getInputStream());

            GameSettings settings = (GameSettings) in.readObject();
            game = new TicTacToe(settings.gameFieldSize(), settings.gameFieldSize());
            
            Optional<GameSymbol> winner = Optional.empty();
            while (game.isGameRunning()) {
                // 1. print: waiting for other player
                userOutput.println("Waiting for opponent's move...");

                // 2. receive move from other player from socket
                GameMove opponentsMove = (GameMove) in.readObject();
                game.playMove(opponentsMove, TicTacToe.SERVER_SYMBOL);
                winner = game.getWinner();
                if (winner.isPresent()) {
                    break;
                }
                // 3. print: current playing field
                userOutput.println(game.getFormattedGameField());

                // 4. print: question where this player want to place move
                userOutput.println("Type your move coordinates:");
                var line = userInput.readLine();
                GameMove myMove = new GameMove(line.charAt(0) - '1', line.charAt(1) - 'A');
                game.playMove(myMove, TicTacToe.CLIENT_SYMBOL);

                // 5. send move to other side through socket
                out.writeObject(myMove);
                winner = game.getWinner();
                if (winner.isPresent()) {
                    break;
                }

                // 6. print: current playing field
                userOutput.println(game.getFormattedGameField());
            }

            userOutput.println(game.getFormattedGameField());
            userOutput.println("The winner is: " + winner.get().getSymbol());
        }
        catch (IOException | ClassNotFoundException ex) {
            userOutput.println("Client failed: " + ex.getMessage());
        }
    }
}
