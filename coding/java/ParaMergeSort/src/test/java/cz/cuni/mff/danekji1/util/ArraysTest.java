package cz.cuni.mff.danekji1.util;

import org.junit.jupiter.api.Test;

import java.util.Random;

import static org.junit.jupiter.api.Assertions.*;

class ArraysTest {

    @Test
    void NullArrayPassed() {
        assertThrows(NullPointerException.class, () -> Arrays.paraMergeSort(null));
    }

    @Test
    void EmptyArrayPassed() {
        var emptyArray = new int[] {};
        assertDoesNotThrow(() -> Arrays.paraMergeSort(emptyArray));
        assertArrayEquals(new int[0], emptyArray);
    }

    @Test
    void OneElementArrayPassed() {
        var oneElementArray = new int[] { 1 };
        Arrays.paraMergeSort(oneElementArray);
        assertArrayEquals(new int[] {1}, oneElementArray);
    }

    @Test
    void twoOrderedElementArrayPassed() {
        var twoOrderedElementArray = new int[] { 1, 2 };
        Arrays.paraMergeSort(twoOrderedElementArray);
        assertArrayEquals(new int[] {1, 2}, twoOrderedElementArray);
    }

    @Test
    void twoUnorderedElementArrayPassed() {
        var twoUnorderedElementArray = new int[] { 2, 1 };
        Arrays.paraMergeSort(twoUnorderedElementArray);
        assertArrayEquals(new int[] {1, 2}, twoUnorderedElementArray);
    }

    @Test
    void largeOrderedElementArrayPassed() {
        int numberOfElements = 1_000;
        var largeOrderedElementArraySorted = new int[numberOfElements];
        var largeOrderedElementArray = new int[numberOfElements];
        for(int i = 0; i < numberOfElements; i++) {
            largeOrderedElementArraySorted[i] = i;
            largeOrderedElementArray[i] = i;
        }

        Arrays.paraMergeSort(largeOrderedElementArraySorted);

        assertArrayEquals(largeOrderedElementArray, largeOrderedElementArraySorted);
    }

    @Test
    void largeUnorderedElementArrayPassed() {
        int numberOfElements = 1_000;
        Random random = new Random(0xC0FFEE);
        var largeUnorderedElementArrayMyParaSort = new int[numberOfElements];
        var largeUnorderedElementArrayLibrarySort = new int[numberOfElements];
        for(int i = 0; i < numberOfElements; i++) {
            int randomInt = random.nextInt();
            largeUnorderedElementArrayMyParaSort[i] = randomInt;
            largeUnorderedElementArrayLibrarySort[i] = randomInt;
        }

        Arrays.paraMergeSort(largeUnorderedElementArrayMyParaSort);
        java.util.Arrays.sort(largeUnorderedElementArrayLibrarySort);

        assertArrayEquals(largeUnorderedElementArrayLibrarySort, largeUnorderedElementArrayMyParaSort);
    }
}
