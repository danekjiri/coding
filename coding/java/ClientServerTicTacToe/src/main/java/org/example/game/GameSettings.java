package org.example.game;

import java.io.Serializable;

public record GameSettings(int gameFieldSize, int sequenceLengthToWin) implements Serializable { }
