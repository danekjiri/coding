package cz.cuni.mff.danekji1.programs;

import cz.cuni.mff.danekji1.util.DynamicArray;

/**
 * In your implementation of a dynamic array (as per the assignment DYNAMIC ARRAY II), add the following:
 *
 * A) Static method of.
 * The method takes any number of parameters of any type and returns a new dynamic array containing the given
 *  parameters. Usage example:
 *  DynArray arr = DynArray.of("hello", "world", "!");
 *
 * B) Method sort, which sorts the array using Quicksort. The method assumes that it will always be called on
 *  an array with "comparable" objects, meaning the objects will always implement the java.lang.Comparable
 *  interface, so it is safe to cast the objects to Comparable in the sort method.
 *
 * Write a program that takes parameters from the command line, inserts them into an instance of your array,
 *  sorts them using the sort method, and then prints the array to the standard output, with each parameter
 *  on a separate line.
 *
 * Note: The type String implements Comparable, so the parameters can be directly inserted into the array.
 */
public class Main {
    public static void main(String[] args) {
        var dynamicCollection = DynamicArray.of(args);
        dynamicCollection.sort();

        int collectionSize = dynamicCollection.size();
        for (int i = 0; i < collectionSize; i++) {
            System.out.println(dynamicCollection.get(i));
        }

    }
}