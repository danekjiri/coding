package cz.cuni.mff.danekji1.brainfuck;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Optional;
import java.util.Stack;

public class Interpreter {
    private final String programme;
    private final InputStream input;
    private final OutputStream output;

    private final Tape tape;
    private final Stack<CommandInfo> cycleStack = new Stack<>();
    private int programCounter = 0;

    Interpreter(String programme) {
        this(Tape.DEFAULT_CAPACITY, programme);
    }

    Interpreter(int tapeCapacity, String programme) {
        this(tapeCapacity, programme, System.in, System.out);
    }

    Interpreter(int tapeCapacity, String programme, InputStream input, OutputStream output) {
        this.tape = new Tape(tapeCapacity);
        this.input = input;
        this.output = output;
        this.programme = programme;
    }

    private void movePastNextClosingPair(char openingPairSymbol, char closingPairSymbol) {
        int closingPairSymbolToSkip = 0;
        char currentSymbol;
        while (programCounter < programme.length()) {
            programCounter++;

            currentSymbol = programme.charAt(programCounter);
            if (currentSymbol == openingPairSymbol) {
                closingPairSymbolToSkip++;
            } else if (currentSymbol == closingPairSymbol) {
                closingPairSymbolToSkip--;
                if (closingPairSymbolToSkip < 0) {
                    break;
                }
            }
        }
    }

    public void run() {
        char currentSymbol;
        try {
            while (programCounter < programme.length()) {
                currentSymbol = programme.charAt(programCounter);
                Optional<Command> command = Command.tryParseCommand(currentSymbol);
                if (command.isPresent()) {
                    switch (command.get()) {
                        case SHIFT_RIGHT -> tape.incrementPointer();
                        case SHIFT_LEFT -> tape.decrementPointer();
                        case INCREMENT_CURRENT -> tape.incrementCurrentCell();
                        case DECREMENT_CURRENT -> tape.decrementCurrentCell();
                        case PRINT_CURRENT -> tape.printOutCurrentCell(output);
                        case STORE_CURRENT -> tape.acceptValueIntoCurrentCell(input);
                        case BEQZ_MOVE_RIGHT_PAIR -> {
                            if (tape.isCurrentCellZero()) {
                                movePastNextClosingPair(currentSymbol, command.get().getCommandPair().symbol());
                            } else {
                                cycleStack.push(new CommandInfo(command.get(), 0, programCounter));
                            }
                        }
                        case BNEQZ_MOVE_LEFT_PAIR -> {
                            if (!tape.isCurrentCellZero()) {
                                CommandInfo commandInfo = cycleStack.peek();
                                programCounter = commandInfo.character();
                            } else {
                                cycleStack.pop();
                            }
                        }
                    }
                }

                programCounter++;
            }
        } catch (IOException ex) {
            System.err.println(ex.getMessage());
        }
    }
}
