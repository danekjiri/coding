<?php

namespace Core;

class FrontController {

    /**
     * Handles the request by routing it to the appropriate controller
     */
    public function handle(): void {
        // parse provided information about the request
        $method = Method::from($_SERVER['REQUEST_METHOD']);
        $page = $_GET['page'] ?? '';
        $data = array_merge($_GET, $_POST);

        $request = new Request($method, $page, $data);

        // route the request to the appropriate controller
        $router = new Router();
        $route = $router->route($request);

        $dispatcher = new Dispatcher();
        $response = $dispatcher->dispatch($route, $request);
        $response->send();
    }
}