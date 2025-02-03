package cz.cuni.mff.danekji1.util;

public class QuickSort {

    /**
     * @see QuickSort#recursePivotDivide
     * @param arr array to be sorted
     */
    public static <T extends Comparable<T>> void sort(T[] arr) {
        recursePivotDivide(arr, 0, arr.length - 1);
    }

    /**
     * Recursively divide elements of an array by its value (smaller are left, greater are
     *  right of chosen pivot - last element of array) which lead to sorted array
     *  It is assumed that Object in passed array are (implements) Comparable
     * @param arr array to be sorted
     * @param left first index of subarray where to do division by pivot
     * @param right last index of subarray where to do division by pivot
     */
    private static <T extends Comparable<T>> void recursePivotDivide(T[] arr, int left, int right) {
        if (left >= right) {
            return;
        }

        var pivot = arr[right];
        int swapIndex = left;

        for (int i = left; i < right; i++) {
            if (pivot.compareTo(arr[i]) > 0) {
                swap(arr, i, swapIndex);
                swapIndex++;
            }
        }
        swap(arr, swapIndex, right);

        recursePivotDivide(arr, left, swapIndex - 1);
        recursePivotDivide(arr, swapIndex + 1, right);
    }

    /**
     * Swaps two elements of the array assuming correct arguments passed
     * @param arr array where elements to be swapped
     * @param i index of first element
     * @param j index of second element
     */
    private static <T extends Comparable<T>> void swap(T[] arr, int i, int j) {
        T temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }
}
