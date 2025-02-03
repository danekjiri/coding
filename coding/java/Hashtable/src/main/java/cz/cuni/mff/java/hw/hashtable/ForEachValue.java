package cz.cuni.mff.java.hw.hashtable;

@FunctionalInterface
public interface ForEachValue<TValue> {
    void call(TValue value);
}