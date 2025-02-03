package cz.cuni.mff.danekji1.util;

import java.util.Comparator;

public class MergeSort {

    public static <T extends Comparable<T>> void sort(T[] arr) {
        sort(arr, 0, arr.length - 1);
    }

    private static <T extends Comparable<T>> void merge(T[] sequence, int left, int middle, int right) {
        // todo: implement merge part of mergesort
    }

    private static <T extends Comparable<T>> void sort(T[] sequence, int left, int right) {
        if (left >= right) {
            return;
        }

        int middle = left + ((right - left) / 2);
        sort(sequence, left, middle);
        sort(sequence, middle, right);
        merge(sequence, left, middle, right);
    }

}
