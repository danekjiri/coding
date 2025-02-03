<?php

namespace Core\Controllers;

use Core\Models\ArticleRepository;
use Core\{Request, Response, Code, ResponseType, Config};

class ArticleController {
    private ArticleRepository $repo;

    public function __construct() {
        $this->repo = new ArticleRepository();
    }

    /**
     * Get the article by id and parse it to the article view returning it in Response $body.
     * If the article is not found, return a 404 response.
     */
    public function show(Request $req): Response {
        $id = $req->data['id'];
        $article = $this->repo->getById($id);
        if (!$article) {
            // 404
            return ErrorController::notFound($req, "Article 'id:$id' to be shown was not found");
        }

        ob_start();
        $articleToShow = $article;
        // interpret article view
        include Config::PROJECT_BASE . '/Views/article.php';
        $body = ob_get_clean();

        return new Response(
            code: Code::OK,
            type: ResponseType::HTML,
            body: $body
        );
    }

    /**
     * AJAX endpoint to delete the article.
     * Get the article by id and return a JSON response with the status of the deletion.
     * If the article is not found return error json response.
     */
    public function delete(Request $req): Response {
        $id = $req->data['id'];
        $article = $this->repo->getById($id);
        if (!$article) {
            // return JSON error response
            return new Response(
                code: Code::NOT_FOUND,
                type: ResponseType::JSON,
                body: json_encode(['error' => "Article 'id:$id' not found"])
            );
        }

        $this->repo->delete($id);
        // return JSON success response
        return new Response(
            code: Code::OK,
            type: ResponseType::JSON,
            body: json_encode(['status' => 'ok', 'message' => "Article 'id:$id' deleted"])
        );
    }
}
