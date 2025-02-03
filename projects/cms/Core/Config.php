<?php

namespace Core;

class Config {
    const DEBUG = true;

    // File-system base (for includes)
    const PROJECT_BASE = __DIR__ . "/..";

    // URL base
    const BASE_PATH = '/~73999444/cms';

    /**
     * Debug helper
     */
    public static function debug(string $string): void
    {
        if (self::DEBUG) {
            echo "<div style='color:red'><pre>DEBUG: " . htmlspecialchars($string) . "</pre></div>";
        }
    }
}
