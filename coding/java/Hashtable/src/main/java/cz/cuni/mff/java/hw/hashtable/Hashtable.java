package cz.cuni.mff.java.hw.hashtable;

import java.util.Iterator;

/**
 * Simple implementation of Hashtable with generics type of Keys and Values.
 *  There are only two base methods for accessing the hashtable such as {@link #set(Object, Object)}
 *   and {@link #get(Object)} and an iterator over given keys.
 *  There is an inner Entry class implementation that holds the key-value pair {@link Entry}
 *
 * @param <TKey> The type for key representation with properly implemented {@link #hashCode()} method
 * @param <TValue> The corresponding value type to given key
 */
public class Hashtable<TKey, TValue> implements Iterable<TKey> {
    /**
     * Could be record class - but I wanted to try to implement equal contract {@link #hashCode()} {@link #equals(Object)}
     * An inner class representing the data key-value pair to be stored in hashtable array.
     *
     * @param <TKey> The type for key representation with properly implemented {@link #hashCode()} method
     * @param <TValue> The corresponding value type to given key
     */
    public static class Entry<TKey, TValue> {
        public final TKey key;
        public final TValue value;

        public Entry(TKey key, TValue value) {
            this.key = key;
            this.value = value;
        }

        /**
         * {@link Object#toString()}
         */
        @Override
        public String toString() {
            return "[" + key + ", " + value + "]";
        }

        /**
         * {@link Object#hashCode()}
         *
         * @throws NullPointerException Key or Value is set to null
         */
        @Override
        public int hashCode() throws NullPointerException {
            return key.hashCode() + value.hashCode();
        }

        /**
         * {@link Object#equals(Object)}
         */
        @Override
        public boolean equals(Object obj) {
            if (this == obj) {
                return true;
            }

            if (obj == null || getClass() != obj.getClass()) {
                return false;
            }

            var other = (Entry<?, ?>) obj;
            return this.key.equals(other.key) && this.value.equals(other.value);
        }
    }

    public void forEachValue(ForEachValue<TValue> operation) {
        for (var key : this) {
            TValue value = this.get(key);
            operation.call(value);
        }
    }

    public static int FULL_TABLE_EXPANSION_RATIO = 2;
    public static final int DEFAULT_CAPACITY = 16;
    private static final int TABLE_FULL = -1;
    private Entry<?, ?>[] table;
    private int count;

    /**
     * The ctor for {@link Hashtable} where the initial capacity of storage is set by user
     *
     * @param initialCapacity The initial capacity of {@link Hashtable}
     * @throws IllegalArgumentException When the 'initialCapacity' argument is less than one
     */
    public Hashtable(int initialCapacity) throws IllegalArgumentException {
        if (initialCapacity <= 0) {
            throw new IllegalArgumentException("The capacity must be positive.");
        }
        table = new Entry[initialCapacity];
        count = 0;
    }

    /**
     * Default ctor for {@link Hashtable} class that init its storage to {@link #DEFAULT_CAPACITY}
     */
    public Hashtable() {
        this(DEFAULT_CAPACITY);
    }

    /**
     * Method for getting value of given key argument if key is not present in the {@link Hashtable}
     *  then returns null
     *
     * @param key Key that corresponds to searched data
     * @return Value if key present otherwise null
     */
    public TValue get(TKey key) throws IllegalArgumentException {
        if (key == null) {
            throw new IllegalArgumentException("The key cannot be null.");
        }

        int index = getIndex(key, table);
        if (index == TABLE_FULL || table[index] == null) {
            return null;
        }
        TValue value = (TValue) table[index].value;
        return value;
    }

    /**
     * Method for storing the given key-value pair in the {@link Hashtable}
     *  Setting value that is already present in the {@link Hashtable} will override inserted Value
     *  Also when the storage of {@link Hashtable} is full, then storage is expanded by
     *   {@link #FULL_TABLE_EXPANSION_RATIO} and all current key-value pairs are rehashed into new storage
     *
     * @param key Key that corresponds to given Value
     * @param value Value that corresponds to given Key
     */
    public void set(TKey key, TValue value) throws IllegalArgumentException {
        if (key == null || value == null) {
             throw new IllegalArgumentException("The key either the value cannot be null.");
        }

        var newEntry = new Entry<>(key, value);
        /// expand and rehash the table if is full
        if (table.length == count) {
            table = expandTable(table.length * FULL_TABLE_EXPANSION_RATIO);
        }

        int index = getIndex(newEntry.key, table);
        /// index is space for new entry, if not null then overriding matching key {@link #getIndex}
        if (table[index] == null) {
            count++;
        }
        table[index] = newEntry;
    }

    /**
     * Get number of elements in hashtable
     * @return number of elements
     */
    public int size() {
        return count;
    }

    /**
     * Get the index where the new key-value pair {@link Entry} could be inserted
     *  If key is already present in table of type {@link Hashtable} then return its position
     *   otherwise position where new key should be placed
     *  The method is used for rehashing to new table so that the table argument
     *
     * @param key Key which position in storage we want to find out
     * @param table Table in which we want to find out the index of key
     * @return Index of key to be inserted OR when table is full and key not present then -1
     */
    private int getIndex(TKey key, Entry<?, ?>[] table) {
        int keyHash = key.hashCode();
        int currentIndex = (keyHash & Integer.MAX_VALUE) % table.length;
        final int index = currentIndex;

        /// find free index if current position is occupied
        while (table[currentIndex] != null) {
            if (key.equals(table[currentIndex].key)) {
                /// the given key has been already added so table replace its value
                return currentIndex;
            }

            currentIndex++;
            /// could overflow the storage so return back to start index
            currentIndex %= table.length;

            /// the cycle went through the whole storage and did not find place or equal key
            if (currentIndex == index) {
                return TABLE_FULL;
            }
        }

        return currentIndex;
    }

    /**
     * Expand the current {@link Hashtable} storage capacity by given 'newCapacity'
     *  and rehash all key-value pairs into new storage that is returned
     *
     * @param newCapacity New {@link Hashtable} storage capacity
     * @return New {@link Hashtable} storage with rehashed previous key-value pairs
     */
    private Entry<TKey, TValue>[] expandTable(int newCapacity) {
        assert newCapacity > table.length;
        var newTable = (Entry<TKey, TValue>[]) new Entry[newCapacity];
        rehashEntries(table, newTable);

        return newTable;
    }

    /**
     * Rehash all entries from old hashtable storage into new hash table storage
     *
     * @param oldTable storage from the entries being hashed
     * @param newTable storage to the entries being hashed
     */
    private void rehashEntries(Entry<TKey, TValue>[] oldTable, Entry<TKey, TValue>[] newTable) {
        assert oldTable.length <= newTable.length;
        for (var entry : oldTable) {
            int index = getIndex(entry.key, newTable);
            newTable[index] = entry;
        }
    }

    /**
     * Iterator over the keys of {@link Hashtable}
     *
     * @return iterator which iterates over keys using {@link Iterator#next()} and {@link Iterator#hasNext()}
     */
    public Iterator<TKey> iterator() {
        return new Iterator<>() {
            private int it_counter = 0;
            private int table_index = 0;

            /**
             * Find out if there is another element in the storage
             *
             * @return True if next element exists otherwise false
             */
            @Override
            public boolean hasNext() {
                return it_counter < count;
            }

            /**
             * Get the current element and move to the next one
             *
             * @return currently pointing element
             * @throws IndexOutOfBoundsException Try to access element out of storage
             */
            @Override
            public TKey next() throws IndexOutOfBoundsException {
                if (it_counter >= count) {
                    throw new IndexOutOfBoundsException();
                }

                assert table_index < table.length;
                while (table[table_index] == null) {
                    table_index++;
                }
                TKey key = table[table_index].key;

                it_counter++;
                table_index++;
                return key;
            }
        };
    }
}