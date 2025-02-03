package cz.cuni.mff.java.hw;

import cz.cuni.mff.danekji1.util.Arrays;

import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;

public class MainParaMergeSort {
    public static void main(String[] args) {
        var scanner = new Scanner(System.in);
        var inputList = new ArrayList<Integer>();

        while (scanner.hasNextLine()) {
            String trimmedLine = scanner.nextLine().trim();
            try {
                int parsedInput = Integer.parseInt(trimmedLine);
                inputList.add(parsedInput);
            } catch (NumberFormatException ignored) { }
        }

        var array = ArrayIntegerListToIntArray(inputList);
        Arrays.paraMergeSort(array);

        for (int num : array) {
            System.out.println(num);
        }
    }


    /**
     * Converts dynamic collection List<Integer> into int[] of its size
     *
     * @param list the data structure that will be converted into int[]
     * @return int array of list argument size
     */
    private static int[] ArrayIntegerListToIntArray(List<Integer> list) {
        var result = new int[list.size()];
        for (int i = 0; i < list.size(); i++) {
            result[i] = list.get(i);
        }

        return result;
    }
}