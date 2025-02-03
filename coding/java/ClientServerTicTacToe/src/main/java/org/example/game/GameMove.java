package org.example.game;

import java.io.Serializable;

public record GameMove(int row, int column) implements Serializable { }
