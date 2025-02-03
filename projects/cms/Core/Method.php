<?php

namespace Core;

enum Method: string {
    case POST = 'POST';
    case GET = 'GET';
    case DELETE = 'DELETE';
}