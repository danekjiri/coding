package cz.cuni.mff.danekji1.brainfuck;

import java.util.Optional;

public enum Command {
    SHIFT_RIGHT('>', null),
    SHIFT_LEFT('<', null),
    INCREMENT_CURRENT('+', null),
    DECREMENT_CURRENT('-', null),
    PRINT_CURRENT('.', null ),
    STORE_CURRENT(',', null ),
    BEQZ_MOVE_RIGHT_PAIR('[', new CommandPair(']', true)),
    BNEQZ_MOVE_LEFT_PAIR(']', new CommandPair('[', false)),
    ;

    private final char symbol;
    private CommandPair commandPair;

    Command(char symbol, CommandPair commandPair) {
        this.symbol = symbol;
        this.commandPair = commandPair;
    }

    public char getSymbol() {
        return symbol;
    }

    public CommandPair getCommandPair() {
        return commandPair;
    }

    public boolean isOpeningCommand() {
        return commandPair != null && commandPair.isOpening();
    }

    public boolean isClosingCommand() {
        return commandPair != null && !commandPair.isOpening();
    }

    public static Optional<Command> tryParseCommand(char symbol) {
        for (Command command : Command.values()) {
            if (command.symbol == symbol) {
                return Optional.of(command);
            }
        }

        return Optional.empty();
    }
}
