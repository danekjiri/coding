<?php

namespace Core;

enum ResponseType: string {
    case HTML = 'text/html';
    case JSON = 'application/json';
}