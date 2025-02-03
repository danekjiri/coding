<?php

namespace Core;

class Response {
    private Code $code;
    private ResponseType $type;
    private ?string $body;
    private ?string $redirectUrl;

    public function __construct($code = Code::OK, $type = ResponseType::HTML, ?string $body = null, ?string $redirectUrl = null) {
        $this->code = $code;
        $this->type = $type;
        $this->body = $body;
        $this->redirectUrl = $redirectUrl;
    }
    
    /**
     * Send response to client.
     */
    public function send(): void {
        // set status code
        http_response_code($this->code->value);

        // if redirection, set Location header and return
        if ($this->redirectUrl && $this->code === Code::SEE_OTHER) {
            header('Location: ' . $this->redirectUrl, true, $this->code->value);
            return;
        }
        
        // otherwise set content type and echo body if any
        header('Content-Type: ' . $this->type->value . '; charset=utf-8');
        if ($this->body !== null) {
            echo $this->body;
        }
    }
}