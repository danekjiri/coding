package cz.cuni.mff.danekji1.util;

import java.util.List;

public class DynamicArray<T extends Comparable<T>> implements SimpleCollection<T> {
    private T[] storage;
    private int elementsCount = 0;
    public static final int DEFAULT_COLLECTION_SIZE = 4;

    // Default constructor, initializes the array with default size
    public DynamicArray() {
        this(DEFAULT_COLLECTION_SIZE);
    }

    @SuppressWarnings("unchecked")
    public DynamicArray(int collectionSize) {
        storage = (T[]) new Object[collectionSize];
    }

    // Constructor that initializes from a given array
    public DynamicArray(T[] collection, int collectionSize) {
        storage = collection.clone();
        elementsCount = collectionSize;
    }

    // Returns the number of elements in the array
    public int size() {
        return elementsCount;
    }

    // Expands the storage capacity two times when needed
    @SuppressWarnings("unchecked")
    private void extendStorage() {
        T[] extendedStorage = (T[]) new Object[storage.length * 2];
        System.arraycopy(storage, 0, extendedStorage, 0, storage.length);
        storage = extendedStorage;
    }

    // Adds a new element to the array
    public void add(T element) {
        if (storage.length == elementsCount) {
            extendStorage();
        }
        storage[elementsCount] = element;
        elementsCount++;
    }

    // Retrieves an element by its index
    public T get(int i) throws IndexOutOfBoundsException {
        if (i > elementsCount - 1) {
            throw new IndexOutOfBoundsException();
        }
        return storage[i];
    }

    // Removes an element by its value
    public void remove(T element) {
        for (int i = storage.length - 1; i >= 0; i--) {
            if (element.equals(storage[i])) {
                remove(i);
                return;
            }
        }
    }

    // Removes an element by its index
    public void remove(int i) throws IndexOutOfBoundsException {
        if (i > elementsCount - 1) {
            throw new IndexOutOfBoundsException();
        }
        for (int j = i; j <= elementsCount - 1; j++) {
            storage[j] = storage[j + 1];
        }
        elementsCount--;
        storage[elementsCount] = null;
    }

    /**
     * Sort instance of (this) DynamicArray
     */
    public void sort() {
        QuickSort.sort(storage);
    }

    /**
     * Instantiate an DynamicArray out of given T Type elements passed as arguments
     * @param elements elements to be passed into created DynamicArray
     * @return DynamicArray filled with args
     * @param <T> Type of elements passed into args
     */
    @SafeVarargs
    public static <T extends Comparable<T>> DynamicArray<T> of(T ... elements) {
        var array = new DynamicArray<T>(elements.length);
        for (var element : elements) {
            array.add(element);
        }
        return array;
    }
}