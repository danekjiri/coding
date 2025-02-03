package cz.cuni.mff.danekji1.util;

import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;

public class ArraysTest {

    @Test
    public void maxSimple() {
        int[] arr = { 1, 2, 3, 10, 5 };
        int result = Arrays.max(arr);
        Assertions.assertEquals(10, result, "Max should be 10");
    }

    @Test
    public void maxNegative() {
        int[] arr = { -5, -10, -3, -100, -1 };
        int result = Arrays.max(arr);
        Assertions.assertEquals(-1, result, "Max among negatives should be -1");
    }

    @Test
    public void maxSingleElement() {
        int[] arr = { 42 };
        int result = Arrays.max(arr);
        Assertions.assertEquals(42, result, "Max in a single-element array should be that element");
    }

    @Test
    public void testMaxThrowsOnEmpty() {
        int[] arr = {};
        Assertions.assertThrows(IllegalArgumentException.class, () -> {
            Arrays.max(arr);
        }, "Should throw IllegalArgumentException for empty array");
    }
}
