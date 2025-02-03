<?php

namespace Core\Controllers;

use Core\Models\ArticleRepository;
use Core\{Request, Response, Code, ResponseType, Config};

class ArticlesController {
    private ArticleRepository $repo;

    public function __construct() {
        $this->repo = new ArticleRepository();
    }

    /**
     * Parse the articles list view and return it in Response $body.
     */
    public function index(Request $req): Response {
        $articles = $this->repo->getAll();
        
        ob_start();
        // interpret article list view
        include Config::PROJECT_BASE . '/Views/articles.php';
        $body = ob_get_clean();

        return new Response(
            code: Code::OK,
            type: ResponseType::HTML,
            body: $body
        );
    }

    /**
     * Create a new article (POST /article) and return Response with redirection to article edit page.
     */
    public function create(Request $req): Response {
        // $_POST stored into Request->data in FrontController::handle
        $name = $req->data['name'] ?? '';
        $name = trim($name);
        if ($name === '' || strlen($name) > 32) {
            $html = $name === '' ? "<h3>Bad Request: name is empty.</h3>" : "<h3>Bad Request: name is too long.</h3>";
            return new Response(
                code: Code::BAD_REQUEST,
                type: ResponseType::HTML,
                body: $html
            );
        }

        $newId = $this->repo->create($name);
        // redirected to the article edit page
        return new Response(
            code: Code::SEE_OTHER,
            type: ResponseType::HTML,
            redirectUrl: Config::BASE_PATH . "/article-edit/{$newId}"
        );
    }
}
