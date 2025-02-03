package cz.cuni.mff.java.hw;

import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;
import cz.cuni.mff.danekji1.util.Arrays;

public class MainParaMax {
    public static void main(String[] args) {

        // init variables
        var scanner = new Scanner(System.in);
        List<Integer> list = new ArrayList<>();

        // load numbers into an array
        while (scanner.hasNextLine()) {
            String line = scanner.nextLine();
            try {
                int current = Integer.parseInt(line);
                list.add(current);
            } catch (NumberFormatException ignored) { }
        }

        // process array to find out max
        int[] input = toIntArray(list);
        int result = Arrays.max(input);

        // output array maximum
        System.out.println(result);
    }

    public static int[] toIntArray(List<Integer> array) {
        int[] result = new int[array.size()];

        for (int i = 0; i < array.size(); i++) {
           result[i] = array.get(i);
        }
        return result;
    }
}
