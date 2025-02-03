<?php

// registrer an autoloader function to load classes automatically
spl_autoload_register(
    static function ($className) {
        // break the fully quialified class name into parts
        $classParts = explode('\\', $className);
        $classFileName = array_pop($classParts);

        // build the path to the class file
        $relativePath = empty($classParts)
            ? $classFileName . '.php'
            : implode(DIRECTORY_SEPARATOR, $classParts) . DIRECTORY_SEPARATOR . $classFileName . '.php';

        $absolutePath = __DIR__ . DIRECTORY_SEPARATOR . $relativePath;

        // require the class file if it exists
        if (file_exists($absolutePath)) {
            require_once $absolutePath;
        } else {
            die('Class ' . $className . ' not found');
        }
    }
);

use Core\FrontController;

// program entry point
$fc = new FrontController();
$fc->handle();
