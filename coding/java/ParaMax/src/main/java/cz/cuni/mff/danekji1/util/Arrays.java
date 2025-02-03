package cz.cuni.mff.danekji1.util;

import java.util.concurrent.*;
import java.util.ArrayList;
import java.util.List;

public class Arrays {

    /**
     * Finds the maximum value in the given array using a parallel approach.
     *
     * @param arr the array of integers
     * @return the maximum integer in arr
     * @throws IllegalArgumentException if arr is null or empty
     */
    public static int max(int[] arr) throws IllegalArgumentException {
        if (arr == null || arr.length == 0) {
            throw new IllegalArgumentException("Array cannot be null or empty.");
        }

        int numThreads = Runtime.getRuntime().availableProcessors();
        int globalMax;
        try (ExecutorService executor = Executors.newFixedThreadPool(numThreads)) {
            int chunkSize = (arr.length + numThreads - 1) / numThreads;
            List<Future<Integer>> futures = new ArrayList<>();

            // submit a task for each chunk to find the local maximum
            for (int start = 0; start < arr.length; start += chunkSize) {
                final int begin = start;
                final int end = Math.min(start + chunkSize, arr.length);

                Callable<Integer> task = () -> {
                    int localMax = arr[begin];
                    for (int i = begin + 1; i < end; i++) {
                        if (arr[i] > localMax) {
                            localMax = arr[i];
                        }
                    }
                    return localMax;
                };

                futures.add(executor.submit(task));
            }

            // gather the results and find the global maximum
            globalMax = Integer.MIN_VALUE;
            try {
                for (Future<Integer> future : futures) {
                    int localMax = future.get();
                    if (localMax > globalMax) {
                        globalMax = localMax;
                    }
                }
            } catch (InterruptedException | ExecutionException e) {
                throw new RuntimeException("Error during parallel max computation", e);
            }
            finally {
                executor.shutdownNow();
            }
        }

        return globalMax;
    }
}
