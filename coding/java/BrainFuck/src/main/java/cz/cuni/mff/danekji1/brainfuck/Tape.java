package cz.cuni.mff.danekji1.brainfuck;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Stack;

public class Tape {
    public static final int DEFAULT_CAPACITY = 30_000;

    private final char[] tape;
    public final int tapeCapacity;
    private int tapePointer = 0;

    Tape() {
       this(DEFAULT_CAPACITY);
    }

    Tape(int capacity) {
        tape = new char[capacity];
        tapeCapacity = capacity;
    }

    public void incrementPointer() {
        tapePointer++;

        if (tapePointer >= tapeCapacity) {
            throw new MemOverrunException("Memory overrun");
        }
    }

    public void decrementPointer() {
        tapePointer--;

        if (tapePointer < 0) {
            throw new MemUnderrunException("Memory underrun");
        }
    }

    public void incrementCurrentCell() {
        tape[tapePointer]++;
    }

    public void decrementCurrentCell() {
        tape[tapePointer]--;
    }

    public void printOutCurrentCell(OutputStream os) throws IOException {
        os.write(tape[tapePointer]);
        os.flush();
    }

    public void acceptValueIntoCurrentCell(InputStream is) throws IOException {
        int newValue = is.read();
        tape[tapePointer] = (char) newValue;
    }

    public boolean isCurrentCellZero() {
        return tape[tapePointer] == 0;
    }


}
