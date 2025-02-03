<?php

namespace Core;

class Request {
    public Method $method; // GET, POST, DELETE, etc.
    public string $url;   // e.g., "/articles" || "article-edit/12" || "article/12"
    public array  $data;  // post&get form data, query params, etc.

    public function __construct(Method $method, string $url, array $data = []) {
        $this->method = $method;
        $this->url = $url;
        $this->data = $data;
    }
}
