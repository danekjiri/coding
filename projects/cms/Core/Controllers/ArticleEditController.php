<?php

namespace Core\Controllers;

use Core\Models\ArticleRepository;
use Core\{Request, Response, Code, ResponseType, Config};

class ArticleEditController {
    private ArticleRepository $repo;

    public function __construct() {
        $this->repo = new ArticleRepository();
    }

    /**
     * Get the article to edit, parse the article edit view and return it in Response $body.
     * If article not found return 404.
     */
    public function edit(Request $req): Response {
        $id = $req->data['id'];
        $article = $this->repo->getById($id);
        if (!$article) {
            // 404
            return ErrorController::notFound($req, "Article 'id:$id' to be shown in edit was not found");
        }

        ob_start();
        $articleToEdit = $article;
        // interpret article edit view
        include Config::PROJECT_BASE . '/Views/article-edit.php';
        $body = ob_get_clean();

        return new Response(
            code: Code::OK,
            type: ResponseType::HTML,
            body: $body
        );
    }

    /**
     * Save the edited article and return Response with redirection to the first page of article list.
     * If article not found return 404.
     * If name is empty or name/content is too long (32/1024 chars) show an error.
     */
    public function save(Request $req): Response {
        $id = $req->data['id'];
        $article = $this->repo->getById($id);
        if (!$article) {
            // 404
            return ErrorController::notFound($req, "Article 'id:$id' to be updated was not found");
        }

        $name = $req->data['articleName'] ?? '';
        $content = $req->data['articleContent'] ?? '';
        if ($name === '' || mb_strlen($name, 'UTF-16') > 32 || mb_strlen($content, 'UTF-16') > 1024) {
            // name is empty or too long || content is too long
            return new Response(
                code: Code::BAD_REQUEST,
                type: ResponseType::HTML,
                body: '<h3>Bad Request: Article name cannot be empty or longer than 32 characters. Content cannot be longer than 1024 characters.</h3>'
            );
        }

        // update article
        $article->setName($name);
        $article->setContent($content);
        $this->repo->update($article);

        // after saving, redirect back to the first page of article list
        return new Response(
            code: Code::SEE_OTHER,
            type: ResponseType::HTML,
            redirectUrl: Config::BASE_PATH . '/articles'
        );
    }
}