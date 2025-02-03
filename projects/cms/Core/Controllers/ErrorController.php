<?php

namespace Core\Controllers;

use Core\{Config, Code, ResponseType, Request, Response};

class ErrorController {
    /**
     * Returns a 404 response
     */
    public static function notFound(Request $req, string $message = null): Response {
        ob_start();
        if ($message) {
            $messageToShow = $message;
        }
        include Config::PROJECT_BASE . '/Views/404.php';
        $body = ob_get_clean();

        // return a 404 response
        return new Response(
            code: Code::NOT_FOUND,
            type: ResponseType::HTML,
            body: $body
        );
    }
}
