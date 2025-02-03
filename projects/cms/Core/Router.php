<?php

namespace Core;

/**
 * cases:
 * 1) GET /articles
 * 2) POST /article 
 * 3) DELETE /article/{id}
 * 4) GET /article/{id}
 * 5) GET /article-edit/{id}
 * 6) POST /article-edit/{id}
 */

class Router {
    /**
     * With provided path and method choose the appropriate controller and its method
     */
    public function route(Request $req): Route {
        $path = trim($req->url, '/'); // see Request.php
        $parts = explode('/', $path);

        // default => list articles
        if ($path === '' || $path === 'index.php') {
            return new Route (
                controllerClassPath: 'Core\\Controllers\\ArticlesController',
                methodName: 'index',
                params: []
            );
        }

        // 1) list articles => GET /articles
        if ($path === 'articles' && $req->method === Method::GET) {
            return new Route (
                controllerClassPath: 'Core\\Controllers\\ArticlesController',
                methodName: 'index',
                params: []
            );
        }

        // 2) create a new article => POST /article
        if ($path === 'article' && $req->method === Method::POST) {
            return new Route (
                controllerClassPath: 'Core\\Controllers\\ArticlesController',
                methodName: 'create',
                params: []
            );
        }

        if ($parts[0] === 'article' && isset($parts[1])) {
            $id = (int) $parts[1];
            // 3) delete article => DELETE /article/{id}
            if ($req->method === Method::DELETE) {
                return new Route (
                    controllerClassPath: 'Core\\Controllers\\ArticleController',
                    methodName: 'delete',
                    params: ['id' => $id]
                );
            // 4) show article detail (name & content) => GET /article/{id}
            } elseif ($req->method === Method::GET) {
                return new Route (
                    controllerClassPath: 'Core\\Controllers\\ArticleController',
                    methodName: 'show',
                    params: ['id' => $id]
                );
            } 
        }

        if ($parts[0] === 'article-edit' && isset($parts[1])) {
            $id = (int)$parts[1];
            // 5) show article edit form => GET /article-edit/{id}
            if ($req->method === Method::GET) {
                return new Route (
                    controllerClassPath: 'Core\\Controllers\\ArticleEditController',
                    methodName: 'edit',
                    params: ['id' => $id]
                );
            // 6) save article => POST /article-edit/{id}
            } elseif ($req->method === Method::POST) {
                return new Route (
                    controllerClassPath: 'Core\\Controllers\\ArticleEditController',
                    methodName: 'save',
                    params: ['id' => $id]
                );
            }
        }

        // no path match => code: 404
        return new Route (
            controllerClassPath:'Core\\Controllers\\ErrorController',
            methodName: 'notFound',
            params: [] 
        );
    }
}
