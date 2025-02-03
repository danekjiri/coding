package cz.cuni.mff.danekji1.brainfuck;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;

public class Main {

    public static void main(String[] args) {
        // verify args count
        if (args.length > 2 || args.length == 0) {
            System.out.println("Invalid number of arguments");
        }

        // parse args
        int tapeCapacity = args.length == 2 ? Integer.parseInt(args[1]) : Tape.DEFAULT_CAPACITY;
        String fileWithProgramme = args[0];

        // get programme into string and run interpreter
        try (InputStream is = new FileInputStream(fileWithProgramme)) {
            String programmeString = new String(is.readAllBytes());
            var verificationResult = ProgramVerifier.verify(programmeString);

            if (verificationResult.isEmpty()) {
                var interpreter = new Interpreter(tapeCapacity, programmeString);
                interpreter.run();
            } else {
                System.out.println(verificationResult.get());
            }
        } catch (IOException ex) {
            System.err.println("Error while reading programme: " + ex.getMessage());
        } catch (MemOverrunException | MemUnderrunException e) {
            System.out.println(e.getMessage());
        }
    }
}