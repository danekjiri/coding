<?php

namespace Core;

class Route {
    public string $controllerClassPath;
    public string $methodName;
    public array $params; // rest of the URL parts eg.: /article/1 => ['id' => 1]
    
    public function __construct(string $controllerClassPath, string $methodName, array $params) {
        $this->controllerClassPath = $controllerClassPath;
        $this->methodName = $methodName;
        $this->params = $params;
    }
}