package cz.cuni.mff.danekji1.brainfuck;

import java.util.HashSet;
import java.util.Optional;
import java.util.Set;
import java.util.Stack;

public class ProgramVerifier {
    public static final char NEWLINE = '\n';
    public static final int LINE_START_INDEX = 1;
    public static final int CHARACTER_START_INDEX = 1;

    public static Optional<String> verify(String program) {
        var stack = new Stack<CommandInfo>();
        int lineCounter = LINE_START_INDEX;
        int charCounter = CHARACTER_START_INDEX;

        for (char c : program.toCharArray()) {
            Optional<Command> optionalCommand = Command.tryParseCommand(c);
            if (optionalCommand.isPresent()) {
                Command cmd = optionalCommand.get();

                if (cmd.isOpeningCommand()) {
                    stack.push(new CommandInfo(cmd, lineCounter, charCounter));
                } else if (cmd.isClosingCommand()) {
                    if (stack.isEmpty()) {
                        return Optional.of(getUnopenedError(lineCounter, charCounter));
                    }

                    CommandInfo lastPairCommand = stack.pop();
                    assert (lastPairCommand.command().getCommandPair() != null);
                    if (lastPairCommand.command().getCommandPair().symbol() != c) {
                        return Optional.of(getUnclosedError(lastPairCommand.line(), lastPairCommand.character()));
                    }
                }
            }

            // update line/char counters
            charCounter++;
            if (c == NEWLINE) {
                lineCounter++;
                charCounter = CHARACTER_START_INDEX;
            }
        }

        if (!stack.isEmpty()) {
            CommandInfo lastCommand = stack.pop();
            return Optional.of(getUnclosedError(lastCommand.line(), lastCommand.character()));
        }
        return Optional.empty();
    }

    private static String getUnopenedError(int line, int character) {
        return "Unopened cycle - line " + line + " character " + character;
    }
    private static String getUnclosedError(int line, int character) {
        return "Unclosed cycle - line " + line + " character " + character;
    }
}
