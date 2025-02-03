package cz.cuni.mff.danekji1.brainfuck;

public class MemUnderrunException extends RuntimeException {
    public MemUnderrunException(String message) {
        super(message);
    }
}
