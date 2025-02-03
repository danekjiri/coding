package cz.cuni.mff.danekji1.brainfuck;

public class MemOverrunException extends RuntimeException {
    public MemOverrunException(String message) {
        super(message);
    }
}
