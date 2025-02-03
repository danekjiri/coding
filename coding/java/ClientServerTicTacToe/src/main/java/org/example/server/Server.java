package org.example.server;

import org.example.game.*;

import java.io.*;
import java.net.ServerSocket;
import java.util.Optional;

public class Server {
    private final ServerSocket serverSocket;
    private final int port;
    private final TicTacToe game;
    private final BufferedReader userInput;
    private final PrintWriter userOutput;

    public Server(int port, TicTacToe game, InputStream userInput, OutputStream userOutput)
            throws IOException {
        this.port = port;
        this.game = game;
        this.serverSocket = new ServerSocket(port);
        this.userInput = new BufferedReader(new InputStreamReader(userInput));
        this.userOutput = new PrintWriter(new OutputStreamWriter(userOutput), true);
    }

    public void start() {
        try (var socket = serverSocket.accept()) {
            // init in/out socket connection
            ObjectOutputStream out = new ObjectOutputStream(socket.getOutputStream());
            ObjectInputStream in = new ObjectInputStream(socket.getInputStream());

            // sent game settings to client
            GameSettings settings = new GameSettings(game.getGameFieldSize(), game.getSequenceLenghtToWin());
            out.writeObject(settings);

            // printout the field
            userOutput.println(game.getFormattedGameField());

            // game loop
            Optional<GameSymbol> winner = Optional.empty();
            while(game.isGameRunning()) {
                // 4. print: question where this player want to place move
                userOutput.println("Type your move coordinates:");
                var line = userInput.readLine();
                GameMove myMove = new GameMove(line.charAt(0) - '1', line.charAt(1) - 'A');
                game.playMove(myMove, TicTacToe.SERVER_SYMBOL);

                // 5. send move to other side through socket
                out.writeObject(myMove);
                winner = game.getWinner();
                if (winner.isPresent()) {
                    break;
                }

                // 6. print: current playing field
                userOutput.println(game.getFormattedGameField());

                // 1. print: waiting for other player
                userOutput.println("Waiting for opponent's move...");

                // 2. receive move from other player from socket
                GameMove opponentsMove = (GameMove) in.readObject();
                game.playMove(opponentsMove, TicTacToe.CLIENT_SYMBOL);
                winner = game.getWinner();
                if (winner.isPresent()) {
                    break;
                }
                // 3. print: current playing field
                userOutput.println(game.getFormattedGameField());
            }

            userOutput.println(game.getFormattedGameField());
            userOutput.println("The winner is: " + winner.get().getSymbol());
            serverSocket.close();
        } catch (IOException | ClassNotFoundException ex) {
            userOutput.println("Server failed: " + ex.getMessage());
        }
    }
}
