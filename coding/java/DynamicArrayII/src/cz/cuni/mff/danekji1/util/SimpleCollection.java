package cz.cuni.mff.danekji1.util;

interface SimpleCollection<T> {
    void add(T element);
    T get(int i);
    void remove(T element);
    void remove(int i);
    int size();
}