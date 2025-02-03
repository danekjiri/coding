package cz.cuni.mff.danekji1.util;

import java.util.concurrent.ForkJoinPool;
import java.util.concurrent.RecursiveAction;

public class Arrays {

    /**
     * Sorts the given array in ascending order using parallel merge sort.
     *
     * @param arr the array to be sorted; must not be null or of length less than 2
     */
    public static void paraMergeSort(int[] arr) {
        if (arr == null) {
            throw new NullPointerException("Array is null");
        }

        if (arr.length < 2) {
            return;
        }

        try (var pool = ForkJoinPool.commonPool()) {
            pool.invoke(new MergeSortTask(arr, 0, arr.length - 1));
        }
    }

    /**
     * A task for performing merge sort on a portion of an array.
     */
    private static class MergeSortTask extends RecursiveAction {
        private final int[] array;
        private final int right;
        private final int left;

        /**
         * Creates a new merge sort task for the specified range of the array.
         *
         * @param array the array to sort
         * @param right the index of the right boundary (inclusive)
         * @param left  the index of the left boundary (inclusive)
         */
        MergeSortTask(int[] array, int left, int right) {
            this.array = array;
            this.left = left;
            this.right = right;
        }

        /**
         * Executes the merge sort algorithm for the assigned range.
         * If the range contains more than one element, it splits the task
         * into subtasks and merges the sorted results.
         */
        @Override
        protected void compute() {
            // Base case: array section contains one or no elements
            if (right <= left) {
                return;
            }

            int middle = left + ((right - left) / 2);

            // Divide into two parts to be sorted and merged
            MergeSortTask leftTask = new MergeSortTask(array, left, middle);
            MergeSortTask rightTask = new MergeSortTask(array, middle + 1, right);

            invokeAll(leftTask, rightTask);
            merge(array, left, middle, right);
        }

        /**
         * Merges two sorted halves of the array into a single sorted segment.
         *
         * @param array  the array containing the segments to merge
         * @param left   the starting index of the left segment
         * @param middle the ending index of the left segment (middle of range)
         * @param right  the ending index of the right segment
         */
        private void merge(int[] array, int left, int middle, int right) {
            var temp = new int[right - left + 1];
            int leftIndex = left;
            int rightIndex = middle + 1;
            int mergingIndex = 0;

            // Merge two sorted halves into one
            while ((rightIndex <= right) && (leftIndex <= middle)) {
                if (array[leftIndex] <= array[rightIndex]) {
                    temp[mergingIndex] = array[leftIndex];
                    leftIndex++;
                } else {
                    temp[mergingIndex] = array[rightIndex];
                    rightIndex++;
                }
                mergingIndex++;
            }

            // Copy remaining elements from the left half
            while (leftIndex <= middle) {
                temp[mergingIndex] = array[leftIndex];
                leftIndex++;
                mergingIndex++;
            }

            // Copy remaining elements from the right half
            while (rightIndex <= right) {
                temp[mergingIndex] = array[rightIndex];
                rightIndex++;
                mergingIndex++;
            }

            // Copy the merged segment back into the original array
            System.arraycopy(temp, 0, array, left, temp.length);
        }
    }
}