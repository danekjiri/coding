<?php

namespace Core;

use Core\Controllers\ErrorController;

class Dispatcher {
    /**
     * Dispatches the request to the appropriate controller and its method given in Route $route
     */
    public function dispatch(Route $route, Request $request): Response {
        // add the route's parameters to the request's data
        $request->data = array_merge($request->data, $route->params);

        // check if passed controller class exists
        if (!class_exists($route->controllerClassPath)) {
            $errorController = new ErrorController();
            Config::debug("Controller class '{$route->controllerClassPath}' not found");
            return $errorController->notFound($request);
        }

        $controller = new $route->controllerClassPath();
        // check if passed method exists in the controller
        if (!method_exists($controller, $route->methodName)) {
            $errorController = new ErrorController();
            Config::debug("Method '{$route->methodName}' not found in '{$route->controllerClassPath}'");
            return $errorController->notFound($request);
        }
        
        return call_user_func([$controller, $route->methodName], $request);
    }
}