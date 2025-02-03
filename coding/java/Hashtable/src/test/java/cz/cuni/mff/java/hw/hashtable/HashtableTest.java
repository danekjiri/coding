package cz.cuni.mff.java.hw.hashtable;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import java.util.HashSet;
import static org.assertj.core.api.Assertions.*;

import static org.junit.jupiter.api.Assertions.*;

class HashtableTest {

    private Hashtable<Integer, String> hashtable;

    @BeforeEach
    void setUp() {
        hashtable = new Hashtable<>(5);
    }

    @Test
    void addSingleEntry() {
        hashtable.set(1, "one");
        assertThat(hashtable.get(1)).isEqualTo("one");
    }

    @Test
    void overrideExistingEntry() {
        hashtable.set(1, "one");
        assertThat(hashtable.get(1)).isEqualTo("one");

        hashtable.set(1, "uno"); // override existing one
        assertThat(hashtable.get(1)).isEqualTo("uno");
    }

    @Test
    void setMultipleEntries() {
        hashtable.set(1, "one");
        hashtable.set(2, "two");
        hashtable.set(3, "three");

        assertThat(hashtable).containsExactly(1, 2, 3) ;
        assertThat(hashtable.get(1)).isEqualTo("one");
        assertThat(hashtable.get(2)).isEqualTo("two");
        assertThat(hashtable.get(3)).isEqualTo("three");
    }

    @Test
    void handleSetNullKey() {
        assertThrows(IllegalArgumentException.class, () -> hashtable.set(null, "nullValue"));
    }

    @Test
    void handleSetNullValue() {
        assertThrows(IllegalArgumentException.class, () -> hashtable.set(1, null));
    }

    @Test
    void expansionOfTable() {
        final int NUMBER_OF_ITERATIONS = 10;
        for (int i = 1; i <= NUMBER_OF_ITERATIONS; i++) {
            hashtable.set(i, "value" + i);
        }

        for (int i = 1; i <= NUMBER_OF_ITERATIONS; i++) {
            assertThat(hashtable.get(i)).isEqualTo("value" + i);
        }
    }

    @Test
    void setEntriesWithCollisions() {
        // Hash collision scenario: forcing keys to collide by implementing custom hashCode in Entry
        hashtable.set(1, "one");
        hashtable.set(6, "six"); // Collides with 1 due to small capacity (index = hash % 5)

        assertThat(hashtable.get(1)).isEqualTo("one");
        assertThat(hashtable.get(6)).isEqualTo("six");
        assertThat(hashtable).containsExactly(1, 6);
    }

    @Test
    void getNonExistentKey() {
        hashtable.set(1, "one");

        assertThat(hashtable.get(1)).isEqualTo("one");
        assertThat(hashtable.get(2)).isNullOrEmpty();
    }

    @Test
    void iteratorTest() {
        hashtable.set(1, "one");
        hashtable.set(2, "two");
        hashtable.set(3, "three");

        var iterator = hashtable.iterator();
        var keys = new HashSet<Integer>();

        while (iterator.hasNext()) {
            keys.add(iterator.next());
        }

        assertThat(hashtable.size()).isEqualTo(keys.size());
        assertThat(keys).containsExactly(1, 2, 3);
    }

    @Test
    void iteratorEmptyTable() {
        var iterator = hashtable.iterator();
        assertThat(iterator.hasNext()).isFalse();
    }

    @Test
    void iteratorOutOfBounds() {
        hashtable.set(1, "one");
        var iterator = hashtable.iterator();

        iterator.next();
        assertThat(iterator.hasNext()).isFalse();
        assertThrows(IndexOutOfBoundsException.class, iterator::next);
    }

    @Test
    void tableRehashing() {
        hashtable.set(1, "one");
        hashtable.set(2, "two");
        hashtable.set(3, "three");
        hashtable.set(4, "four");
        hashtable.set(5, "five");
        hashtable.set(6, "six"); // should rehash and add sixth

        assertThat(hashtable.get(1)).isEqualTo("one");
        assertThat(hashtable.get(2)).isEqualTo("two");
        assertThat(hashtable.get(3)).isEqualTo("three");
        assertThat(hashtable.get(4)).isEqualTo("four");
        assertThat(hashtable.get(5)).isEqualTo("five");
        assertThat(hashtable.get(6)).isEqualTo("six");
        assertThat(hashtable.size()).isEqualTo(6);
    }

    @Test
    void forEachValue() {
        StringBuilder output = new StringBuilder();
        hashtable.set(1, "one");
        hashtable.set(2, "two");
        hashtable.set(3, "three");

        hashtable.forEachValue(output::append);
        assertThat(output).containsSequence("one", "two", "three");
        assertThat(output.toString()).isEqualTo("onetwothree");
    }

    @Test
    void entityEquals() {
        Hashtable.Entry<String, Integer> nini21a = new Hashtable.Entry<>("nini", 21);
        Hashtable.Entry<String, Integer> nini21b = new Hashtable.Entry<>("nini", 21);
        Hashtable.Entry<String, Integer> nini21c = new Hashtable.Entry<>("nini", 21);
        Hashtable.Entry<String, Integer> jirc21 = new Hashtable.Entry<>("jirc", 21);
        Hashtable.Entry<String, Integer> jirc22 = new Hashtable.Entry<>("jirc", 22);
        Hashtable.Entry<Integer, String> jirc42 = new Hashtable.Entry<>(42, "jirc");

        // Assert that nini21a is not equal to null
        assertThat(nini21a.equals(null)).isFalse();
        // Assert reflexivity: an object should be equal to itself
        assertThat(nini21a.equals(nini21a)).isTrue();
        // Assert transitivity
        assertThat(nini21a.equals(nini21b) && nini21b.equals(nini21c) ? nini21a.equals(nini21c) : true).isTrue();
        // Assert equality of entities with same fields
        assertThat(nini21a.equals(nini21b)).isTrue();
        // Assert inequality of entities with different keys
        assertThat(nini21a.equals(jirc21)).isFalse();
        // Assert inequality of entities with different values
        assertThat(jirc22.equals(jirc21)).isFalse();
        // Assert inequality of entities with both different keys and values
        assertThat(nini21a.equals(jirc22)).isFalse();
        // Assert inequality of entities with completely different types
        assertThat(jirc22.equals(jirc42)).isFalse();
    }

}
